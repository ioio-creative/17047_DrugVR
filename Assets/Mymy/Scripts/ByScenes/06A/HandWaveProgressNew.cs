using System;
using UnityEngine;
using VRStandardAssets.Utils;

public class HandWaveProgressNew : MonoBehaviour
{
    /* progress */

    public event Action OnSelectionComplete;

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
    
    [SerializeField]
    private float m_StaticForwardOffset;
    [SerializeField]
    private float m_Zenith = 0f;
    [SerializeField]
    private float m_Azimuth = 0f;

    [SerializeField]
    private UIFader m_HandWaveProgressFader;
    [SerializeField]
    private Transform m_HandImageTransform;    
    private Transform m_FocusControllerTransform;

    private bool m_IsFirstTimeEnable = true;

    /* end of for tracking transform angles */


    /* MonoBehaviour */

    private void OnEnable()
    {
        m_ProgressBar.OnProgressComplete += HandleProgressBarProgressComplete;

        if (!m_IsFirstTimeEnable)
        {
            StartCoroutine(m_HandWaveProgressFader.InterruptAndFadeIn());
        }
        else
        {
            m_IsFirstTimeEnable = false;
        }
    }

    private void OnDisable()
    {
        m_ProgressBar.OnProgressComplete -= HandleProgressBarProgressComplete;
        
        StartCoroutine(m_HandWaveProgressFader.InterruptAndFadeOut());        
        m_ProgressBar.Reset();
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

    /* end of MonoBehaviour */


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
