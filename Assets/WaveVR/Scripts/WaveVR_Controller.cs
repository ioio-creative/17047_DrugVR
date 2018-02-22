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
using System;

public class WaveVR_Controller
{
    private static string LOG_TAG = "WaveVR_Controller";

    public static bool IsLeftHanded
    {
        get
        {
            return isLeftHanded;
        }
    }
    private static bool isLeftHanded = false;

    public static void SetLeftHandedMode()
    {
        isLeftHanded = !isLeftHanded;
        #if UNITY_EDITOR
        Debug.Log ("SetLeftHandedMode() " + isLeftHanded);
        #endif
        Log.d (LOG_TAG, "SetLeftHandedMode() left handed? " + isLeftHanded);
    }

    private static Device[] devices;

    /// <summary>
    /// Get the controller by device index.
    /// </summary>
    /// <param name="deviceIndex">The index of the controller.</param>
    /// <returns></returns>
    public static Device Input(WVR_DeviceType deviceIndex)
    {
        if (isLeftHanded)
        {
            switch (deviceIndex)
            {
            case WVR_DeviceType.WVR_DeviceType_Controller_Right:
                deviceIndex = WVR_DeviceType.WVR_DeviceType_Controller_Left;
                break;
            case WVR_DeviceType.WVR_DeviceType_Controller_Left:
                deviceIndex = WVR_DeviceType.WVR_DeviceType_Controller_Right;
                break;
            default:
                break;
            }
        }

        return ChangeRole (deviceIndex);
    }

    private static Device ChangeRole(WVR_DeviceType deviceIndex)
    {
        if (devices == null)
        {
            devices = new Device[Enum.GetNames (typeof(WVR_DeviceType)).Length];
            uint i = 0;
            devices [i++] = new Device (WVR_DeviceType.WVR_DeviceType_HMD);
            devices [i++] = new Device (WVR_DeviceType.WVR_DeviceType_Controller_Right);
            devices [i++] = new Device (WVR_DeviceType.WVR_DeviceType_Controller_Left);
        }

        for (uint i = 0; i < devices.Length; i++)
        {
            if (deviceIndex == devices [i].DeviceType)
            {
                return devices [i];
            }
        }

        return null;
    }

    public class Device
    {
        #region button definition
        public static ulong Input_Mask_Menu         = 1UL << (int)WVR_InputId.WVR_InputId_Alias1_Menu;
        public static ulong Input_Mask_Grip         = 1UL << (int)WVR_InputId.WVR_InputId_Alias1_Grip;
        public static ulong Input_Mask_DPad_Left    = 1UL << (int)WVR_InputId.WVR_InputId_Alias1_DPad_Left;
        public static ulong Input_Mask_DPad_Up      = 1UL << (int)WVR_InputId.WVR_InputId_Alias1_DPad_Up;
        public static ulong Input_Mask_DPad_Right   = 1UL << (int)WVR_InputId.WVR_InputId_Alias1_DPad_Right;
        public static ulong Input_Mask_DPad_Down    = 1UL << (int)WVR_InputId.WVR_InputId_Alias1_DPad_Down;
        public static ulong Input_Mask_Volume_Up    = 1UL << (int)WVR_InputId.WVR_InputId_Alias1_Volume_Up;
        public static ulong Input_Mask_Volume_Down  = 1UL << (int)WVR_InputId.WVR_InputId_Alias1_Volume_Down;
        public static ulong Input_Mask_Touchpad     = 1UL << (int)WVR_InputId.WVR_InputId_Alias1_Touchpad;
        public static ulong Input_Mask_Trigger      = 1UL << (int)WVR_InputId.WVR_InputId_Alias1_Trigger;
        public static ulong Input_Mask_Bumper       = 1UL << (int)WVR_InputId.WVR_InputId_Alias1_Bumper;

