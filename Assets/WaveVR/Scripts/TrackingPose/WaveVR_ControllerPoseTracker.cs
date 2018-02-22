// Copyright 2016 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using wvr;
//using UnityEngine.VR;
using System.Collections;
using WaveVR_Log;

/// A standard interface to interact with a scene with the controller.
/// It is responsible for:
/// -  Determining the orientation and location of the controller.
/// -  Predict the location of the shoulder, elbow, wrist, and pointer.
public class WaveVR_ControllerPoseTracker : MonoBehaviour
{
    private static string LOG_TAG = "WaveVR_ControllerPoseTracker";
    #region Developer variables
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

    /// Height of the elbow  (m).
    [Range(0.0f, 0.2f)]
    public float ElbowRaiseYaxis = 0.0f;

    /// Depth of the elbow  (m).
    [Range(0.0f, 0.4f)]
    public float ElbowRaiseZaxis = 0.0f;
    #endregion

    private GameObject Head = null;

    /// The Downward tilt or pitch of the laser pointer relative to the controller (degrees).
    [HideInInspector]
    [Range(0.0f, 30.0f)]
    public float pointerTiltAngle = 15.0f;

    /// Quaternion to represent the pointer's rotation relative to
    /// the controller.
    public Quaternion PointerRotationFromController
    {
        get
        {
            return Quaternion.AngleAxis(pointerTiltAngle, Vector3.right);
        }
    }

    /// Offset of the laser pointer origin relative to the wrist (meters)
    //private static readonly Vector3 POINTER_OFFSET = new Vector3(0.0f, -0.009f, 0.099f);


    /// Vector to present the arm model position of controller.
    /// NOTE: This is in meatspace coordinates.
    private Vector3 controllerArmModelPosition;

    /// Quaternion to present the arm model rotation of controller.
    /// NOTE: This is in meatspace coordinates.
    private Quaternion controllerArmModelRotation;

    /// Last controller rotation value of arm model
    private Quaternion lastCtrlrRtPose = Quaternion.identity;

    /// Last head position value of arm model
    private Vector3 lastHeadPosition = Vector3.zero;

    /// The angle bound of controller without head position.
    private float noHeadPosAngleBound = 5.0f;

    /// controller lerp speed for smooth movement between with head position case and without head position case
    private float smoothMoveSpeed = 0.3f;

    /// The init flag of lastCtrlrRtPose and lastHeadPosition
    private bool firstPosInit = true;

    /// true: calculating position without head position case, false: calculating position with head position case
    private bool noHeadPos = false;

    /// X axis of arm is reverse in left / right arm.
    private Vector3 v3ChangeArmXAxis = new Vector3(0, 1, 1);

    /// Head's real rotation, not local rotation.
    private Quaternion headRotation;

    /// Vector to present the body's rotation.
    private Vector3 bodyDirection;

    /// Quaternion to present the shoulder's rotation.
    /// NOTE: This is in meatspace coordinates.
    private Quaternion bodyRotation;

    /// Simulated direction of wrist.
    private Quaternion wristOrientation;

    /// Offset (position) related to view center. (meters)
    private Vector3 HEADTOELBOW_OFFSET = new Vector3(0.2f, -0.7f, 0f);
    private Vector3 ELBOW_PITCH_OFFSET = new Vector3(-0.15f, 0.55f, 0.08f);
    private float ELBOW_PITCH_ANGLE_MIN = 0, ELBOW_PITCH_ANGLE_MAX = 60;
    /// The elbow curve in XY-plane is not smooth.
    /// The lerp value increases much rapider when elbow raises up.
    private const float ELBOW_TO_XYPLANE_LERP_MIN = 0.45f;
    private const float ELBOW_TO_XYPLANE_LERP_MAX = 0.65f;
    private Vector3 ELBOWTOWRIST_OFFSET = new Vector3(0.0f, 0.0f, 0.15f);
    private Vector3 WRISTTOCONTROLLER_OFFSET = new Vector3(0.0f, 0.0f, 0.05f);

