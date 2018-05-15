using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using wvr;

// This class shares some similarity to SelectionProgress.
public class LighterTriggerProgress : MonoBehaviour
{
    /* progress */

    public event Action OnSelectionComplete;
    public float SelectionDuration { get { return m_SelectionDuration; } }

    [SerializeField]
    private float m_SelectionDuration = 2f;

    [SerializeField]
    private AudioSource m_Audio;
    [SerializeField]
    private AudioClip m_OnFilledClip;
    [SerializeField]
    private Transform m_ProgressableTransform;
    [SerializeField]
    private UIFader m_ProgressableFader;
    [SerializeField]
    private Image m_Progressable; 
    private Coroutine m_SelectionFillRoutine;
    private bool m_SelectionFilled;

    private bool m_GazeOver;
    private bool m_ButtonPressed;

    // use m_Lighter to set m_WaveVrDevice, m_InputToListen & m_EditorUseMouseButton
    [SerializeField]
    private Lighter m_Lighter;
    private WaveVR_Controller.Device m_WaveVrDevice;
    private WVR_InputId m_InputToListen;
    private int m_EditorUseMouseButton = 0;

    private bool m_IsTriggerPress;

    /* end of progressable */


    /* for tracking transform angles */

    // TODO: There is same thing in HandLighterSwitchControl.cs
    private const string HeadObjectName = "/VIVEFocusWaveVR/head";

    private static Vector3 StaticUp = Vector3.up;
    private static Vector3 StaticForward = Vector3.forward;
    private static Vector3 StaticRight = Vector3.right;

    // TODO: need to offset scene rotation
    [SerializeField]
    private float m_StaticForwardOffset;
    [SerializeField]
    private float m_LighterTargetHorizontalHalfAngleRange;
    [SerializeField]
    private float m_LighterTargetMaxAllowedHeight;
    [SerializeField]
    private float m_LighterTargetMinAllowedHeight;

    // m_Zenith & m_Azimuth are just for display, not to be set in Editor
    [SerializeField]
    private float m_Zenith = 0f;
    [SerializeField]
    private float m_Azimuth = 0f;
    
    private Transform m_HeadTransform;

    /* end of for tracking transform angles */


    /* MonoBehaviour */

    private void Awake()
    {
        m_HeadTransform = GameObject.Find(HeadObjectName).transform;
    }

    private void Start()
    {
        m_WaveVrDevice = WaveVR_Controller.Input(m_Lighter.DeviceToListen);
        m_InputToListen = m_Lighter.InputToListen;
        m_EditorUseMouseButton = m_Lighter.EditorUseMouseButton;

        SetProgressableValueToMin();
        
        // offset scene rotation + a fixed offset
        float sceneRotation = GameManager.Instance.CurrentSceneScroll.SkyShaderDefaultRotation;
        Quaternion rotationAlongY = Quaternion.Euler(0, sceneRotation + m_StaticForwardOffset, 0);
        StaticForward = rotationAlongY * StaticForward;
        StaticRight = rotationAlongY * StaticRight;
    }

    private void Update()
    {
        //Vector3 forwardVec = m_HeadTransform.forward;
        // forward direction points from head to this transform
        Vector3 forwardVec = transform.position - m_HeadTransform.position;
        m_Zenith = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(forwardVec, StaticUp));

        Vector3 normedProjectionOnFloor = Vector3.Normalize(forwardVec - new Vector3(0, forwardVec.y, 0));
        float signedMagnitudeOfSineAzimuth = Vector3.Dot(Vector3.Cross(StaticForward, normedProjectionOnFloor), StaticUp);
        float newAzimuth = Mathf.Rad2Deg * Mathf.Asin(signedMagnitudeOfSineAzimuth);

        Debug.DrawRay(transform.position, StaticUp, Color.green);
        Debug.DrawRay(transform.position, StaticForward, Color.yellow);
        Debug.DrawRay(transform.position, StaticRight, Color.red);

        Debug.DrawRay(transform.position, forwardVec, Color.blue);
        Debug.DrawRay(transform.position, normedProjectionOnFloor, Color.black);      

        // Debug.Log(newAzimuth);
        if (IsLighterWithinTargetZone(newAzimuth, transform.position))
        {
            // make progress bar always point to head
            m_ProgressableTransform.rotation =
                Quaternion.LookRotation(forwardVec);

            if (!m_GazeOver)
            {
                HandleEnter();
            }
            else
            {
#if UNITY_EDITOR
                m_IsTriggerPress = Input.GetMouseButton(m_EditorUseMouseButton);
#else
                m_IsTriggerPress = waveVrDevice.GetPress(m_InputToListen);        
#endif
                // Get button press state from controller device
                if (m_IsTriggerPress)
                {                                        
                    HandleDown();
                }
                else
                {
                    HandleUp();
                }
            }
        }
        else
        {
            if (m_GazeOver)
            {
                HandleExit();
            }
        }

