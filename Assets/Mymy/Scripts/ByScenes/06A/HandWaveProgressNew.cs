using System;
using UnityEngine;
using VRStandardAssets.Utils;

public class HandWaveProgressNew : MonoBehaviour
{
    /* progress */

    public event Action OnSelectionComplete;

    [SerializeField]
    private HandWaveProgressable m_ProgressBar;

    /* end of progress */


    /* for tracking transform angles */

    // TODO: There is same thing in LigherTriggerProgress.cs
    private const string HeadObjectName = "/VIVEFocusWaveVR/head";

    private const string FocusControllerObjectName = "/VIVEFocusWaveVR/FocusController";    

    private static Vector3 StaticUp = Vector3.up;
    private static Vector3 StaticForward = Vector3.forward;
    private static Vector3 StaticRight = Vector3.right;
    
    [SerializeField]
    private float m_StaticForwardOffset;
    [SerializeField]
    private float m_HeadTargetHorizontalHalfAngleRange;
    [SerializeField]
    private float m_HeadTargetVerticalHalfAngleRange;

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
        m_HeadTransform = GameObject.Find(HeadObjectName).transform;
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
        GameObject focusControllerObject = GameObject.Find(FocusControllerObjectName);
        m_FocusControllerTransform = focusControllerObject.transform;        

        // offset scene rotation + a fixed offset
        float sceneRotation = GameManager.Instance.CurrentSceneScroll.SkyShaderDefaultRotation;
        Quaternion rotationAlongY = Quaternion.Euler(0, sceneRotation + m_StaticForwardOffset, 0);
        StaticForward = rotationAlongY * StaticForward;
        StaticRight = rotationAlongY * StaticRight;
    }

    private void Update()
    {        
        Debug.DrawRay(transform.position, StaticUp, Color.green);
        Debug.DrawRay(transform.position, StaticForward, Color.yellow);
        Debug.DrawRay(transform.position, StaticRight, Color.red);

        Vector3 forwardVec = m_FocusControllerTransform.forward;

        // hand
        float newHandAzimuth = 0f;
        CalculateAzimuthAndZenithFromPointerDirection(m_FocusControllerTransform.forward,
            Color.blue, Color.black,
            ref newHandAzimuth, ref m_HandZenith);

        // head
        CalculateAzimuthAndZenithFromPointerDirection(m_HeadTransform.forward,
            Color.cyan, Color.gray,
            ref m_HeadAzimuth, ref m_HeadZenith);

        if (IsHeadWithinTargetZone(m_HeadAzimuth, m_HeadZenith))
        {
            Debug.Log("head in zone");
            CheckAndFadeIn();

            m_HandImageTransform.localRotation = Quaternion.Euler(0, 0, newHandAzimuth);

            // if azimuth changes sign
            if (m_HandAzimuth * newHandAzimuth < 0)
            {
                m_ProgressBar.StepIt();
            }
        }
        else
        {
            Debug.Log("head not in zone");
            CheckAndFadeOutAndReset();
        }

        m_HandAzimuth = newHandAzimuth;
    }

    /* end of MonoBehaviour */


    /* angle calculations */

    private void CalculateAzimuthAndZenithFromPointerDirection(Vector3 pointerDirection,
        Color debugRayColorForPointer, Color debugRayColorForPointerProjectionOnFloor,
        ref float azimuth, ref float zenith)
    {
        Vector3 normedProjectionOnFloor = Vector3.Normalize(pointerDirection - new Vector3(0, pointerDirection.y, 0));
        zenith = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(pointerDirection, StaticUp));

        Debug.DrawRay(transform.position, pointerDirection, debugRayColorForPointer);
        Debug.DrawRay(transform.position, normedProjectionOnFloor, debugRayColorForPointerProjectionOnFloor);

        float signedMagnitudeOfSineAzimuth = Vector3.Dot(Vector3.Cross(StaticForward, normedProjectionOnFloor), StaticUp);
        azimuth = Mathf.Rad2Deg * Mathf.Asin(signedMagnitudeOfSineAzimuth);
    }

    private bool IsHeadWithinTargetZone(float azimuth, float zenith)
    {
        return Mathf.Abs(azimuth) <= m_HeadTargetHorizontalHalfAngleRange
            && (90 - Mathf.Abs(zenith)) <= m_HeadTargetVerticalHalfAngleRange;
    }

    /* end of angle calculations */


    /* Fader */

    private void CheckAndFadeIn()
    {
        if (!m_HandWaveProgressFader.Visible)
        {            
            StartCoroutine(m_HandWaveProgressFader.CheckAndFadeIn());
        }
    }

    private void CheckAndFadeOutAndReset()
    {
        if (m_HandWaveProgressFader.Visible)
        {
            StartCoroutine(m_HandWaveProgressFader.CheckAndFadeOut());
        }
        m_ProgressBar.Reset();
    }

    // HandLigherSwitchControl can control fade out
    public void InterruptAndFadeOut()
    {
        if (m_HandWaveProgressFader.Visible)
        {
            StartCoroutine(m_HandWaveProgressFader.InterruptAndFadeOut());
        }
    }

    /* end of Fader */


    /* event handlers */

    private void HandleProgressBarProgressComplete()
    {
        if (OnSelectionComplete != null)
        {
            OnSelectionComplete();
        }            
    }

    /* end of event handlers */
}
