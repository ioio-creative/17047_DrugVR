// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using System.Collections;
using wvr;

public class ChooseDoF : MonoBehaviour {
    public enum TrackingSpace {
        TS_NO_SPECIFY = -1,
        TS_3DOF,
        TS_6DOF_Ground,
        TS_6DOF_Head
    };

    public static TrackingSpace whichHead = TrackingSpace.TS_NO_SPECIFY;
    public TrackingSpace WhichHead = TrackingSpace.TS_3DOF;

    void OnEnable() {
        method1();
    }

    void method1() {
        // Global find
        GameObject body3DOF = transform.root.Find("Body3DOF").gameObject;
        GameObject body6DOF = transform.root.Find("Body6DOF").gameObject;
        GameObject followHMDPosition = transform.root.Find("FollowHMDPosition").gameObject;
        GameObject followHMDRotation = transform.root.Find("FollowHMDRotation").gameObject;

        // Children find
        //GameObject body1 = transform.Find("Body1").gameObject;
        if (whichHead == TrackingSpace.TS_NO_SPECIFY)
            whichHead = WhichHead;
        switch (whichHead)
        {
            case TrackingSpace.TS_3DOF:
                WaveVR_Render.globalOrigin = (int)WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead_3DoF;
                followHMDPosition.GetComponentInChildren<WaveVR_DevicePoseTracker>().trackPosition = false;
                followHMDRotation.GetComponentInChildren<WaveVR_DevicePoseTracker>().trackPosition = false;
                body3DOF.SetActive(true);
                break;
            case TrackingSpace.TS_6DOF_Ground:
                WaveVR_Render.globalOrigin = (int)WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround;
                body6DOF.SetActive(true);
                break;
            case TrackingSpace.TS_6DOF_Head:
                WaveVR_Render.globalOrigin = (int)WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;
                body6DOF.SetActive(true);
                break;
            case TrackingSpace.TS_NO_SPECIFY:
                Application.Quit();
                break;
        }
    }
}
