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

public class Watcher : MonoBehaviour
{
    private static string LOG_TAG = "Watcher";
    public GameObject WatchingObject;

    void OnEnable()
    {
        Log.i (LOG_TAG, "OnEnable");
        WatchingObject.SetActive (false);
        WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.DEVICE_CONNECTED, OnDeviceConnected);
    }

    void OnDisable()
    {
        WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.DEVICE_CONNECTED, OnDeviceConnected);
    }

    private void OnDeviceConnected(params object[] args)
    {
        var device = (WVR_DeviceType)args[0];
        var connected = (bool)args[1];
        Log.i (LOG_TAG, "device " + device + " is " + (connected == true ? "connected" : "disconnected"));

        if (connected)
        {
            Log.i (LOG_TAG, "Enable " + WatchingObject.name);
            WatchingObject.SetActive (true);
        }
    }
}
