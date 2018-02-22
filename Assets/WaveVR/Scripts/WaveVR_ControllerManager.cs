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

public class WaveVR_ControllerManager : MonoBehaviour
{
    private static string LOG_TAG = "WaveVR_ControllerManager";

    public GameObject right, left;

    public enum CIndex
    {
        invalid = -1,
        right = 0,
        left = 1
    }
    private GameObject[] ControllerObjects; // populate with objects you want to assign to additional controllers
    private bool[] ControllerConnected = new bool[2]{false, false};

    #region Override functions
    void Awake()
    {
        var objects = new GameObject[2];
        objects [(uint)CIndex.right] = right;
        objects [(uint)CIndex.left] = left;

        this.ControllerObjects = objects;
    }

    void OnEnable()
    {
        for (int i = 0; i < ControllerObjects.Length; i++)
        {
            var obj = ControllerObjects[i];
            if (obj != null)
            {
                Log.i (LOG_TAG, "OnEnable, disable controller " + i);
                obj.SetActive (false);
            }
        }

        for (int i = 0; i < WaveVR.DeviceTypes.Length; i++)
        {
            if (WaveVR.Instance.connected [i])
            {
                Log.i (LOG_TAG, "OnEnable, device " + WaveVR.DeviceTypes[i] + " is connected.");
                OnDeviceConnected (WaveVR.DeviceTypes [i], true);
            }
        }

        WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.DEVICE_CONNECTED, OnDeviceConnected);
    }

    void OnDisable()
    {
        WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.DEVICE_CONNECTED, OnDeviceConnected);
    }
    #endregion

    private void BroadcastToObjects(CIndex _index)
    {
        WVR_DeviceType deviceIndex = WVR_DeviceType.WVR_DeviceType_Controller_Right;

        var obj = ControllerObjects [(uint)_index];
        if (obj != null)
        {
            if (ControllerConnected [(uint)_index] == false)
            {
                obj.SetActive (false);
            } else
            {   // means object with _index is not null and connected.
                obj.SetActive(true);
                deviceIndex = _index == CIndex.right ?
                    WVR_DeviceType.WVR_DeviceType_Controller_Right :
                    WVR_DeviceType.WVR_DeviceType_Controller_Left; 

                obj.BroadcastMessage("SetDeviceIndex", deviceIndex, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void OnDeviceConnected(params object[] args)
    {
        var device = (WVR_DeviceType)args[0];
        var connected = (bool)args[1];
        Log.i (LOG_TAG, "device " + device + " is " + (connected == true ? "connected" : "disconnected"));

        if (device == WVR_DeviceType.WVR_DeviceType_Controller_Right)
        {
            if (ControllerConnected [(uint)CIndex.right] != connected)
            {   // Connection status has been changed.
                ControllerConnected [(uint)CIndex.right] = connected;
                BroadcastToObjects (CIndex.right);
            }
        } else if (device == WVR_DeviceType.WVR_DeviceType_Controller_Left)
        {
            if (ControllerConnected [(uint)CIndex.left] != connected)
            {   // Connection status has been changed.
                ControllerConnected [(uint)CIndex.left] = connected;
                BroadcastToObjects (CIndex.left);
            }
        }
    }

}