        m_Azimuth = newAzimuth;
    }

    /* end of MonoBehaviour */


    /* audios */

    public void PlayOnFilledClip()
    {
        PlayAudioClip(m_OnFilledClip);
    }

    public void PlayAudioClip(AudioClip aClip)
    {
        //m_Audio.clip = aClip;
        //if (m_Audio.clip != null)
        //{
        //    m_Audio.Play();
        //}
    }

    /* end of audios */


    private bool IsLighterWithinTargetZone(float azimuth, Vector3 pos)
    {
        //Debug.Log(pos.y);
        return Mathf.Abs(azimuth) <= m_LighterTargetHorizontalHalfAngleRange
            && pos.y >= m_LighterTargetMinAllowedHeight
            && pos.y <= m_LighterTargetMaxAllowedHeight;
    }

    public IEnumerator WaitForSelectionToFill()
    {
        return new WaitUntil(() => m_SelectionFilled);
    }

    private IEnumerator FillSelection()
    {
        // At the start of the coroutine, the bar is not filled.
        m_SelectionFilled = false;

        // Create a timer and reset the fill amount.
        float timer = 0f;
        SetProgressableValueToMin();

        // This loop is executed once per frame until the timer exceeds the duration.
        while (timer < m_SelectionDuration)
        {
            // The selection's fill amount requires a value from 0 to 1 so we normalise the time.
            SetProgressableValue(timer / m_SelectionDuration);

            // Increase the timer by the time between frames and wait for the next frame.
            timer += Time.deltaTime;

            // Wait until next frame.
            yield return null;

            // The following code is just to play safe
            // if the StopCoroutine() is somehow not called

            // If the user is still looking at the selection,
            // go on to the next iteration of the loop.
            if (m_GazeOver && m_ButtonPressed)
                continue;

            // If the user is no longer looking at the selection,
            // reset the selection and leave the function.
            ResetSelectionProgress();
            yield break;
        }

        // When the loop is finished set the fill amount to be full.
        SetProgressableValueToMax();

        // The selection is now filled so the coroutine waiting for it can continue.
        m_SelectionFilled = true;

        RaiseOnSelectedEvent();

        // Play the clip for when the selection is filled.        
        PlayOnFilledClip();
    }

    private void StartSelectionFillRoutine()
    {
        // start filling it.        
        m_SelectionFillRoutine =
            StartCoroutine(FillSelection());
    }

    private void StopSelectionFillRoutine()
    {
        // stop filling it and reset its value.    
        if (m_SelectionFillRoutine != null)
        {
            StopCoroutine(m_SelectionFillRoutine);
        }
        ResetSelectionProgress();
    }

    private void ResetSelectionProgress()
    {
        m_SelectionFillRoutine = null;
        SetProgressableValueToMin();
    }

    private void RaiseOnSelectedEvent()
    {
        // If there is anything subscribed to OnSelectionComplete call it.
        if (OnSelectionComplete != null)
            OnSelectionComplete();
    }


    /* m_Progressable */

    private void SetProgressableValueToMin()
    {        
        m_Progressable.fillAmount = 0f;
    }

    private void SetProgressableValueToMax()
    {
        m_Progressable.fillAmount = 1f;
    }

    private void SetProgressableValue(float value)
    {
        m_Progressable.fillAmount = value;
    }

    /* end of m_Progressable */


    /* Fader */

    public void FadeInProgressable()
    {
        StartCoroutine(m_ProgressableFader.InterruptAndFadeIn());        
    }

    public void FadeOutProgressable()
    {        
        StartCoroutine(m_ProgressableFader.InterruptAndFadeOut());
    }

    /* end of Fader */


    /* IHandleUiButton interfaces */

    private void HandleDown()
    {     
        m_ButtonPressed = true;
        StartSelectionFillRoutine();
    }

    private void HandleEnter()
    {
        m_GazeOver = true;
        //PlayOnOverClip();

        FadeInProgressable();
    }

    private void HandleExit()
    {
        m_GazeOver = false;
        StopSelectionFillRoutine();

        FadeOutProgressable();
    }

    private void HandleUp()
    {        
        m_ButtonPressed = false;
        StopSelectionFillRoutine();
    }

    /* end of IHandleUiButton interfaces */
}
