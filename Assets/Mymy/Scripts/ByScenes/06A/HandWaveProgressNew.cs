using System;
using UnityEngine;
using VRStandardAssets.Utils;

public class HandWaveProgressNew : MonoBehaviour
{
    /* progress */

    public event Action OnSelectionComplete;
    public event Action OnSelectionFadeOutComplete;

    [SerializeField]
    private HandWaveProgressable m_ProgressBar;

    [SerializeField]
    private AudioSource m_Audio;
    [SerializeField]
    private AudioClip m_OnSelectedClip;
    /* end of progress */


    /* for tracking transform angles */
    //!! CANNOT USE STATIC FIELD, won't reset after class instance destroyed !!//
    private Vector3 SceneUp = Vector3.up;
    private Vector3 SceneForward = Vector3.forward;
    private Vector3 SceneRight = Vector3.right;
    
    [SerializeField]
    private float m_StaticForwardOffset;
    [SerializeField]
    private float m_HeadTargetHorizontalHalfAngleRange;
    [SerializeField]
    private float m_HeadTargetVerticalHalfAngleRange;
    [SerializeField]
    private float m_HandTargetVerticalUpperHalfAngleRange;
    [SerializeField]
    private float m_HandTargetVerticalLowerHalfAngleRange;

    // m_Zenith & m_Azimuth are just for display, not to be set in Editor
    [SerializeField]
    private float m_HandZenith = 0f;
    [SerializeField]
    private float m_HandAzimuth = 0f;
    [SerializeField]
    private float m_HeadZenith = 0f;
    [SerializeField]
    private float m_HeadAzimuth = 0f;

    [SerializeField]
    private UIFader m_HandWaveProgressFader;
    [SerializeField]
    private Transform m_HandImageTransform;    
    private Transform m_FocusControllerTransform;

    private Transform m_HeadTransform;

    /* end of for tracking transform angles */


    /* MonoBehaviour */

    private void Awake()
    {
        m_HeadTransform = GameManager.Instance.HeadObject.transform;
    }

    private void OnEnable()
    {
        m_ProgressBar.OnProgressComplete += HandleProgressBarProgressComplete;
    }

    private void OnDisable()
    {
        m_ProgressBar.OnProgressComplete -= HandleProgressBarProgressComplete;
    }

    private void Start()
    {
        GameObject focusControllerObject = GameManager.Instance.FocusControllerObject;
        m_FocusControllerTransform = focusControllerObject.transform;        

        // offset scene rotation + a fixed offset
        float sceneRotation = GameManager.Instance.CurrentSceneScroll.SkyShaderDefaultRotation;
        Quaternion rotationAlongY = Quaternion.Euler(0, sceneRotation + m_StaticForwardOffset, 0);
        SceneForward = rotationAlongY * SceneForward;
        SceneRight = rotationAlongY * SceneRight;
    }

    private void Update()
    {        
        Debug.DrawRay(transform.position, SceneUp, Color.green);
        Debug.DrawRay(transform.position, SceneForward, Color.yellow);
        Debug.DrawRay(transform.position, SceneRight, Color.red);

        // hand
        // We use the Head's forward vector to Calculate central axis for hand waving
        float newHandAzimuth = 0f;
        Vector3 handwaveForward = Vector3.ProjectOnPlane(m_HeadTransform.forward, SceneUp);
        //CalculateAzimuthAndZenithFromPointerDirection(m_FocusControllerTransform.forward,
        //    Color.blue, Color.black,
        //    ref newHandAzimuth, ref m_HandZenith);
        AngleCalculations.CalculateAzimuthAndZenithFromPointerDirection(m_FocusControllerTransform.forward,
            SceneUp, handwaveForward,
            transform.position,
            Color.blue, Color.black,
            ref newHandAzimuth, ref m_HandZenith);

        // head
        // We use Scene Forward (the man's position in scene) to calculate central axis allowed for HandWaveProgress
        CalculateAzimuthAndZenithFromPointerDirection(m_HeadTransform.forward,
            Color.cyan, Color.gray,
            ref m_HeadAzimuth, ref m_HeadZenith);

        if (IsHeadWithinTargetZone(m_HeadAzimuth, m_HeadZenith) && IsHandWithinTargetZone(m_HandZenith))
        {
            //Debug.Log("head in zone");
            CheckAndFadeIn();
            newHandAzimuth = (Mathf.Abs(newHandAzimuth) < 90 ? Mathf.Abs(newHandAzimuth) : 180 - Mathf.Abs(newHandAzimuth)) * Mathf.Sign(newHandAzimuth);
            m_HandImageTransform.localRotation = Quaternion.Euler(0, 0, newHandAzimuth);

            // if azimuth changes sign
            if (m_HandAzimuth * newHandAzimuth < 0)
            {
                m_ProgressBar.StepIt();
            }
        }
        else
        {
            //Debug.Log("head not in zone");
            CheckAndFadeOutAndReset();
        }

        m_HandAzimuth = newHandAzimuth;
    }

