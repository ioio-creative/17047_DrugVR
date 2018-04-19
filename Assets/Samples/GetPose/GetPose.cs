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
using WaveVR_Log;

public class GetPose : MonoBehaviour
{
    private static string LOG_TAG = "GetPose";
	/// <summary>
	/// The index of this controller device, it should be unique.
	/// </summary>
	public WVR_DeviceType index;
    public WVR_PoseOriginModel origin = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround;

    private WVR_DevicePosePair_t[] poses = new WVR_DevicePosePair_t[3];  // HMD, R, L controllers.
    private WaveVR_Utils.RigidTransform[] rtPoses = new WaveVR_Utils.RigidTransform[3];

    // Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
	{
        #if UNITY_EDITOR
        if (Application.isEditor)
            return;
        #endif

        WaveVR.Device _device = WaveVR.Instance.getDeviceByType (this.index);
        WVR_DeviceType _type = _device.type;

        Interop.WVR_GetSyncPose(origin, poses, (uint) poses.Length);

        Log.d (LOG_TAG, "Update() poses length = " + poses.Length);
        for (int i = 0; i < poses.Length; i++)
        {
            Log.d (LOG_TAG, "Update() pose[" + i + "] type = " + poses [i].type
                + " pose is " + (poses [i].pose.IsValidPose ? "valid" : "invalid"));
            if (poses[i].type == _type && poses[i].pose.IsValidPose)
            {
                rtPoses [i].update (poses [i].pose.PoseMatrix);
                updatePose (rtPoses [i]);
                break;
            }
        }
	}

	public void updatePose(WaveVR_Utils.RigidTransform pose)
	{
		transform.localPosition = pose.pos;
		transform.localRotation = pose.rot;
	}
}
