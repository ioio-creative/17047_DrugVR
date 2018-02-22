using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;
using System;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

public class WaveVR_ControllerLoader : MonoBehaviour {
    private static string LOG_TAG = "WaveVR_ControllerLoader";
    public enum ControllerHand
    {
        Controller_Right,
        Controller_Left
    };

    public enum CComponent
    {
        One_Bone,
        Multi_Component
    };

    public enum CTrackingSpace
    {
        CTS_3DOF,
        CTS_6DOF,
        CTS_SYSTEM
    };

    public ControllerHand WhichHand = ControllerHand.Controller_Right;
    public CComponent ControllerComponents = CComponent.Multi_Component;
    public CTrackingSpace TrackingMethod = CTrackingSpace.CTS_SYSTEM;

    private GameObject controllerPrefab = null;
    private GameObject originalControllerPrefab = null;
    private string controllerFileName = "";
    private string controllerModelFoler = "Controller/";
    private string genericControllerFileName = "Generic_";

    private WVR_DeviceType deviceType = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    private bool connected = false;
#if UNITY_EDITOR
    public delegate void ControllerModelLoaded(GameObject go);
    public static event ControllerModelLoaded onControllerModelLoaded = null;
#endif

    void OnEnable()
    {
        controllerPrefab = null;
        controllerFileName = "";
        genericControllerFileName = "Generic_";
        if (WhichHand == ControllerHand.Controller_Right)
        {
            deviceType = WVR_DeviceType.WVR_DeviceType_Controller_Right;
        }
        else
        {
            deviceType = WVR_DeviceType.WVR_DeviceType_Controller_Left;
        }
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            if (deviceType == WVR_DeviceType.WVR_DeviceType_Controller_Right) onLoadController();
            return;
        }
#endif

        WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.DEVICE_CONNECTED, onDeviceConnected);
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            return;
        }
#endif
        WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.DEVICE_CONNECTED, onDeviceConnected);
    }
    // Use this for initialization
    void Start() {
        if (checkConnection() != connected)
            connected = !connected;

        if (connected) onLoadController();
    }

    private void onDeviceConnected(params object[] args)
    {
        var _dt = (WVR_DeviceType)args[0];
        var _connected = (bool)args[1];
        Log.i(LOG_TAG, "device " + _dt + " is " + (_connected == true ? "connected" : "disconnected"));

        if (deviceType == _dt)
        {
            if (connected != _connected)
            {
                connected = _connected;
            }

            if (connected)
            {
                if (controllerPrefab == null) onLoadController();
            } else
            {
                Destroy(controllerPrefab);
                Resources.UnloadUnusedAssets();  // this is temp solution to fix close suddenly issue, might it came from memory leak
                                                 // to check memory leak when higher priority task is done.
                controllerPrefab = null;
                controllerFileName = "";
                genericControllerFileName = "Generic_";
                WaveVR_Utils.Event.Send(WaveVR_Utils.Event.CONTROLLER_MODEL_UNLOADED, deviceType);
            }
        }
    }

    private void onLoadController() {
        // Make up file name
        // Rule = 
        // ControllerModel_TrackingMethod_CComponent_Hand
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            genericControllerFileName = "Generic_";

            genericControllerFileName += "MC_";

            if (WhichHand == ControllerHand.Controller_Right)
            {
                genericControllerFileName += "R";
            }
            else
            {
                genericControllerFileName += "L";
            }

            originalControllerPrefab = Resources.Load(controllerModelFoler + genericControllerFileName) as GameObject;
            if (originalControllerPrefab == null)
            {
                Debug.Log("Cant load generic controller model, Please check file under Resources/" + controllerModelFoler + genericControllerFileName + ".prefab is exist!");
            }
            else
            {
                Debug.Log(genericControllerFileName + " controller model is found!");
                controllerPrefab = Instantiate(originalControllerPrefab, transform.position, transform.rotation);
                controllerPrefab.transform.parent = this.transform.parent;

                if (TrackingMethod == CTrackingSpace.CTS_6DOF)
                {
                    WaveVR_ControllerPoseTracker armComp = controllerPrefab.GetComponent<WaveVR_ControllerPoseTracker>();
                    if (armComp != null)
                    {
                        armComp.trackPosition = true;
                    }
                    Debug.Log("Controller model CTS_USE_Position");
                }

                Debug.Log("Controller model loaded");
                if (onControllerModelLoaded != null)
                {
                    Debug.Log("trigger delegate");
                    onControllerModelLoaded(controllerPrefab);
                }
            }
            return;
        }
