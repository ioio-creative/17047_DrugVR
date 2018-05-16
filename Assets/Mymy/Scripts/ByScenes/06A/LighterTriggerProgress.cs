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
        m_HeadTransform = GameManager.Instance.HeadObject.transform;
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
        Vector3 lighterPos = m_Lighter.transform.position;

        // forward direction points from head to this transform
        Vector3 forwardVec = lighterPos - m_HeadTransform.position;

        Debug.DrawRay(lighterPos, StaticUp, Color.green);
        Debug.DrawRay(lighterPos, StaticForward, Color.yellow);
        Debug.DrawRay(lighterPos, StaticRight, Color.red);

        float newAzimuth = 0f;

        // points from head to this transform
        CalculateAzimuthAndZenithFromPointerDirection(forwardVec,
            Color.blue, Color.black,
            ref newAzimuth, ref m_Zenith);

        // Debug.Log(newAzimuth);
        if (IsLighterWithinTargetZone(newAzimuth, lighterPos))
        {
            // make progress bar always point to head
            //m_ProgressableTransform.rotation =
            //     Quaternion.LookRotation(forwardVec);

            if (!m_GazeOver)
            {
                HandleEnter();
            }
            else
            {
#if UNITY_EDITOR
                m_IsTriggerPress = Input.GetMouseButton(m_EditorUseMouseButton);
#else
                m_IsTriggerPress = m_WaveVrDevice.GetPress(m_InputToListen);        
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


    /* angle calculations */

    private void CalculateAzimuthAndZenithFromPointerDirection(Vector3 pointerDirection,
        Color debugRayColorForPointer, Color debugRayColorForPointerProjectionOnFloor,
        ref float signedAzimuth, ref float unsignedZenith)
    {
        AngleCalculations.CalculateAzimuthAndZenithFromPointerDirection(pointerDirection,
            StaticUp, StaticForward,
            m_Lighter.transform.position,
            debugRayColorForPointer, debugRayColorForPointerProjectionOnFloor,
            ref signedAzimuth, ref unsignedZenith);
    }

    /* end of angle calculations */


    /* audios */

    public void PlayOnFilledClip()
    {
        PlayAudioClip(m_OnFilledClip);
    }

    public void PlayAudioClip(AudioClip aClip)
    {
        m_Audio.clip = aClip;
        if (m_Audio.clip != null)
        {
            m_Audio.Play();
        }
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

        if (!m_SelectionFilled)
        {
            RaiseOnSelectedEvent();
            // The selection is now filled so the coroutine waiting for it can continue.
            m_SelectionFilled = true;
        }
        

               
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
        // Play the clip for when the selection is filled.        
        PlayOnFilledClip();

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

    public void InterruptAndFadeIn()
    {
        if (!m_ProgressableFader.Visible)
        {
            StartCoroutine(m_ProgressableFader.InterruptAndFadeIn()); 
        }        
    }

    public void InterruptAndFadeOut()
    {
        if (m_ProgressableFader.Visible)
        {
            StartCoroutine(m_ProgressableFader.InterruptAndFadeOut());
            m_GazeOver = false;

            //!! DO NOT CALL HandleExit() HERE!!//
            //this IS CALLED BY HandleExit()///
        }
        
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

        InterruptAndFadeIn();
    }

    private void HandleExit()
    {
        m_GazeOver = false;
        StopSelectionFillRoutine();

        InterruptAndFadeOut();
    }

    private void HandleUp()
    {        
        m_ButtonPressed = false;
        StopSelectionFillRoutine();
    }

    /* end of IHandleUiButton interfaces */
}
