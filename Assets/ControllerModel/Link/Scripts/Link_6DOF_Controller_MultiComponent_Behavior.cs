// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;

public class Link_6DOF_Controller_MultiComponent_Behavior : MonoBehaviour {
    private static string LOG_TAG = "Link_6DOF_CTR_Behavior";

    public WVR_DeviceType device = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    public GameObject TriggerButtonPress_Effect = null;
    public GameObject MenuButtonPress_Effect = null;
    public GameObject Touch_Effect = null;
    public GameObject Battery_Effect = null;
    public Texture[] textures = new Texture[6];
    private bool getValidBattery = false;

    [Range(0, 1.0f)]
    public float[] percents = new float[6];

    private MeshRenderer batteryMeshRenderer = null;

    private Vector3 originPosition;

    void OnEnable()
    {
        WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.BATTERY_STATUS_UPDATE, onBatteryStatusUpdate);
    }

    void OnDisable()
    {
        WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.BATTERY_STATUS_UPDATE, onBatteryStatusUpdate);
    }

    private void onBatteryStatusUpdate(params object[] args)
    {
        Log.d(LOG_TAG, "receive battery status update event");
        if (Battery_Effect != null)
        {
            getValidBattery = updateBatteryInfo();
        }
    }

    // Use this for initialization
    void Start () {
        if (TriggerButtonPress_Effect != null)
        {
            TriggerButtonPress_Effect.SetActive(false);
        }
        if (MenuButtonPress_Effect != null)
        {
            MenuButtonPress_Effect.SetActive(false);
        }
        if (Touch_Effect != null)
        {
            originPosition = Touch_Effect.transform.localPosition;
            Touch_Effect.SetActive(false);
        }
        if (Battery_Effect != null)
        {
            batteryMeshRenderer = Battery_Effect.GetComponent<MeshRenderer>();

            Battery_Effect.SetActive(false);
        }
    }
    int t = 0;

    // Update is called once per frame
    void Update () {
        if (Battery_Effect != null)
        {
            if (!getValidBattery)
            {
                if (t++ > 300)
                {
                    getValidBattery = updateBatteryInfo();

                    t = 0;
                }
            }
        }

        //WVR_InputId_Alias1_Menu
        if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Menu))
        {
            if (MenuButtonPress_Effect != null)
            {
                MenuButtonPress_Effect.SetActive(true);
            }
        }
        // button pressed
        if (WaveVR_Controller.Input(device).GetPress(WVR_InputId.WVR_InputId_Alias1_Menu))
        {
            if (MenuButtonPress_Effect != null)
            {
                MenuButtonPress_Effect.SetActive(true);
            }
        }
        if (WaveVR_Controller.Input(device).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Menu))
        {
            if (MenuButtonPress_Effect != null)
            {
                MenuButtonPress_Effect.SetActive(false);
            }
        }

        //WVR_InputId_Alias1_Touchpad
        if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            if (Touch_Effect != null)
            {
                Touch_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPress(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            if (Touch_Effect != null)
            {
                Touch_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            if (Touch_Effect != null)
            {
                Touch_Effect.SetActive(false);
            }
        }
        // button touch down
        if (WaveVR_Controller.Input(device).GetTouchDown(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            if (Touch_Effect != null)
            {
                Touch_Effect.SetActive(true);
            }
        }

        // button touch up
        if (WaveVR_Controller.Input(device).GetTouchUp(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            if (Touch_Effect != null)
            {
                Touch_Effect.SetActive(false);
            }
        }
        // button touched
        if (WaveVR_Controller.Input(device).GetTouch(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            var axis = WaveVR_Controller.Input(device).GetAxis(WVR_InputId.WVR_InputId_Alias1_Touchpad);

            float xangle = axis.x/100, yangle = axis.y/100;
            Log.d(LOG_TAG, "WVR_InputId_Alias1_Touchpad axis xangle: " + xangle + ", yangle: " + yangle);
            if (xangle > 0.006f) xangle = 0.006f;
            if (xangle < -0.009f) xangle = -0.009f;

            if (yangle > 0.006f) yangle = 0.006f;
            if (yangle < -0.009f) yangle = -0.009f;

            if (Touch_Effect != null)
            {
                var translateVec = new Vector3(xangle, yangle, 0);
                Touch_Effect.transform.localPosition = originPosition + translateVec;
           }
        }

        //WVR_InputId_Alias1_Grip
        if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            if (TriggerButtonPress_Effect != null)
            {
                TriggerButtonPress_Effect.SetActive(true);
            }
        }

        // button pressed
        if (WaveVR_Controller.Input(device).GetPress(WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            if (TriggerButtonPress_Effect != null)
            {
                TriggerButtonPress_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            if (TriggerButtonPress_Effect != null)
            {
                TriggerButtonPress_Effect.SetActive(false);
            }
        }

        //WVR_InputId_Alias1_Trigger
        if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            if (TriggerButtonPress_Effect != null)
            {
                TriggerButtonPress_Effect.SetActive(true);
            }
        }
        // button pressed
        if (WaveVR_Controller.Input(device).GetPress(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            if (TriggerButtonPress_Effect != null)
            {
                TriggerButtonPress_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            if (TriggerButtonPress_Effect != null)
            {
                TriggerButtonPress_Effect.SetActive(false);
            }
        }

        // button touch down
        if (WaveVR_Controller.Input(device).GetTouchDown(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            // do nothing
        }

        // button touch up
        if (WaveVR_Controller.Input(device).GetTouchUp(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            // do nothing
        }
        // button touched
        if (WaveVR_Controller.Input(device).GetTouch(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            // do nothing
        }
    }

    private bool updateBatteryInfo()
    {
        if (Application.isEditor)
            return false;

        float batteryPer = Interop.WVR_GetDeviceBatteryPercentage(device);
        Log.d(LOG_TAG, "BatteryPercentage device: " + device + ", percentage: " + batteryPer);
        if (batteryPer < 0)
        {
            Log.d(LOG_TAG, "device: " + device + " BatteryPercentage is negative, return false");
            return false;
        }

        if (batteryPer > percents[0])
        {
            batteryMeshRenderer.material.mainTexture = textures[0];
        }
        else if (batteryPer > percents[1])
        {
            batteryMeshRenderer.material.mainTexture = textures[1];
        }
        else if (batteryPer > percents[2])
        {
            batteryMeshRenderer.material.mainTexture = textures[2];
        }
        else if (batteryPer > percents[3])
        {
            batteryMeshRenderer.material.mainTexture = textures[3];
        }
        else if (batteryPer > percents[4])
        {
            batteryMeshRenderer.material.mainTexture = textures[4];
        }
        else
        {
            batteryMeshRenderer.material.mainTexture = textures[5];
        }
        Battery_Effect.SetActive(true);
        return true;
    }
}
