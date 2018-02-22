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

public class ControllerConnectionStateReactor : MonoBehaviour
{
    private static string LOG_TAG = "WVRConnReactor";
    public WVR_DeviceType type;
    private bool connected = false;
    public List<GameObject> targetGameObjects = new List<GameObject>();
    private bool mFocusCapturedBySystem = false;

    void OnEnable()
    {
        if (!Application.isEditor)
        {
            WaveVR_Utils.Event.Listen (WaveVR_Utils.Event.DEVICE_CONNECTED, onDeviceConnected);
            if (checkConnection () != connected)
                connected = !connected;
            setActive (connected && (!mFocusCapturedBySystem));
        }
    }

    void OnDisable()
    {
        if (!Application.isEditor)
        {
            WaveVR_Utils.Event.Remove (WaveVR_Utils.Event.DEVICE_CONNECTED, onDeviceConnected);
        }
    }

    void Update ()
    {
        if (!connected) return;

        bool focusCaptured = Interop.WVR_IsInputFocusCapturedBySystem();

        if (focusCaptured != mFocusCapturedBySystem)
        {
            // InputFocus changed!
            mFocusCapturedBySystem = focusCaptured;

            Log.i(LOG_TAG, "device " + type + " is " + (mFocusCapturedBySystem == true ? " captured by system" : " not captured"));

            setActive(!mFocusCapturedBySystem);
        }
    }

    private bool checkConnection()
    {
        var wvr = WaveVR.Instance;
        if (wvr != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (WaveVR.DeviceTypes [i] == type)
                {
                    return wvr.connected [i];
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    private void setActive(bool active)
    {
        foreach (var go in targetGameObjects)
        {
            if (go != null) go.SetActive(active);
        }
    }

    private void onDeviceConnected(params object[] args)
    {
        WVR_DeviceType type = this.type;
        if (WaveVR_Controller.IsLeftHanded)
        {
            switch (type)
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

        var _dt = (WVR_DeviceType)args[0];
        var _connected = (bool)args[1];
        Log.i (LOG_TAG, "device " + _dt + " is " + (_connected == true ? "connected" : "disconnected"));

        if (type == _dt)
        {
            if (connected != _connected)
            {
                connected = _connected;
                setActive (connected && (!mFocusCapturedBySystem));
            }
        }
    }
}
