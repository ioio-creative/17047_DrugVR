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

public class ControllerListenerTest : MonoBehaviour
{
    private static string LOG_TAG = "ControllerListenerTest";
    public WVR_DeviceType device = WVR_DeviceType.WVR_DeviceType_HMD;
    public GameObject ControlledObject;

    private void HandleConnectionStatus(bool _value)
    {
        #if UNITY_EDITOR
        Debug.Log (device + " is " + (_value ? "connected" : "disconnected"));
        #endif
        Log.d (LOG_TAG, device + " is " + (_value ? "connected" : "disconnected"));
    }

    #region Button Press Down
    private void HandlePressDownMenu()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Menu + " press down");
        #endif
        ControlledObject.SetActive (false);
    }

    private void HandlePressDownGrip()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Grip + " press down");
        #endif
        ControlledObject.SetActive (false);
    }

    private void HandlePressDownTouchpad()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Touchpad + " press down");
        #endif
        ControlledObject.SetActive (false);
    }

    private void HandlePressDownTrigger()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Trigger + " press down");
        #endif
        ControlledObject.SetActive (false);
    }
    #endregion

    #region Button Press Up
    private void HandlePressUpMenu()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Menu + " press up");
        #endif
        ControlledObject.SetActive (true);
    }

    private void HandlePressUpGrip()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Grip + " press up");
        #endif
        ControlledObject.SetActive (true);
    }

    private void HandlePressUpTouchpad()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Touchpad + " press up");
        #endif
        ControlledObject.SetActive (true);
    }

    private void HandlePressUpTrigger()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Trigger + " press up");
        #endif
        ControlledObject.SetActive (true);
    }
    #endregion

    #region Button Touch Down
    private void HandleTouchDownTouchpad()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Touchpad + " touch down");
        #endif
        Log.d (LOG_TAG, "button " + WVR_InputId.WVR_InputId_Alias1_Touchpad + " touch down.");

        Renderer rend = ControlledObject.GetComponent<Renderer> ();
        if (null != rend)
        {
            rend.material.color = Color.green;
        }
    }

    private void HandleTouchDownTrigger()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Trigger + " touch down");
        #endif
        Log.d (LOG_TAG, "button " + WVR_InputId.WVR_InputId_Alias1_Trigger + " touch down.");

        Renderer rend = ControlledObject.GetComponent<Renderer> ();
        if (null != rend)
        {
            rend.material.color = Color.green;
        }
    }
    #endregion

    #region Button Touch Up
    private void HandleTouchUpTouchpad()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Touchpad + " touch up");
        #endif
        Log.d (LOG_TAG, "button " + WVR_InputId.WVR_InputId_Alias1_Touchpad + " touch up.");

        Renderer rend = ControlledObject.GetComponent<Renderer> ();
        if (null != rend)
        {
            rend.material.color = Color.gray;
        }
    }

    private void HandleTouchUpTrigger()
    {
        #if UNITY_EDITOR
        Debug.Log (WVR_InputId.WVR_InputId_Alias1_Trigger + " touch up");
        #endif
        Log.d (LOG_TAG, "button " + WVR_InputId.WVR_InputId_Alias1_Trigger + " touch up.");

        Renderer rend = ControlledObject.GetComponent<Renderer> ();
        if (null != rend)
        {
            rend.material.color = Color.gray;
        }
    }
    #endregion

	// Use this for initialization
	void Start ()
    {
		
	}

    void OnEnable ()
    {
        var _ctrllsn = WaveVR_ControllerListener.Instance;

        if (_ctrllsn != null)
        {
            if (device != WVR_DeviceType.WVR_DeviceType_HMD)
            {
                #if UNITY_EDITOR
                Debug.Log ("Register " + device + " callback functions.");
                #endif
                Log.d (LOG_TAG, "Register " + device + " callback functions.");
                _ctrllsn.Input (device).ConnectionStatusListeners += HandleConnectionStatus;
                _ctrllsn.Input (device).PressDownListenersMenu += HandlePressDownMenu;
                _ctrllsn.Input (device).PressDownListenersGrip += HandlePressDownGrip;
                _ctrllsn.Input (device).PressDownListenersTouchpad += HandlePressDownTouchpad;
                _ctrllsn.Input (device).PressDownListenersTrigger += HandlePressDownTrigger;
                _ctrllsn.Input (device).PressUpListenersMenu += HandlePressUpMenu;
                _ctrllsn.Input (device).PressUpListenersGrip += HandlePressUpGrip;
                _ctrllsn.Input (device).PressUpListenersTouchpad += HandlePressUpTouchpad;
                _ctrllsn.Input (device).PressUpListenersTrigger += HandlePressUpTrigger;
                _ctrllsn.Input (device).TouchDownListenersTouchpad += HandleTouchDownTouchpad;
                _ctrllsn.Input (device).TouchDownListenersTrigger += HandleTouchDownTrigger;
                _ctrllsn.Input (device).TouchUpListenersTouchpad += HandleTouchUpTouchpad;
                _ctrllsn.Input (device).TouchUpListenersTrigger += HandleTouchUpTrigger;
            }
        }
    }

    WVR_InputId[] buttonIds = new WVR_InputId[] {
        WVR_InputId.WVR_InputId_Alias1_Menu,
        WVR_InputId.WVR_InputId_Alias1_Grip,
        WVR_InputId.WVR_InputId_Alias1_Touchpad,
        WVR_InputId.WVR_InputId_Alias1_Trigger
    };

    WVR_InputId[] axisIds = new WVR_InputId[] {
        WVR_InputId.WVR_InputId_Alias1_Touchpad,
        WVR_InputId.WVR_InputId_Alias1_Trigger
    };
	
	// Update is called once per frame
	void Update ()
    {
        bool controller_rotates = true;

        foreach (WVR_InputId buttonId in buttonIds)
        {
            if (WaveVR_ControllerListener.Instance.Input (device).GetPress (buttonId))
            {
                #if UNITY_EDITOR
                Debug.Log (buttonId + " pressed.");
                #endif
                Log.d (LOG_TAG, "button " + buttonId + " pressed.");
            }
        }

        foreach (WVR_InputId axisId in axisIds)
        {
            // button touched
            if (WaveVR_ControllerListener.Instance.Input(device).GetTouch(axisId))
            {
                var axis = WaveVR_ControllerListener.Instance.Input(device).GetAxis(axisId);

                #if UNITY_EDITOR
                Debug.Log("axis: " + axis);
                #endif

                float xangle = 360 * axis.x, yangle = 360 * axis.y;
                Log.d(LOG_TAG, "button " + axisId + " axis xangle: " + xangle + ", yangle: " + yangle);
                xangle = xangle > 0 ? xangle : -xangle;
                yangle = yangle > 0 ? yangle : -yangle;

                ControlledObject.transform.Rotate (xangle*(10*Time.deltaTime), 0, 0);
                ControlledObject.transform.Rotate (0, yangle*(10*Time.deltaTime), 0);

                controller_rotates = false;
            }
        }

        if (controller_rotates)
            ControlledObject.transform.localRotation = WaveVR_ControllerListener.Instance.Input (device).transform.rot;
    }
}
