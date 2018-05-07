using UnityEngine;
using VRStandardAssets.Utils;
using wvr;

public class HandWaveProgressNew : MonoBehaviour
{
    /* progress */

    [SerializeField]
    private WVR_DeviceType m_DeviceToListen = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    [SerializeField]
    private WVR_InputId m_InputToListen = WVR_InputId.WVR_InputId_Alias1_Touchpad;

    [SerializeField]
    private HandWaveProgressable m_ProgressBar;
    [SerializeField]
    private int m_NumOfHandWaveStrokes = 0;

    /* end of progress */


    /* for tracking transform angles */

    private const string FocusControllerObjectName = "/VIVEFocusWaveVR/FocusController";    

    private static Vector3 StaticUp = Vector3.up;
    private static Vector3 StaticForward = Vector3.forward;
    private static Vector3 StaticRight = Vector3.right;

    // TODO: need to offset scene rotation
    [SerializeField]
    private float m_StaticForwardOffset = -52.5f;
    [SerializeField]
    private float m_Zenith = 0f;
    [SerializeField]
    private float m_Azimuth = 0f;

    [SerializeField]
    private UIFader m_HandWaveProgressFader;
    private Coroutine m_HandWaveProgressFadeInRoutine = null;
    private Coroutine m_HandWaveProgressFadeOutRoutine = null;
    [SerializeField]
    private Transform m_HandImageTransform;    
    private Transform m_FocusControllerTransform;
    private WaveVR_ControllerPoseTracker m_ControllerPT;
    [SerializeField]
    private AddLighterToFocusController m_AddLighterComponent;

    /* end of for tracking transform angles */


    /* MonoBehaviour */

    private void Start()
    {
        GameObject focusControllerObject = GameObject.Find(FocusControllerObjectName);
        m_FocusControllerTransform = focusControllerObject.transform;
        m_ControllerPT = focusControllerObject.GetComponent<WaveVR_ControllerPoseTracker>();

        Quaternion rotationAlongY = Quaternion.Euler(0, m_StaticForwardOffset, 0);
        StaticForward = rotationAlongY * StaticForward;
        StaticRight = rotationAlongY * StaticRight;
    }

    private void Update()
    {        
        Debug.DrawRay(transform.position, StaticUp, Color.green);
        Debug.DrawRay(transform.position, StaticForward, Color.yellow);
        Debug.DrawRay(transform.position, StaticRight, Color.red);
        
        bool isTriggerBtnPressed = WaveVR_Controller.Input(m_DeviceToListen).GetPress(m_InputToListen);

        if (isTriggerBtnPressed)
        {
            m_AddLighterComponent.ReplaceLighterByController();
            m_HandWaveProgressFadeOutRoutine = null;
            if (m_HandWaveProgressFadeInRoutine == null)
            {
                m_HandWaveProgressFadeInRoutine = StartCoroutine(m_HandWaveProgressFader.CheckAndFadeIn());
            }

            Vector3 forwardVec = m_FocusControllerTransform.forward;
            Vector3 normedProjectionOnFloor = Vector3.Normalize(forwardVec - new Vector3(0, forwardVec.y, 0));
            m_Zenith = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(forwardVec, StaticUp));

            Debug.DrawRay(transform.position, forwardVec, Color.blue);
            Debug.DrawRay(transform.position, normedProjectionOnFloor, Color.black);

            float signedMagnitudeOfSineAzimuth = Vector3.Dot(Vector3.Cross(StaticForward, normedProjectionOnFloor), StaticUp);
            float newAzimuth = Mathf.Rad2Deg * Mathf.Asin(signedMagnitudeOfSineAzimuth);
            
            m_HandImageTransform.localRotation = Quaternion.Euler(0, 0, newAzimuth);

            // if azimuth changes sign
            if (m_Azimuth * newAzimuth < 0)
            {
                m_NumOfHandWaveStrokes++;
                m_ProgressBar.StepIt();
            }

            m_Azimuth = newAzimuth;
        }
        else
        {
            m_AddLighterComponent.ReplaceControllerByLighter();           
            m_HandWaveProgressFadeInRoutine = null;
            if (m_HandWaveProgressFadeOutRoutine == null)
            {
                m_HandWaveProgressFadeOutRoutine = StartCoroutine(m_HandWaveProgressFader.CheckAndFadeOut());
            }
        }
    }

    /* end of MonoBehaviour */
}