    /* end of MonoBehaviour */


    /* angle calculations */

    private void CalculateAzimuthAndZenithFromPointerDirection(Vector3 pointerDirection,        
        Color debugRayColorForPointer, Color debugRayColorForPointerProjectionOnFloor,
        ref float signedAzimuth, ref float unsignedZenith)
    {
        AngleCalculations.CalculateAzimuthAndZenithFromPointerDirection(pointerDirection,
            SceneUp, SceneForward,
            transform.position,
            debugRayColorForPointer, debugRayColorForPointerProjectionOnFloor,
            ref signedAzimuth, ref unsignedZenith);        
    }
    
    private bool IsHeadWithinTargetZone(float azimuth, float zenith)
    {
        //Debug.Log(zenith);
        return Mathf.Abs(azimuth) <= m_HeadTargetHorizontalHalfAngleRange
            && Mathf.Abs(zenith - 90) <= m_HeadTargetVerticalHalfAngleRange;
    }

    private bool IsHandWithinTargetZone(float zenith)
    {
        return (zenith - 90f) < 0f ? 
                Mathf.Abs(zenith - 90f) <= m_HandTargetVerticalUpperHalfAngleRange
                : Mathf.Abs(zenith - 90f) <= m_HandTargetVerticalLowerHalfAngleRange;
    }

    /* end of angle calculations */


    /* Fader */
    
    private void CheckAndFadeIn()
    {
        if (m_HandWaveProgressFader.Fading == m_HandWaveProgressFader.Visible)
        {            
            StartCoroutine(m_HandWaveProgressFader.InterruptAndFadeIn());
        }        
    }

    //TODO::Revise Condition
    private void CheckAndFadeOutAndReset()
    {
        if (m_HandWaveProgressFader.Visible)
        {
            StartCoroutine(m_HandWaveProgressFader.CheckAndFadeOut());
            m_ProgressBar.Reset();
        }
        
    }

    // HandLigherSwitchControl can control fade out
    //TODO::Revise Condition
    public void InterruptAndFadeOutAndReset()
    {
        if (m_HandWaveProgressFader.Fading || m_HandWaveProgressFader.Visible)
        {
            StartCoroutine(m_HandWaveProgressFader.InterruptAndFadeOut());
            m_ProgressBar.Reset();
        }
        
    }

    /* end of Fader */


    /* event handlers */

    private void HandleProgressBarProgressComplete()
    {
        m_Audio.clip = m_OnSelectedClip;
        m_Audio.Play();

        m_HandWaveProgressFader.OnFadeOutComplete += HandleHandWaveProgressFadeOutComplete;
        InterruptAndFadeOutAndReset();
        if (OnSelectionComplete != null)
        {
            OnSelectionComplete();
        }
    }

    private void HandleHandWaveProgressFadeOutComplete()
    {
        if (OnSelectionFadeOutComplete != null)
        {
            OnSelectionFadeOutComplete();
        }
        m_HandWaveProgressFader.OnFadeOutComplete -= HandleHandWaveProgressFadeOutComplete;
    }
    /* end of event handlers */
}