    /// Pitch angle ranges (degrees) of elbow (from head).
    private const float ELBOW_ANGLE_PITCH_MIN = 10.0f;
    private const float ELBOW_ANGLE_PITCH_Y_MAX = 80.0f;    // 90 - ELBOW_ANGLE_PITCH_MIN
    private const float ELBOW_ANGLE_PITCH_MAX = 170.0f;
    private const float ELBOW_ANGLE_PITCH_Y_MIN = -80.0f;   // 90 - ELBOW_ANGLE_PITCH_MAX
    private static readonly float SIMULATED_OFFSET_Y_MIN = -0.5f;
    private static readonly float SIMULATED_OFFSET_Y_MAX = 0.55f;

    // Yaw angle ranges (degrees) of elbow (from right).
    private const float ELBOW_ANGLE_YAW_MIN = 0.0f;
    private const float ELBOW_ANGLE_YAW_X_MAX = 90.0f;  // 90 - ELBOW_ANGLE_YAW_MIN
    private const float ELBOW_ANGLE_YAW_MAX = 180.0f;
    private const float ELBOW_ANGLE_YAW_X_MIN = -90.0f; // 90 - ELBOW_ANGLE_YAW_MAX
    private static readonly float SIMULATED_OFFSET_X_MIN = -0.35f;
    private static readonly float SIMULATED_OFFSET_X_MAX = 0.35f;

    // Z-axis ranges (meter) of wrist (from body).
    private const float ELBOWTOWRIST_OFFSET_Z_MAX = 0.3f;
    private const float ELBOWTOWRIST_OFFSET_Z_MIN_UP = 0.05f;
    private const float ELBOWTOWRIST_OFFSET_Z_MIN_DOWN = 0.05f;

    /// Strength of the acceleration filter (unitless).
    private const float GRAVITY_CALIB_STRENGTH = 0.999f;

    /// Strength of the velocity suppression (unitless).
    private const float VELOCITY_FILTER_SUPPRESS = 0.99f;

    /// Strength of the velocity suppression during low acceleration (unitless).
    private const float LOW_ACCEL_VELOCITY_SUPPRESS = 0.9f;

    /// Strength of the acceleration suppression during low velocity (unitless).
    private const float LOW_VELOCITY_ACCEL_SUPPRESS = 0.5f;

    /// The minimum allowable accelerometer reading before zeroing (m/s^2).
    private const float MIN_ACCEL = 1.0f;

    /// The expected force of gravity (m/s^2).
    private const float GRAVITY_FORCE = 9.807f;

    /// Amount of normalized alpha transparency to change per second.
    private const float DELTA_ALPHA = 4.0f;

    private void ReadJsonValues()
    {
        string json_values = WaveVR_Utils.OEMConfig.getControllerConfig ();
        if (!json_values.Equals (""))
        {
            SimpleJSON.JSONNode jsNodes = SimpleJSON.JSONNode.Parse (json_values);
            string node_value = "";

            char[] split_symbols = { '[', ',', ']' };

            node_value = jsNodes ["arm"] ["head_to_elbow_offset"].Value;
            if (!node_value.Equals (""))
            {
                string[] split_value = node_value.Split (split_symbols);
                HEADTOELBOW_OFFSET = new Vector3 (float.Parse (split_value [1]), float.Parse (split_value [2]), float.Parse (split_value [3]));
            }

            node_value = jsNodes ["arm"] ["elbow_to_wrist_offset"].Value;
            if (!node_value.Equals (""))
            {
                string[] split_value = node_value.Split (split_symbols);
                ELBOWTOWRIST_OFFSET = new Vector3 (float.Parse (split_value [1]), float.Parse (split_value [2]), float.Parse (split_value [3]));
            }

            node_value = jsNodes ["arm"] ["wrist_to_controller_offset"].Value;
            if (!node_value.Equals (""))
            {
                string[] split_value = node_value.Split (split_symbols);
                WRISTTOCONTROLLER_OFFSET = new Vector3 (float.Parse (split_value [1]), float.Parse (split_value [2]), float.Parse (split_value [3]));
            }

            node_value = jsNodes ["arm"] ["elbow_pitch_angle_offset"].Value;
            if (!node_value.Equals (""))
            {
                string[] split_value = node_value.Split (split_symbols);
                ELBOW_PITCH_OFFSET = new Vector3 (float.Parse (split_value [1]), float.Parse (split_value [2]), float.Parse (split_value [3]));
            }

            node_value = jsNodes ["arm"] ["elbow_pitch_angle_min"].Value;
            if (!node_value.Equals (""))
            {
                ELBOW_PITCH_ANGLE_MIN = float.Parse (node_value);
            }

            node_value = jsNodes ["arm"] ["elbow_pitch_angle_max"].Value;
            if (!node_value.Equals (""))
            {
                ELBOW_PITCH_ANGLE_MAX = float.Parse (node_value);
            }

            Log.d (LOG_TAG, 
                "HEADTOELBOW_OFFSET: " + HEADTOELBOW_OFFSET.ToString() +
                "\nELBOWTOWRIST_OFFSET: " + ELBOWTOWRIST_OFFSET.ToString() +
                "\nWRISTTOCONTROLLER_OFFSET: " + WRISTTOCONTROLLER_OFFSET.ToString() +
                "\nELBOW_PITCH_OFFSET: " + ELBOW_PITCH_OFFSET.ToString() +
                "\nELBOW_PITCH_ANGLE_MIN: " + ELBOW_PITCH_ANGLE_MIN +
                "\nELBOW_PITCH_ANGLE_MAX: " + ELBOW_PITCH_ANGLE_MAX);
        }
    }