#endif
        if (WhichHand == ControllerHand.Controller_Right)
        {
            deviceType = WVR_DeviceType.WVR_DeviceType_Controller_Right;
        }
        else
        {
            deviceType = WVR_DeviceType.WVR_DeviceType_Controller_Left;
        }

        string parameterName = "GetRenderModelName";
        IntPtr ptrParameterName = Marshal.StringToHGlobalAnsi(parameterName);

        IntPtr ptrResult = Marshal.AllocHGlobal(30);
        uint resultVertLength = 30;

        Interop.WVR_GetParameters(deviceType, ptrParameterName, ptrResult, resultVertLength);

        string renderModelName = Marshal.PtrToStringAnsi(ptrResult);

        Log.i(LOG_TAG, "get controller id from runtime is " + renderModelName);

        controllerFileName += renderModelName;
        controllerFileName += "_";

        if (ControllerComponents == CComponent.Multi_Component)
        {
            controllerFileName += "MC_";
        }
        else
        {
            controllerFileName += "OB_";
        }

        if (WhichHand == ControllerHand.Controller_Right)
        {
            controllerFileName += "R";
        }
        else
        {
            controllerFileName += "L";
        }

        Log.i(LOG_TAG, "controller file name is " + controllerFileName);

        originalControllerPrefab = Resources.Load(controllerModelFoler + controllerFileName) as GameObject;
        var found = true;

        if (originalControllerPrefab == null)
        {
            if (WhichHand == ControllerHand.Controller_Right)
            {
                genericControllerFileName += "MC_R";
            }
            else
            {
                genericControllerFileName += "MC_L";
            }
            Log.w(LOG_TAG, "cant find preferred controller model, load generic controller : " + genericControllerFileName);
            Log.i(LOG_TAG, "Please download controller model from .... to have better experience!");
            originalControllerPrefab = Resources.Load(controllerModelFoler + genericControllerFileName) as GameObject;
            if (originalControllerPrefab == null)
            {
                Log.e(LOG_TAG, "Cant load generic controller model, Please check file under Resources/" + controllerModelFoler + genericControllerFileName + ".prefab is exist!");
                found = false;
            } else
            {
                Log.i(LOG_TAG, genericControllerFileName + " controller model is found!");
            }
        } else
        {
            Log.i(LOG_TAG, controllerFileName + " controller model is found!");
        }

        if (found)
        {
            controllerPrefab = Instantiate(originalControllerPrefab, transform.position, transform.rotation);
            controllerPrefab.transform.parent = this.transform.parent;

            if (TrackingMethod == CTrackingSpace.CTS_SYSTEM)
            {
                if (WaveVR.Instance.is6DoFTracking() == 6)
                {
                    WaveVR_ControllerPoseTracker armComp = controllerPrefab.GetComponent<WaveVR_ControllerPoseTracker>();
                    if (armComp != null)
                    {
                        armComp.trackPosition = true;
                    }
                    Log.i(LOG_TAG, "Controller model CTS_ENABLE_ArmModel (CTS_SYSTEM = 6DOF)");
                }
            } else if (TrackingMethod == CTrackingSpace.CTS_6DOF)
            {
                WaveVR_ControllerPoseTracker armComp = controllerPrefab.GetComponent<WaveVR_ControllerPoseTracker>();
                if (armComp != null)
                {
                    armComp.trackPosition = true;
                }
                Log.i(LOG_TAG, "Controller model CTS_ENABLE_ArmModel (force enable)");
            }

            WaveVR_Utils.Event.Send(WaveVR_Utils.Event.CONTROLLER_MODEL_LOADED, deviceType, controllerPrefab);
        }
        Marshal.FreeHGlobal(ptrParameterName);
        Marshal.FreeHGlobal(ptrResult);
    }
	
	// Update is called once per frame
	void Update () {
    }

    private bool checkConnection()
    {
        var wvr = WaveVR.Instance;
        if (wvr != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (WaveVR.DeviceTypes[i] == deviceType)
                {
                    return wvr.connected[i];
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }
}