        // press
        WVR_InputId[] pressIds = new WVR_InputId[] {
            WVR_InputId.WVR_InputId_Alias1_Menu,
            WVR_InputId.WVR_InputId_Alias1_Grip,
            WVR_InputId.WVR_InputId_Alias1_DPad_Left,
            WVR_InputId.WVR_InputId_Alias1_DPad_Up,
            WVR_InputId.WVR_InputId_Alias1_DPad_Right,  // 5
            WVR_InputId.WVR_InputId_Alias1_DPad_Down,
            WVR_InputId.WVR_InputId_Alias1_Volume_Up,
            WVR_InputId.WVR_InputId_Alias1_Volume_Down,
            WVR_InputId.WVR_InputId_Alias1_Bumper,
            WVR_InputId.WVR_InputId_Alias1_Touchpad,    // 10
            WVR_InputId.WVR_InputId_Alias1_Trigger
        };

        // Timer of each button (has press state) should be seperated.
        // 4 buttons need 4 timer.
        int[] prevFrameCount_press = new int[11]{
            -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1,
            -1 };

        // touch
        WVR_InputId[] touchIds = new WVR_InputId[] {
            WVR_InputId.WVR_InputId_Alias1_Touchpad,
            WVR_InputId.WVR_InputId_Alias1_Trigger
        };

        // Timer of each button (has touch state) should be seperated.
        // 2 buttons need 2 timer.
        int[] prevFrameCount_touch = new int[]{ -1, -1 };
        #endregion

        public Device(WVR_DeviceType dt)
        {
            Log.i (LOG_TAG, "Initialize WaveVR_Controller Device: " + dt);
            DeviceType = dt;
        }

        public WVR_DeviceType DeviceType
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether is the device connected.
        /// </summary>
        public bool connected
        {
            get
            {
                #if UNITY_EDITOR || UNITY_STANDALONE
                if (isEditorMode)
                    return true;
                else
                #endif
                return Interop.WVR_IsDeviceConnected (DeviceType);
            }
        }

        public WaveVR_Utils.RigidTransform transform
        {
            get
            {
                #if UNITY_EDITOR || UNITY_STANDALONE
                if (isEditorMode)
                {
                    var system = WaveVR_PoseSimulator.Instance;
                    return system.GetRigidTransform(DeviceType);
                }
                #endif

                Interop.WVR_GetPoseState (
                    DeviceType,
                    WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround,
                    500,
                    ref pose);
                return new WaveVR_Utils.RigidTransform (pose.PoseMatrix);
            }
        }

        internal WVR_PoseState_t pose;
        internal WVR_Axis_t axis;
        internal WaveVR_Utils.WVR_ButtonState_t state, pre_state;

        #if UNITY_EDITOR || UNITY_STANDALONE
        private bool isEditorMode = true;
        internal WaveVR_Utils.WVR_ButtonState_t emu_state;
        #endif