    #region Monobehaviour
    void Start ()
    {
        if (Head == null)
            Head = WaveVR_Render.Instance.gameObject;
    }

    void OnEnable()
    {
        ReadJsonValues ();

        if (timing == TrackingEvent.WhenNewPoses)
            WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.NEW_POSES, OnNewPoses);

        WaveVR_Utils.Event.Listen(wvr.WVR_EventType.WVR_EventType_RecenterSuccess_3DoF.ToString(), OnRecentered);
    }

    void OnDisable()
    {
        if (timing == TrackingEvent.WhenNewPoses)
            WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.NEW_POSES, OnNewPoses);

        WaveVR_Utils.Event.Remove(wvr.WVR_EventType.WVR_EventType_RecenterSuccess_3DoF.ToString(), OnRecentered);
    }

    void Update ()
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
    #endregion

    private void OnRecentered(params object[] args)
    {
        // do something when recentered.
    }

    private WVR_DevicePosePair_t wvr_pose = new WVR_DevicePosePair_t ();
    private WaveVR_Utils.RigidTransform rigid_pose = WaveVR_Utils.RigidTransform.identity;

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

    private void updatePose(WVR_DevicePosePair_t pose, WaveVR_Utils.RigidTransform rtPose)
    {
        if (trackRotation == true)
        {
            if (trackPosition == true)
            {
                updateDevicePose (pose, rtPose);
            } else
            {
                if (Head != null)
                {
                    switch (type)
                    {
                    case WVR_DeviceType.WVR_DeviceType_Controller_Right:
                        v3ChangeArmXAxis.x = WaveVR_Controller.IsLeftHanded ? -1.0f : 1.0f;
                        break;
                    case WVR_DeviceType.WVR_DeviceType_Controller_Left:
                        v3ChangeArmXAxis.x = WaveVR_Controller.IsLeftHanded ? 1.0f : -1.0f;
                        break;
                    default:
                        break;
                    }

                    updateControllerPose (pose, rtPose);
                }
            }
        }
    }

    private void updateDevicePose(WVR_DevicePosePair_t pose, WaveVR_Utils.RigidTransform rtPose)
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

    private uint posePulseCount = 0;
    private void updateControllerPose(WVR_DevicePosePair_t pose, WaveVR_Utils.RigidTransform rtPose)
    {
        // Place the shoulder in anatomical positions based on the height and handedness.
        bodyRotation = Quaternion.identity;

        Vector3 v3ControllerAngularVelocity =
            new Vector3 (pose.pose.AngularVelocity.v0, pose.pose.AngularVelocity.v1, pose.pose.AngularVelocity.v2);
        UpdateBodyRotation(v3ControllerAngularVelocity);

        ComputeControllerPose2 (pose, rtPose);

        if (controllerArmModelPosition.y - transform.localPosition.y > 0.8f || transform.localPosition.y - controllerArmModelPosition.y > 0.8f)
        {
            posePulseCount++;
            if (posePulseCount <= 5)
            {
                Log.w (LOG_TAG, "Skip rendering wrong pose!!");
                return;
            } else
            {
                posePulseCount = 0;
            }
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, controllerArmModelPosition, smoothMoveSpeed); ;
        transform.localRotation = controllerArmModelRotation;
    }

    private Vector3 GetHeadForward()
    {
        return GetHeadRotation() * Vector3.forward;
    }

    private Quaternion GetHeadRotation()
    {
        return Head.transform.localRotation;
    }

    private Vector3 GetHeadPosition()
    {
        return Head.transform.localPosition;
    }

    private void UpdateBodyRotation(Vector3 v3AngularVelocity)
    {
        // Determine the gaze direction horizontally.
        Vector3 gazeDirection = GetHeadForward();
        gazeDirection.y = 0;
        gazeDirection.Normalize();

        bodyDirection = gazeDirection;
        bodyRotation = Quaternion.FromToRotation(Vector3.forward, bodyDirection);
    }

    /// <summary>
    /// Gets the relative controller rotation.
    /// <para></para>
    /// <para>When user moves controller and body concurrently,
    /// the controller rotation seen by user is not real rotation of controller in world space.</para>
    /// The rotation related to body is
    /// <para></para>
    /// (angle of controller rotation) - (angle of body rotation).
    /// </summary>
    /// <returns>The relative controller rotation.</returns>
    /// <param name="rot">rotation of controller in world space.</param>
    private Quaternion GetRelativeControllerRotation(Quaternion rot)
    {
        Quaternion _inverseBodyRotation = Quaternion.Inverse (bodyRotation);

        Quaternion _relativeRotation = _inverseBodyRotation * rot;
        return _relativeRotation;
    }

    private Vector3 prevVOffset = Vector3.zero;
    private Vector3 GetVelocityOffset(WVR_DevicePosePair_t pose)
    {
        Vector3 velocity = new Vector3 (pose.pose.Velocity.v0, pose.pose.Velocity.v1, pose.pose.Velocity.v2);
        Vector3 offset = velocity * Time.deltaTime;
        offset.x = offset.x == 0 ? 0 : Mathf.Clamp (offset.x + prevVOffset.x, -0.05f, 0.05f);
        offset.y = offset.y == 0 ? 0 : Mathf.Clamp (offset.y + prevVOffset.y, -0.1f, 0.1f);
        offset.z = offset.z == 0 ? 0 : Mathf.Clamp (offset.z + prevVOffset.z, -0.1f, 0.1f);
        prevVOffset = offset;
        return offset;
    }

    /// <summary>
    /// Get the position of controller in Arm Model
    /// 
    /// Consider the parts construct controller position:
    /// Parts contain elbow, wrist and controller and each part has default offset from head.
    /// 1. simulated elbow offset = default elbow offset apply body rotation = body rotation (Quaternion) * elbow offset (Vector3)
    /// 2. simulated wrist offset = default wrist offset apply elbow rotation = elbow rotation (Quaternion) * wrist offset (Vector3)
    /// 3. simulated controller offset = default controller offset apply wrist rotation = wrist rotation (Quat) * controller offset (V3)
    /// head + 1 + 2 + 3 = controller position.
    /// </summary>
    /// <param name="pose">WVR_DevicePosePair_t</param>
    /// <param name="rtPose">WaveVR_Utils.RigidTransform</param>
    private void ComputeControllerPose2(WVR_DevicePosePair_t pose, WaveVR_Utils.RigidTransform rtPose)
    {
        // if bodyRotation angle is θ, _inverseBodyRation is -θ
        // the operator * of Quaternion in Unity means concatenation, not multipler.
        // If quaternion qA has angle θ, quaternion qB has angle ε,
        // qA * qB will plus θ and ε which means rotating angle θ then rotating angle ε.
        // (_inverseBodyRotation * rotation of controller in world space) means angle ε subtracts angle θ.

        /// 0. Handle two controller position cases: calculating with head position or calculating without head position case
        // initial last head position and last controller rtPose for first time
        if (firstPosInit)
        {
            lastHeadPosition = GetHeadPosition();
            lastCtrlrRtPose = rtPose.rot;
            noHeadPos = false;
            firstPosInit = false;
            Log.w(LOG_TAG, "initial last position value.");
        }

        // If moving controller whose angle is less than the angle bound then calculating controller position without head position.
        // Otherwise, calculating controller position with head position
        if (Quaternion.Angle(lastCtrlrRtPose, rtPose.rot) <= noHeadPosAngleBound)
        {
            noHeadPos = true;
        }
        else
        {
            lastCtrlrRtPose = rtPose.rot;
            noHeadPos = false;
        }

        Quaternion _controllerRotation = Quaternion.Inverse(bodyRotation) * rtPose.rot;
        Vector3 _headPosition;
        if (noHeadPos)
        {
            _headPosition = lastHeadPosition;
        }
        else
        {
            _headPosition = GetHeadPosition();
            lastHeadPosition = _headPosition;
        }

        /// 1. simulated elbow offset = default elbow offset apply body rotation = body rotation (Quaternion) * elbow offset (Vector3)
        // Default left / right elbow offset.
        Vector3 _elbowOffset = Vector3.Scale (HEADTOELBOW_OFFSET, v3ChangeArmXAxis);
        // Default left / right elbow pitch offset.
        Vector3 _elbowPitchOffset = Vector3.Scale (ELBOW_PITCH_OFFSET, v3ChangeArmXAxis) + new Vector3(0.0f, ElbowRaiseYaxis, ElbowRaiseZaxis);

        // Use controller pitch to simulate elbow pitch.
        // Range from ELBOW_PITCH_ANGLE_MIN ~ ELBOW_PITCH_ANGLE_MAX.
        // The percent of pitch angle will be used to calculate the position offset.
        Vector3 _controllerForward = _controllerRotation * Vector3.forward;
        float _controllerPitch = 90.0f - Vector3.Angle (_controllerForward, Vector3.up);    // 0~90
        float _controllerPitchRadio = (_controllerPitch - ELBOW_PITCH_ANGLE_MIN) / (ELBOW_PITCH_ANGLE_MAX - ELBOW_PITCH_ANGLE_MIN);
        _controllerPitchRadio = Mathf.Clamp (_controllerPitchRadio, 0.0f, 1.0f);

        // According to pitch angle percent, plus offset to elbow position.
        _elbowOffset += _elbowPitchOffset * _controllerPitchRadio;
        // Apply body rotation and head position to calculate final elbow position.
        _elbowOffset = _headPosition + bodyRotation * _elbowOffset;


        // Rotation from Z-axis to XY-plane used to simulated elbow & wrist rotation.
        Quaternion _controllerXYRotation = Quaternion.FromToRotation (Vector3.forward, _controllerForward);
        float _controllerXYRotationRadio = (Quaternion.Angle (_controllerXYRotation, Quaternion.identity)) / 180;
        // Simulate the elbow raising curve.
        float _elbowCurveLerpValue = ELBOW_TO_XYPLANE_LERP_MIN + (_controllerXYRotationRadio * (ELBOW_TO_XYPLANE_LERP_MAX - ELBOW_TO_XYPLANE_LERP_MIN));
        Quaternion _controllerXYLerpRotation = Quaternion.Lerp (Quaternion.identity, _controllerXYRotation, _elbowCurveLerpValue);


        /// 2. simulated wrist offset = default wrist offset apply elbow rotation = elbow rotation (Quaternion) * wrist offset (Vector3)
        // Default left / right wrist offset
        Vector3 _wristOffset = Vector3.Scale (ELBOWTOWRIST_OFFSET, v3ChangeArmXAxis);
        // elbow rotation + curve = wrist rotation
        // wrist rotation = controller XY rotation
        // => elbow rotation + curve = controller XY rotation
        // => elbow rotation = controller XY rotation - curve
        Quaternion _elbowRotation = bodyRotation * Quaternion.Inverse(_controllerXYLerpRotation) * _controllerXYRotation;
        // Apply elbow offset and elbow rotation to calculate final wrist position.
        _wristOffset = _elbowOffset + _elbowRotation * _wristOffset;


        /// 3. simulated controller offset = default controller offset apply wrist rotation = wrist rotation (Quat) * controller offset (V3)
        // Default left / right controller offset.
        Vector3 _controllerOffset = Vector3.Scale (WRISTTOCONTROLLER_OFFSET, v3ChangeArmXAxis);
        Quaternion _wristRotation = _controllerXYRotation;
        // Apply wrist offset and wrist rotation to calculate final controller position.
        _controllerOffset = _wristOffset + _wristRotation * _controllerOffset;

        controllerArmModelPosition = /*bodyRotation */ _controllerOffset;
        controllerArmModelRotation = bodyRotation * _controllerRotation;
    }

    private void ComputeControllerPose(WVR_DevicePosePair_t pose, WaveVR_Utils.RigidTransform rtPose)
    {
        // Get relative controller orientation
        Quaternion controllerRelativeRot = rtPose.rot;

        /**
         * Controller relative position =
         *  head position +
         *  elbow relative position +
         *  wrist relative position
         **/
        // -------- 1. get the head position --------
        Vector3 controllerRelativePos = GetHeadPosition();

        // -------- 2. simulate the elbow relative position --------
        Vector3 elbowRelativePos = HEADTOELBOW_OFFSET + new Vector3(0.0f, ElbowRaiseYaxis, ElbowRaiseZaxis);
        elbowRelativePos = Vector3.Scale (elbowRelativePos, v3ChangeArmXAxis);
        controllerRelativePos += elbowRelativePos;

        Vector3 controllerForward = controllerRelativeRot * Vector3.forward;

        float controller_angle_pitch = Vector3.Angle(controllerForward, Vector3.up);
        controller_angle_pitch = Mathf.Clamp (controller_angle_pitch, ELBOW_ANGLE_PITCH_MIN, ELBOW_ANGLE_PITCH_MAX);
        float elbow_angle_pitch = 90.0f - controller_angle_pitch;
        float anglePercent_pitch = elbow_angle_pitch < 0 ?
            elbow_angle_pitch / ELBOW_ANGLE_PITCH_Y_MIN :
            elbow_angle_pitch / ELBOW_ANGLE_PITCH_Y_MAX;
        float simulatedRatioY = Mathf.Clamp(anglePercent_pitch, 0.0f, 1.0f);

        float wrist_y_axis = elbow_angle_pitch < 0 ?
            SIMULATED_OFFSET_Y_MIN * simulatedRatioY :
            SIMULATED_OFFSET_Y_MAX * simulatedRatioY;

        float controller_angle_yaw = Vector3.Angle (controllerForward, Vector3.right);
        controller_angle_yaw = Mathf.Clamp (controller_angle_yaw, ELBOW_ANGLE_YAW_MIN, ELBOW_ANGLE_YAW_MAX);
        float elbow_angle_yaw = 90.0f - controller_angle_yaw;
        float anglePercent_yaw = elbow_angle_yaw < 0 ?
            elbow_angle_yaw / ELBOW_ANGLE_YAW_X_MIN :
            elbow_angle_yaw / ELBOW_ANGLE_YAW_X_MAX;
        float simulatedRadioX = Mathf.Clamp (anglePercent_yaw, 0.0f, 1.0f);

        float wrist_x_axis = elbow_angle_yaw < 0 ?
            SIMULATED_OFFSET_X_MIN * simulatedRadioX :
            SIMULATED_OFFSET_X_MAX * simulatedRadioX;

        // for Z-axis coordinate
        float wrist_z_range = elbow_angle_pitch < 0 ?
            ELBOWTOWRIST_OFFSET_Z_MAX - ELBOWTOWRIST_OFFSET_Z_MIN_DOWN :
            ELBOWTOWRIST_OFFSET_Z_MAX - ELBOWTOWRIST_OFFSET_Z_MIN_UP;
        float wrist_z_axis = ELBOWTOWRIST_OFFSET_Z_MAX - (simulatedRatioY * wrist_z_range);

        Vector3 wristRelativePos = new Vector3 (wrist_x_axis, wrist_y_axis, wrist_z_axis);
        controllerRelativePos += wristRelativePos;

        controllerRelativePos = bodyRotation * controllerRelativePos;

        controllerArmModelPosition = controllerRelativePos;
        controllerArmModelRotation = controllerRelativeRot;
    }

    private void ApplyHeadRotation(Vector3 startPosition, Quaternion startRotation)
    {
        float head_yaw = Head.transform.eulerAngles.y;                  // new yaw angle
        Quaternion quat_head_yaw = Quaternion.Euler (0, head_yaw, 0);   // new yaw rotation
        //float whirl_movement = head_yaw - headRotation.eulerAngles.y;               // (new - old) yaw angle
        //Quaternion quat_whirl_movement = Quaternion.Euler(0, whirl_movement, 0);    // (new - old) yaw rotation

        transform.position = Quaternion.AngleAxis (head_yaw, Vector3.up) * startPosition;
        transform.localRotation = startRotation * quat_head_yaw;

        //headRotation = Head.transform.rotation; // refresh old rotation
    }
}
