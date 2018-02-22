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
using wvr;

/// <summary>
/// This class is mainly for Device Tracking.
/// Tracking object communicates with HMD device or controller device in order to:
/// update new position and rotation for rendering
/// </summary>
public sealed class WaveVR_DevicePoseTracker : MonoBehaviour
{
    /// <summary>
    /// The type of this controller device, it should be unique.
    /// </summary>
    public WVR_DeviceType type;
    public bool inversePosition = false;
    public bool trackPosition = true;
    public bool inverseRotation = false;
    public bool trackRotation = true;

    public enum TrackingEvent {
        WhenUpdate,  // Pose will delay one frame.
        WhenNewPoses
    };

    public TrackingEvent timing = TrackingEvent.WhenNewPoses;

    private WVR_DevicePosePair_t wvr_pose = new WVR_DevicePosePair_t ();
    private WaveVR_Utils.RigidTransform rigid_pose = WaveVR_Utils.RigidTransform.identity;

    void Update()
    {
        if (timing == TrackingEvent.WhenNewPoses)
            return;
        if (WaveVR.Instance == null)
            return;

        WVR_DeviceType type = this.type;

        if (WaveVR_Controller.IsLeftHanded)
        {
            switch (this.type)
            {
            case WVR_DeviceType.WVR_DeviceType_Controller_Right:
                type = WVR_DeviceType.WVR_DeviceType_Controller_Left;
                break;
            case WVR_DeviceType.WVR_DeviceType_Controller_Left:
                type = WVR_DeviceType.WVR_DeviceType_Controller_Right;
                break;
            default:
                break;
            }
        }

        WaveVR.Device device = WaveVR.Instance.getDeviceByType (type);
        if (device.connected)
        {
            updatePose (device.pose, device.rigidTransform);
        }
    }


    /// if device connected, get new pose, then update new position and rotation of transform
    private void OnNewPoses(params object[] args)
    {
        WVR_DeviceType type = this.type;
        if (WaveVR_Controller.IsLeftHanded)
        {
            switch (this.type)
            {
            case WVR_DeviceType.WVR_DeviceType_Controller_Right:
                type = WVR_DeviceType.WVR_DeviceType_Controller_Left;
                break;
            case WVR_DeviceType.WVR_DeviceType_Controller_Left:
                type = WVR_DeviceType.WVR_DeviceType_Controller_Right;
                break;
            default:
                break;
            }
        }

        #if UNITY_EDITOR
        if (Application.isEditor)
        {
            var system = WaveVR_PoseSimulator.Instance;
            system.GetTransform (type, ref wvr_pose, ref rigid_pose);
            updatePose (wvr_pose, rigid_pose);
        } else
        #endif
        {
            var poses = (WVR_DevicePosePair_t[])args [0];
            var rtPoses = (WaveVR_Utils.RigidTransform[])args [1];

            for (int i = 0; i < poses.Length; i++)
            {
                if (poses [i].type == type && poses [i].pose.IsValidPose)
                {
                    updatePose (poses [i], rtPoses [i]);
                    break;
                }
            }
        }
    }

    void updatePose(WVR_DevicePosePair_t pose, WaveVR_Utils.RigidTransform rtPose)
    {
        if (trackPosition)
        {
            if (inversePosition)
                transform.localPosition = -rtPose.pos;
            else
            {
                transform.localPosition = rtPose.pos;
            }
        }
        if (trackRotation)
        {
            if (inverseRotation)
                transform.localRotation = Quaternion.Inverse(rtPose.rot);
            else
                transform.localRotation = rtPose.rot;
        }
    }

    void OnEnable()
    {
        if (timing == TrackingEvent.WhenNewPoses)
            WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.NEW_POSES, OnNewPoses);
    }

    void OnDisable()
    {
        if (timing == TrackingEvent.WhenNewPoses)
            WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.NEW_POSES, OnNewPoses);
    }
}