        #region Timer
        private bool AllowPressActionInAFrame(WVR_InputId _id)
        {
            if (!connected)
                return false;

            for (uint i = 0; i < pressIds.Length; i++)
            {
                if (_id == pressIds [i])
                {
                    if (Time.frameCount != prevFrameCount_press [i])
                    {
                        prevFrameCount_press [i] = Time.frameCount;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool AllowTouchActionInAFrame(WVR_InputId _id)
        {
            if (!connected)
                return false;

            for (uint i = 0; i < touchIds.Length; i++)
            {
                if (_id == touchIds [i])
                {
                    if (Time.frameCount != prevFrameCount_touch [i])
                    {
                        prevFrameCount_touch [i] = Time.frameCount;
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        private void Update_PressState(WVR_InputId _id)
        {
            if (AllowPressActionInAFrame (_id))
            {
                bool _pressed = false;

                ulong _tmpvalue = 0;
                switch (_id)
                {
                case WVR_InputId.WVR_InputId_Alias1_Menu:
                    _tmpvalue = state.BtnPressed & Input_Mask_Menu;
                    if (_tmpvalue == 0)
                        pre_state.BtnPressed &= ~Input_Mask_Menu;
                    else
                        pre_state.BtnPressed |= _tmpvalue;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Grip:
                    _tmpvalue = state.BtnPressed & Input_Mask_Grip;
                    if (_tmpvalue == 0)
                        pre_state.BtnPressed &= ~Input_Mask_Grip;
                    else
                        pre_state.BtnPressed |= _tmpvalue;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_DPad_Left:
                    _tmpvalue = state.BtnPressed & Input_Mask_DPad_Left;
                    if (_tmpvalue == 0)
                        pre_state.BtnPressed &= ~Input_Mask_DPad_Left;
                    else
                        pre_state.BtnPressed |= _tmpvalue;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_DPad_Up:
                    _tmpvalue = state.BtnPressed & Input_Mask_DPad_Up;
                    if (_tmpvalue == 0)
                        pre_state.BtnPressed &= ~Input_Mask_DPad_Up;
                    else
                        pre_state.BtnPressed |= _tmpvalue;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_DPad_Right:
                    _tmpvalue = state.BtnPressed & Input_Mask_DPad_Right;
                    if (_tmpvalue == 0)
                        pre_state.BtnPressed &= ~Input_Mask_DPad_Right;
                    else
                        pre_state.BtnPressed |= _tmpvalue;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_DPad_Down:
                    _tmpvalue = state.BtnPressed & Input_Mask_DPad_Down;
                    if (_tmpvalue == 0)
                        pre_state.BtnPressed &= ~Input_Mask_DPad_Down;
                    else
                        pre_state.BtnPressed |= _tmpvalue;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Volume_Up:
                    _tmpvalue = state.BtnPressed & Input_Mask_Volume_Up;
                    if (_tmpvalue == 0)
                        pre_state.BtnPressed &= ~Input_Mask_Volume_Up;
                    else
                        pre_state.BtnPressed |= _tmpvalue;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Volume_Down:
                    _tmpvalue = state.BtnPressed & Input_Mask_Volume_Down;
                    if (_tmpvalue == 0)
                        pre_state.BtnPressed &= ~Input_Mask_Volume_Down;
                    else
                        pre_state.BtnPressed |= _tmpvalue;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Touchpad:
                    _tmpvalue = state.BtnPressed & Input_Mask_Touchpad;
                    if (_tmpvalue == 0)
                        pre_state.BtnPressed &= ~Input_Mask_Touchpad;
                    else
                        pre_state.BtnPressed |= _tmpvalue;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Trigger:
                    _tmpvalue = state.BtnPressed & Input_Mask_Trigger;
                    if (_tmpvalue == 0)
                        pre_state.BtnPressed &= ~Input_Mask_Trigger;
                    else
                        pre_state.BtnPressed |= _tmpvalue;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Bumper:
                    _tmpvalue = state.BtnPressed & Input_Mask_Bumper;
                    if (_tmpvalue == 0)
                        pre_state.BtnPressed &= ~Input_Mask_Bumper;
                    else
                        pre_state.BtnPressed |= _tmpvalue;
                    break;
                default:
                    break;
                }

                #if UNITY_EDITOR || UNITY_STANDALONE
                if (isEditorMode)
                {
                    var system = WaveVR_PoseSimulator.Instance;
                    _pressed = system.GetButtonPressState(DeviceType, _id);
                } else
                #endif
                {
                    _pressed = Interop.WVR_GetInputButtonState (DeviceType, _id);
                }

                switch (_id)
                {
                case WVR_InputId.WVR_InputId_Alias1_Menu:
                    if (_pressed)
                        state.BtnPressed |= Input_Mask_Menu;
                    else
                        state.BtnPressed &= ~Input_Mask_Menu;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Grip:
                    if (_pressed)
                        state.BtnPressed |= Input_Mask_Grip;
                    else
                        state.BtnPressed &= ~Input_Mask_Grip;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_DPad_Left:
                    if (_pressed)
                        state.BtnPressed |= Input_Mask_DPad_Left;
                    else
                        state.BtnPressed &= ~Input_Mask_DPad_Left;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_DPad_Up:
                    if (_pressed)
                        state.BtnPressed |= Input_Mask_DPad_Up;
                    else
                        state.BtnPressed &= ~Input_Mask_DPad_Up;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_DPad_Right:
                    if (_pressed)
                        state.BtnPressed |= Input_Mask_DPad_Right;
                    else
                        state.BtnPressed &= ~Input_Mask_DPad_Right;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_DPad_Down:
                    if (_pressed)
                        state.BtnPressed |= Input_Mask_DPad_Down;
                    else
                        state.BtnPressed &= ~Input_Mask_DPad_Down;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Volume_Up:
                    if (_pressed)
                        state.BtnPressed |= Input_Mask_Volume_Up;
                    else
                        state.BtnPressed &= ~Input_Mask_Volume_Up;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Volume_Down:
                    if (_pressed)
                        state.BtnPressed |= Input_Mask_Volume_Down;
                    else
                        state.BtnPressed &= ~Input_Mask_Volume_Down;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Touchpad:
                    if (_pressed)
                        state.BtnPressed |= Input_Mask_Touchpad;
                    else
                        state.BtnPressed &= ~Input_Mask_Touchpad;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Trigger:
                    if (_pressed)
                        state.BtnPressed |= Input_Mask_Trigger;
                    else
                        state.BtnPressed &= ~Input_Mask_Trigger;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Bumper:
                    if (_pressed)
                        state.BtnPressed |= Input_Mask_Bumper;
                    else
                        state.BtnPressed &= ~Input_Mask_Bumper;
                    break;
                default:
                    break;
                }
            }
        }

        private void Update_TouchState(WVR_InputId _id)
        {
            if (AllowTouchActionInAFrame (_id))
            {
                bool _touched = false;

                ulong _tmpvalue = 0;
                switch (_id)
                {
                case WVR_InputId.WVR_InputId_Alias1_Touchpad:
                    _tmpvalue = state.BtnTouched & Input_Mask_Touchpad;
                    if (_tmpvalue == 0)
                        pre_state.BtnTouched &= ~Input_Mask_Touchpad;
                    else
                        pre_state.BtnTouched |= _tmpvalue;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Trigger:
                    _tmpvalue = state.BtnTouched & Input_Mask_Trigger;
                    if (_tmpvalue == 0)
                        pre_state.BtnTouched &= ~Input_Mask_Trigger;
                    else
                        pre_state.BtnTouched |= _tmpvalue;
                    break;
                default:
                    break;
                }

                #if UNITY_EDITOR || UNITY_STANDALONE
                if (isEditorMode)
                {
                    var system = WaveVR_PoseSimulator.Instance;
                    _touched = system.GetButtonTouchState(DeviceType, _id);
                } else
                #endif
                {
                    _touched = Interop.WVR_GetInputTouchState (DeviceType, _id);
                }

                switch (_id)
                {
                case WVR_InputId.WVR_InputId_Alias1_Touchpad:
                    if (_touched)
                        state.BtnTouched |= Input_Mask_Touchpad;
                    else
                        state.BtnTouched &= ~Input_Mask_Touchpad;
                    break;
                case WVR_InputId.WVR_InputId_Alias1_Trigger:
                    if (_touched)
                        state.BtnTouched |= Input_Mask_Trigger;
                    else
                        state.BtnTouched &= ~Input_Mask_Trigger;
                    break;
                default:
                    break;
                }
            }
        }

        #region Button Press state
        /// <summary>
        /// Check if button state is equivallent to specified state.
        /// </summary>
        /// <returns><c>true</c>, equal, <c>false</c> otherwise.</returns>
        /// <param name="_id">input button</param>
        public bool GetPress(WVR_InputId _id)
        {
            bool _state = false;
            Update_PressState (_id);

            switch (_id)
            {
            case WVR_InputId.WVR_InputId_Alias1_Menu:
                _state = (state.BtnPressed & Input_Mask_Menu) != 0 ? true : false;
                break;
            case WVR_InputId.WVR_InputId_Alias1_Grip:
                _state = (state.BtnPressed & Input_Mask_Grip) != 0 ? true : false;
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Left:
                _state = (state.BtnPressed & Input_Mask_DPad_Left) != 0 ? true : false;
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Up:
                _state = (state.BtnPressed & Input_Mask_DPad_Up) != 0 ? true : false;
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Right:
                _state = (state.BtnPressed & Input_Mask_DPad_Right) != 0 ? true : false;
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Down:
                _state = (state.BtnPressed & Input_Mask_DPad_Down) != 0 ? true : false;
                break;
            case WVR_InputId.WVR_InputId_Alias1_Volume_Up:
                _state = (state.BtnPressed & Input_Mask_Volume_Up) != 0 ? true : false;
                break;
            case WVR_InputId.WVR_InputId_Alias1_Volume_Down:
                _state = (state.BtnPressed & Input_Mask_Volume_Down) != 0 ? true : false;
                break;
            case WVR_InputId.WVR_InputId_Alias1_Touchpad:
                _state = (state.BtnPressed & Input_Mask_Touchpad) != 0 ? true : false;
                break;
            case WVR_InputId.WVR_InputId_Alias1_Trigger:
                _state = (state.BtnPressed & Input_Mask_Trigger) != 0 ? true : false;
                break;
            case WVR_InputId.WVR_InputId_Alias1_Bumper:
                _state = (state.BtnPressed & Input_Mask_Bumper) != 0 ? true : false;
                break;
            default:
                break;
            }

            return _state;
        }

        /// <summary>
        /// If true, button with _id is pressed, else unpressed.
        /// </summary>
        /// <returns><c>true</c>, if press down was gotten, <c>false</c> otherwise.</returns>
        /// <param name="_id">WVR_ButtonId, id of button</param>
        public bool GetPressDown(WVR_InputId _id)
        {
            bool _state = false;
            Update_PressState (_id);

            switch (_id)
            {
            case WVR_InputId.WVR_InputId_Alias1_Menu:
                _state = (
                    (((state.BtnPressed & Input_Mask_Menu) != 0)
                        && ((pre_state.BtnPressed & Input_Mask_Menu) == 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Grip:
                _state = (
                    (((state.BtnPressed & Input_Mask_Grip) != 0)
                        && ((pre_state.BtnPressed & Input_Mask_Grip) == 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Left:
                _state = (
                    (((state.BtnPressed & Input_Mask_DPad_Left) != 0)
                        && ((pre_state.BtnPressed & Input_Mask_DPad_Left) == 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Up:
                _state = (
                    (((state.BtnPressed & Input_Mask_DPad_Up) != 0)
                        && ((pre_state.BtnPressed & Input_Mask_DPad_Up) == 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Right:
                _state = (
                    (((state.BtnPressed & Input_Mask_DPad_Right) != 0)
                        && ((pre_state.BtnPressed & Input_Mask_DPad_Right) == 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Down:
                _state = (
                    (((state.BtnPressed & Input_Mask_DPad_Down) != 0)
                        && ((pre_state.BtnPressed & Input_Mask_DPad_Down) == 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Volume_Up:
                _state = (
                    (((state.BtnPressed & Input_Mask_Volume_Up) != 0)
                        && ((pre_state.BtnPressed & Input_Mask_Volume_Up) == 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Volume_Down:
                _state = (
                    (((state.BtnPressed & Input_Mask_Volume_Down) != 0)
                        && ((pre_state.BtnPressed & Input_Mask_Volume_Down) == 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Touchpad:
                _state = (
                    (((state.BtnPressed & Input_Mask_Touchpad) != 0)
                        && ((pre_state.BtnPressed & Input_Mask_Touchpad) == 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Trigger:
                _state = (
                    (((state.BtnPressed & Input_Mask_Trigger) != 0)
                        && ((pre_state.BtnPressed & Input_Mask_Trigger) == 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Bumper:
                _state = (
                    (((state.BtnPressed & Input_Mask_Bumper) != 0)
                        && ((pre_state.BtnPressed & Input_Mask_Bumper) == 0))
                    ? true : false);
                break;
            default:
                break;
            }

            return _state;
        }

        /// <summary>
        /// If true, button with _id is unpressed, else pressed.
        /// </summary>
        /// <returns><c>true</c>, if unpress up was gotten, <c>false</c> otherwise.</returns>
        /// <param name="_id">WVR_ButtonId, id of button</param>
        public bool GetPressUp(WVR_InputId _id)
        {
            bool _state = false;
            Update_PressState (_id);

            switch (_id)
            {
            case WVR_InputId.WVR_InputId_Alias1_Menu:
                _state = (
                    (((state.BtnPressed & Input_Mask_Menu) == 0)
                        && ((pre_state.BtnPressed & Input_Mask_Menu) != 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Grip:
                _state = (
                    (((state.BtnPressed & Input_Mask_Grip) == 0)
                        && ((pre_state.BtnPressed & Input_Mask_Grip) != 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Left:
                _state = (
                    (((state.BtnPressed & Input_Mask_DPad_Left) == 0)
                        && ((pre_state.BtnPressed & Input_Mask_DPad_Left) != 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Up:
                _state = (
                    (((state.BtnPressed & Input_Mask_DPad_Up) == 0)
                        && ((pre_state.BtnPressed & Input_Mask_DPad_Up) != 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Right:
                _state = (
                    (((state.BtnPressed & Input_Mask_DPad_Right) == 0)
                        && ((pre_state.BtnPressed & Input_Mask_DPad_Right) != 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_DPad_Down:
                _state = (
                    (((state.BtnPressed & Input_Mask_DPad_Down) == 0)
                        && ((pre_state.BtnPressed & Input_Mask_DPad_Down) != 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Volume_Up:
                _state = (
                    (((state.BtnPressed & Input_Mask_Volume_Up) == 0)
                        && ((pre_state.BtnPressed & Input_Mask_Volume_Up) != 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Volume_Down:
                _state = (
                    (((state.BtnPressed & Input_Mask_Volume_Down) == 0)
                        && ((pre_state.BtnPressed & Input_Mask_Volume_Down) != 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Touchpad:
                _state = (
                    (((state.BtnPressed & Input_Mask_Touchpad) == 0)
                        && ((pre_state.BtnPressed & Input_Mask_Touchpad) != 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Trigger:
                _state = (
                    (((state.BtnPressed & Input_Mask_Trigger) == 0)
                        && ((pre_state.BtnPressed & Input_Mask_Trigger) != 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Bumper:
                _state = (
                    (((state.BtnPressed & Input_Mask_Bumper) == 0)
                        && ((pre_state.BtnPressed & Input_Mask_Bumper) != 0))
                    ? true : false);
                break;
            default:
                break;
            }

            return _state;
        }
        #endregion

        #region Button Touch state
        /// <summary>
        /// If true, button with _id is touched, else untouched..
        /// </summary>
        /// <returns><c>true</c>, if touch was gotten, <c>false</c> otherwise.</returns>
        /// <param name="_id">WVR_ButtonId, id of button</param>
        public bool GetTouch(WVR_InputId _id)
        {
            bool _state = false;
            Update_TouchState (_id);

            switch (_id)
            {
            case WVR_InputId.WVR_InputId_Alias1_Touchpad:
                _state = (state.BtnTouched & Input_Mask_Touchpad) != 0 ? true : false;
                break;
            case WVR_InputId.WVR_InputId_Alias1_Trigger:
                _state = (state.BtnTouched & Input_Mask_Trigger) != 0 ? true : false;
                break;
            default:
                break;
            }

            return _state;
        }

        /// <summary>
        /// If true, button with _id is touched, else untouched..
        /// </summary>
        /// <returns><c>true</c>, if touch was gotten, <c>false</c> otherwise.</returns>
        /// <param name="_id">WVR_ButtonId, id of button</param>
        public bool GetTouchDown(WVR_InputId _id)
        {
            bool _state = false;
            Update_TouchState (_id);

            switch (_id)
            {
            case WVR_InputId.WVR_InputId_Alias1_Touchpad:
                _state = (
                    (((state.BtnTouched & Input_Mask_Touchpad) != 0)
                        && ((pre_state.BtnTouched & Input_Mask_Touchpad) == 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Trigger:
                _state = (
                    (((state.BtnTouched & Input_Mask_Trigger) != 0)
                        && ((pre_state.BtnTouched & Input_Mask_Trigger) == 0))
                    ? true : false);
                break;
            default:
                break;
            }

            return _state;
        }

        /// <summary>
        /// If true, button with _id is touched, else untouched..
        /// </summary>
        /// <returns><c>true</c>, if touch was gotten, <c>false</c> otherwise.</returns>
        /// <param name="_id">WVR_ButtonId, id of button</param>
        public bool GetTouchUp(WVR_InputId _id)
        {
            bool _state = false;
            Update_TouchState (_id);

            switch (_id)
            {
            case WVR_InputId.WVR_InputId_Alias1_Touchpad:
                _state = (
                    (((state.BtnTouched & Input_Mask_Touchpad) == 0)
                        && ((pre_state.BtnTouched & Input_Mask_Touchpad) != 0))
                    ? true : false);
                break;
            case WVR_InputId.WVR_InputId_Alias1_Trigger:
                _state = (
                    (((state.BtnTouched & Input_Mask_Trigger) == 0)
                        && ((pre_state.BtnTouched & Input_Mask_Trigger) != 0))
                    ? true : false);
                break;
            default:
                break;
            }

            return _state;
        }
        #endregion

        public Vector2 GetAxis(WVR_InputId _id)
        {
            if (!connected)
                return Vector2.zero;

            if (_id != WVR_InputId.WVR_InputId_Alias1_Touchpad && _id != WVR_InputId.WVR_InputId_Alias1_Trigger)
            {
                Log.e (LOG_TAG, "GetAxis, button " + _id + " does NOT have axis!");
                return Vector2.zero;
            }

            #if UNITY_EDITOR || UNITY_STANDALONE
            if (isEditorMode)
            {
                var system = WaveVR_PoseSimulator.Instance;
                axis = system.GetAxis(DeviceType, _id);
            } else
            #endif
            {
                axis = Interop.WVR_GetInputAnalogAxis (DeviceType, _id);
            }

            //Log.d (LOG_TAG, "GetAxis: {" + axis.x + ", " + axis.y + "}");
            return new Vector2 (axis.x, axis.y);
        }

        public void TriggerHapticPulse(
            ushort _durationMicroSec = 500, 
            WVR_InputId _id = WVR_InputId.WVR_InputId_Alias1_Touchpad
        )
        {
            #if UNITY_EDITOR || UNITY_STANDALONE
            if (isEditorMode)
            {
                var system = WaveVR_PoseSimulator.Instance;
                system.TriggerHapticPulse (DeviceType, _id, _durationMicroSec);
            } else
            #endif
            {
                Interop.WVR_TriggerVibrator (DeviceType, _id, _durationMicroSec);
            }
        }

    } // public class Device
}
