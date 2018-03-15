using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;

public class HandWaveProgress : MonoBehaviour
{
    private enum HandWaveState
    {
        WaitingRightToLeft,
        WaitingLeftToRight
    }

    private static Vector3 StaticUp = Vector3.up;
    private static Vector3 StaticForward = Vector3.forward;
    private static Vector3 StaticRight = Vector3.right;

    [SerializeField]
    private HandWaveProgressable m_ProgressBar;
    [SerializeField]
    private float m_Zenith = 0f;
    [SerializeField]
    private float m_Azimuth = 0f;
    [SerializeField]
    private int m_NumOfHandWaveStrokes = 0;
    [SerializeField]
    private WVR_DeviceType device = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    [SerializeField]
    private WVR_InputId inputToListen = WVR_InputId.WVR_InputId_16;


    private void Start()
    {
		
	}
		
	private void Update()
    {
        Vector3 forwardVec = transform.forward;
        m_Zenith = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(forwardVec, StaticUp));

        Vector3 normedProjectionOnFloor = Vector3.Normalize(transform.forward - new Vector3(0, transform.forward.y, 0));
        float signedMagnitudeOfSineAzimuth = Vector3.Dot(Vector3.Cross(StaticForward, normedProjectionOnFloor), StaticUp);
        float newAzimuth = Mathf.Rad2Deg * Mathf.Asin(signedMagnitudeOfSineAzimuth);

        Debug.DrawRay(transform.position, StaticUp, Color.green);
        Debug.DrawRay(transform.position, StaticForward, Color.yellow);
        Debug.DrawRay(transform.position, StaticRight, Color.red);

        Debug.DrawRay(transform.position, forwardVec, Color.blue);
        Debug.DrawRay(transform.position, normedProjectionOnFloor, Color.black);

        // if azimuth changes sign
        if (m_Azimuth * newAzimuth < 0)
        {
            m_NumOfHandWaveStrokes++;
            m_ProgressBar.StepIt();
        }

        m_Azimuth = newAzimuth;

        if (WaveVR_Controller.Input(device).GetPress(inputToListen))
        {
            m_ProgressBar.Reset();
        }
    }
}
