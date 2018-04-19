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
using System.Collections.Generic;
using wvr;
using System.Collections;
using WaveVR_Log;

public class ControllerTest : MonoBehaviour
{
    private static string LOG_TAG = "ControllerTest";
    public WVR_DeviceType device = WVR_DeviceType.WVR_DeviceType_HMD;
    public GameObject ControlledObject;

    WVR_InputId[] buttonIds = new WVR_InputId[] {
        WVR_InputId.WVR_InputId_Alias1_Menu,
        WVR_InputId.WVR_InputId_Alias1_Grip,
        WVR_InputId.WVR_InputId_Alias1_DPad_Left,
        WVR_InputId.WVR_InputId_Alias1_DPad_Up,
        WVR_InputId.WVR_InputId_Alias1_DPad_Right,
        WVR_InputId.WVR_InputId_Alias1_DPad_Down,
        WVR_InputId.WVR_InputId_Alias1_Volume_Up,
        WVR_InputId.WVR_InputId_Alias1_Volume_Down,
        WVR_InputId.WVR_InputId_Alias1_Touchpad,
        WVR_InputId.WVR_InputId_Alias1_Trigger,
        WVR_InputId.WVR_InputId_Alias1_Bumper,
        WVR_InputId.WVR_InputId_Alias1_System
    };

    WVR_InputId[] axisIds = new WVR_InputId[] {
        WVR_InputId.WVR_InputId_Alias1_Touchpad,
        WVR_InputId.WVR_InputId_Alias1_Trigger
    };

    public void updatePose(WaveVR_Utils.RigidTransform pose)
    {
        transform.localRotation = pose.rot;
    }

    void Update()
    {
        bool controller_rotates = true;

        for (uint i = 0; i < 2; i++)
        {
            foreach (WVR_InputId buttonId in buttonIds)
            {
                // button down
                if (WaveVR_Controller.Input (device).GetPressDown (buttonId))
                {
                    #if UNITY_EDITOR
                    Debug.Log (buttonId + " press down");
                    #endif
                    Log.d (LOG_TAG, "button " + buttonId + " press down");

                    ControlledObject.SetActive (false);
                }

                // button up
                if (WaveVR_Controller.Input (device).GetPressUp (buttonId))
                {
                    #if UNITY_EDITOR
                    Debug.Log (buttonId + " press up");
                    #endif
                    Log.d (LOG_TAG, "button " + buttonId + " press up");

                    ControlledObject.SetActive (true);

                    if (buttonId == WVR_InputId.WVR_InputId_Alias1_Trigger)
                    {
                        WaveVR_Controller.Input (device).TriggerHapticPulse ();
                    }
                }

                // button pressed
                if (WaveVR_Controller.Input (device).GetPress (buttonId))
                {
                    #if UNITY_EDITOR
                    Debug.Log (buttonId + " pressed.");
                    #endif
                    Log.d (LOG_TAG, "button " + buttonId + " pressed.");
                }
            }

            foreach (WVR_InputId axisId in axisIds)
            {
                // button touch down
                if (WaveVR_Controller.Input (device).GetTouchDown (axisId))
                {
                    #if UNITY_EDITOR
                    Debug.Log (axisId + " touch down");
                    #endif
                    Log.d (LOG_TAG, "button " + axisId + " touch down.");

                    Renderer rend = ControlledObject.GetComponent<Renderer> ();
                    if (null != rend)
                    {
                        rend.material.color = Color.green;
                    }
                }

                // button touch up
                if (WaveVR_Controller.Input (device).GetTouchUp (axisId))
                {
                    #if UNITY_EDITOR
                    Debug.Log (axisId + " touch up");
                    #endif
                    Log.d (LOG_TAG, "button " + axisId + " touch up.");

                    Renderer rend = ControlledObject.GetComponent<Renderer> ();
                    if (null != rend)
                    {
                        rend.material.color = Color.gray;
                    }
                }

                // button touched
                if (WaveVR_Controller.Input (device).GetTouch (axisId))
                {
                    var axis = WaveVR_Controller.Input (device).GetAxis (axisId);

                    #if UNITY_EDITOR
                    Debug.Log ("axis: " + axis);
                    #endif

                    float xangle = 360 * axis.x, yangle = 360 * axis.y;
                    Log.d (LOG_TAG, "button " + axisId + " axis xangle: " + xangle + ", yangle: " + yangle);
                    xangle = xangle > 0 ? xangle : -xangle;
                    yangle = yangle > 0 ? yangle : -yangle;

                    ControlledObject.transform.Rotate (xangle * (10 * Time.deltaTime), 0, 0);
                    ControlledObject.transform.Rotate (0, yangle * (10 * Time.deltaTime), 0);

                    controller_rotates = false;
                }
            }
        }

        if (controller_rotates)
            ControlledObject.transform.localRotation = WaveVR_Controller.Input (device).transform.rot;
    } // Update
}
