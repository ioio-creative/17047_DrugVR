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
using UnityEngine.EventSystems;
using UnityEngine.UI;
using wvr;
using WaveVR_Log;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(WaveVR_ControllerInputModule))]
public class WaveVR_ControllerInputModuleEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var myScript = target as WaveVR_ControllerInputModule;

        myScript.RightController = (GameObject)EditorGUILayout.ObjectField ("Right Controller", myScript.RightController, typeof(GameObject), true);
        myScript.LeftController = (GameObject)EditorGUILayout.ObjectField ("Left Controller", myScript.LeftController, typeof(GameObject), true);
        myScript.ButtonToTrigger = (EControllerButtons)EditorGUILayout.EnumPopup ("Button To Press", myScript.ButtonToTrigger);
        myScript.CanvasTag = EditorGUILayout.TextField ("Canvas Tag", myScript.CanvasTag);
    }
}
#endif

public enum EControllerButtons
{
    Menu = WVR_InputId.WVR_InputId_Alias1_Menu,
    Touchpad = WVR_InputId.WVR_InputId_Alias1_Touchpad,
    Trigger = WVR_InputId.WVR_InputId_Alias1_Trigger
}

public enum EButtonPressEvent
{
    PointerDown,
    PointerClick,
    PointerSubmit
}

public class WaveVR_ControllerInputModule : BaseInputModule
{
    private const string LOG_TAG = "WaveVR_ControllerInputModule";

    #region Developer specified parameters
    public GameObject RightController, LeftController;
    public EControllerButtons ButtonToTrigger = EControllerButtons.Touchpad;
    [TextArea(3,10)]
    public string CanvasTag = null;
    #endregion

    private const WVR_DeviceType ControllerIndex_Right = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    private const WVR_DeviceType ControllerIndex_Left = WVR_DeviceType.WVR_DeviceType_Controller_Left;

    #region EventSystem data
    private PointerEventData rightHandPointer, leftHandPointer;
    private GameObject prevObject_right, prevObject_left;
    #endregion

    // Do NOT allow event DOWN being sent multiple times during CLICK_TIME.
    // Since UI element of Unity needs time to perform transitions.
    private const float CLICK_TIME = 0.1f;

    /* ------------ basic declaration begins ---------------- */
    [SerializeField]
    private bool mForceModuleActive = true;

    public bool ForceModuleActive
    {
        get { return mForceModuleActive; }
        set { mForceModuleActive = value; }
    }

    public override bool IsModuleSupported()
    {
        return mForceModuleActive;
    }

    public override bool ShouldActivateModule()
    {
        if (!base.ShouldActivateModule ())
            return false;

        if (mForceModuleActive)
            return true;

        return false;
    }

    public override void DeactivateModule() {
        base.DeactivateModule();
        if (rightHandPointer != null)
        {
            OnTriggerUp_Right();
            HandlePointerExitAndEnter(rightHandPointer, null);
            rightHandPointer = null;
        }
        if (leftHandPointer != null)
        {
            OnTriggerUp_Right();
            HandlePointerExitAndEnter(leftHandPointer, null);
            leftHandPointer = null;
        }
    }
    /* ------------- basic declaration ends ------------------ */

    /**
     * @brief get intersection position in world space
     **/
    private Vector3 GetIntersectionPosition(Camera cam, RaycastResult raycastResult)
    {
        // Check for camera
        if (cam == null) {
            return Vector3.zero;
        }

        float intersectionDistance = raycastResult.distance + cam.nearClipPlane;
        Vector3 intersectionPosition = cam.transform.position + cam.transform.forward * intersectionDistance;
        return intersectionPosition;
    }

    #region Cast ray to detect GameObject

    private void GraphicRaycast_Right()
    {
        if (rightHandPointer == null)
            rightHandPointer = new PointerEventData (eventSystem);

        rightHandPointer.Reset ();
        rightHandPointer.position = new Vector2 (0.5f * Screen.width, 0.5f * Screen.height);  // center of screen

        List<RaycastResult> _results = new List<RaycastResult>();

        ChangeCanvasEventCamera (ControllerIndex_Right);

        var _objects = GameObject.FindGameObjectsWithTag (CanvasTag);
        if (_objects == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("GraphicRaycast_Right(), objects with tag [" + CanvasTag + "] are not found.");
            #endif
            Log.e (LOG_TAG, "GraphicRaycast_Right(), objects with tag [" + CanvasTag + "] are not found.");
            return;
        }

        RaycastResult _firstResult = new RaycastResult ();
        // Reset rightHandPointer
        rightHandPointer.pointerCurrentRaycast = _firstResult;
        if (_firstResult.gameObject != null)
            rightHandPointer.position = _firstResult.screenPosition;

        foreach (GameObject o in _objects)
        {
            Canvas _canvas = (Canvas)o.GetComponent (typeof(Canvas));
            if (_canvas != null)
            {
                GraphicRaycaster _gr = _canvas.GetComponent<GraphicRaycaster> ();
                if (_gr != null)
                {
                    _gr.Raycast (rightHandPointer, _results);

                    _firstResult = FindFirstRaycast (_results);

                    #if UNITY_EDITOR
                    if (_firstResult.module != null)
                        Debug.Log ("GraphicRaycast_right(), camera: " + _firstResult.module.eventCamera + ", first result = " + _firstResult);
                    #endif

                    if (_firstResult.gameObject != null && _firstResult.worldPosition == Vector3.zero)
                        _firstResult.worldPosition = GetIntersectionPosition (
                            _firstResult.module.eventCamera,
                            //rightHandPointer.enterEventCamera,
                            _firstResult
                        );

                    if (GetRightHandObject () == null)
                    {
                        // Update if no casted object
                        rightHandPointer.pointerCurrentRaycast = _firstResult;
                        if (_firstResult.gameObject != null)
                            rightHandPointer.position = _firstResult.screenPosition;
                    }

                    _results.Clear ();
                }
            }
        }
    }

    private void GraphicRaycast_Left()
    {
        if (leftHandPointer == null)
            leftHandPointer = new PointerEventData (eventSystem);

        leftHandPointer.Reset ();
        leftHandPointer.position = new Vector2 (0.5f * Screen.width, 0.5f * Screen.height);  // center of screen

        List<RaycastResult> _results = new List<RaycastResult>();

        ChangeCanvasEventCamera (ControllerIndex_Left);

        var _objects = GameObject.FindGameObjectsWithTag (CanvasTag);
        if (_objects == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("GraphicRaycast_Left(), objects with tag [" + CanvasTag + "] are not found.");
            #endif
            Log.e (LOG_TAG, "GraphicRaycast_Left(), objects with tag [" + CanvasTag + "] are not found.");
            return;
        }

        RaycastResult _firstResult = new RaycastResult ();
        // Reset leftHandPointer
        leftHandPointer.pointerCurrentRaycast = _firstResult;
        if (_firstResult.gameObject != null)
            leftHandPointer.position = _firstResult.screenPosition;

        foreach (GameObject o in _objects)
        {
            Canvas _canvas = (Canvas)o.GetComponent (typeof(Canvas));
            if (_canvas != null)
            {
                GraphicRaycaster _gr = _canvas.GetComponent<GraphicRaycaster> ();
                if (_gr != null)
                {
                    _gr.Raycast (leftHandPointer, _results);

                    _firstResult = FindFirstRaycast (_results);

                    #if UNITY_EDITOR
                    if (_firstResult.module != null)
                        Debug.Log ("GraphicRaycast_Left(), camera: " + _firstResult.module.eventCamera + ", first result = " + _firstResult);
                    #endif

                    if (_firstResult.gameObject != null && _firstResult.worldPosition == Vector3.zero)
                        _firstResult.worldPosition = GetIntersectionPosition (
                            _firstResult.module.eventCamera,
                            //leftHandPointer.enterEventCamera,
                            _firstResult
                        );

                    if (GetLeftHandObject () == null)
                    {
                        // Update if no casted object
                        leftHandPointer.pointerCurrentRaycast = _firstResult;
                        if (_firstResult.gameObject != null)
                            leftHandPointer.position = _firstResult.screenPosition;
                    }
                    
                    _results.Clear ();
                }
            }
        }
    }

    private void PhysicRaycast_Right()
    {
        Camera _cam = (Camera)RightController.GetComponent (typeof(Camera));
        PhysicsRaycaster _raycaster = RightController.GetComponent<PhysicsRaycaster> ();
        if (_cam == null || _raycaster == null)
        {
            Log.e (LOG_TAG, "PhysicRaycast_Right() no Camera or Physics Raycaster!");
            return;
        }

        if (rightHandPointer == null)
            rightHandPointer = new PointerEventData (eventSystem);

        rightHandPointer.Reset ();
        rightHandPointer.position = new Vector2 (0.5f * Screen.width, 0.5f * Screen.height);  // center of screen

        List<RaycastResult> _results = new List<RaycastResult>();

        if (_raycaster != null)
            _raycaster.Raycast (rightHandPointer, _results);

        RaycastResult _firstResult = FindFirstRaycast (_results);

        if (_firstResult.module != null)
        {
            #if UNITY_EDITOR
            //Debug.Log ("PhysicRaycast_Right(), camera: " + _firstResult.module.eventCamera + ", first result = " + _firstResult);
            #endif
            //Log.d (LOG_TAG, "PhysicRaycast_Right(), camera: " + _firstResult.module.eventCamera + ", first result = " + _firstResult);
        }

        rightHandPointer.pointerCurrentRaycast = _firstResult;
        if (_firstResult.gameObject != null)
            rightHandPointer.position = _firstResult.screenPosition;
    }

    private void PhysicRaycast_Left()
    {
        Camera _cam = (Camera)LeftController.GetComponent (typeof(Camera));
        PhysicsRaycaster _raycaster = LeftController.GetComponent<PhysicsRaycaster> ();
        if (_cam == null || _raycaster == null)
        {
            Log.e (LOG_TAG, "PhysicRaycast_Left() no Camera or Physics Raycaster!");
            return;
        }

        if (leftHandPointer == null)
            leftHandPointer = new PointerEventData (eventSystem);

        leftHandPointer.Reset ();
        leftHandPointer.position = new Vector2 (0.5f * Screen.width, 0.5f * Screen.height);  // center of screen

        List<RaycastResult> _results = new List<RaycastResult>();

        if (_raycaster != null)
            _raycaster.Raycast (leftHandPointer, _results);

        RaycastResult _firstResult = FindFirstRaycast (_results);

        if (_firstResult.module != null)
        {
            #if UNITY_EDITOR
            //Debug.Log ("PhysicRaycast_Left(), camera: " + _firstResult.module.eventCamera + ", first result = " + _firstResult);
            #endif
            //Log.d (LOG_TAG, "PhysicRaycast_Left(), camera: " + _firstResult.module.eventCamera + ", first result = " + _firstResult);
        }

        leftHandPointer.pointerCurrentRaycast = _firstResult;
        if (_firstResult.gameObject != null)
            leftHandPointer.position = _firstResult.screenPosition;
    }
    #endregion

    private bool CheckNullObject(string func_name, GameObject go)
    {
        if (go == null)
        {
            #if UNITY_EDITOR
            Debug.Log(func_name + " no game object.");
            #endif
            Log.i (LOG_TAG, func_name + " no game object.");
            return true;
        }
        return false;
    }
    private GameObject GetLeftHandObject()
    {
        if (leftHandPointer != null)
            return leftHandPointer.pointerCurrentRaycast.gameObject;
        return null;
    }

    private GameObject GetRightHandObject()
    {
        if (rightHandPointer != null)
            return rightHandPointer.pointerCurrentRaycast.gameObject;
        return null;
    }

    private void onButtonClick(WVR_DeviceType _dt)
    {
        GameObject _go = null;
        if (_dt == ControllerIndex_Right)
        {
            _go = GetRightHandObject ();
            eligibleForButtonClick_Right = false;
        } else if (_dt == ControllerIndex_Left)
        {
            _go = GetLeftHandObject ();
            eligibleForButtonClick_Left = false;
        }

        if (CheckNullObject ("onButtonClick() " + _dt.ToString (), _go))
            return;

        Button _btn = _go.GetComponent<Button> ();
        if (_btn != null)
        {
            #if UNITY_EDITOR
            Debug.Log("onButtonClick() trigger Button.onClick to " + _btn + " from " + _dt.ToString ());
            #endif
            Log.d (LOG_TAG, "onButtonClick() trigger Button.onClick to " + _btn + " from " + _dt.ToString ());
            _btn.onClick.Invoke ();
        } else
        {
            Log.d (LOG_TAG, "onButtonClick() " + _dt.ToString() + ", " + _go + " does NOT contain Button!");
        }
    }

    #region EventSystem
    private void OnTriggerEnterAndExit_Right()
    {
        GameObject _go = GetRightHandObject();

        if (rightHandPointer.pointerEnter != _go)
        {
            #if UNITY_EDITOR
            Debug.Log("OnTriggerEnterAndExit_Right() enter: " + _go + ", exit: " + rightHandPointer.pointerEnter);
            #endif
            HandlePointerExitAndEnter (rightHandPointer, _go);
            if (rightHandPointer.pointerEnter != null)
                Log.d (LOG_TAG, "OnTriggerEnterAndExit_Right() pointerEnter: " + rightHandPointer.pointerEnter + "camera: " + rightHandPointer.enterEventCamera);
        }
    }

    private void OnTriggerEnterAndExit_Left()
    {
        GameObject _go = GetLeftHandObject();

        if (leftHandPointer.pointerEnter != _go)
        {
            #if UNITY_EDITOR
            Debug.Log("OnTriggerEnterAndExit_Left() enter: " + _go + ", exit: " + leftHandPointer.pointerEnter);
            #endif
            HandlePointerExitAndEnter (leftHandPointer, _go);
            if (leftHandPointer.pointerEnter != null)
                Log.d (LOG_TAG, "OnTriggerEnterAndExit_Left() pointerEnter: " + leftHandPointer.pointerEnter + "camera: " + leftHandPointer.enterEventCamera);
        }
    }

    private void OnTriggerHover_Right()
    {
        GameObject _go = GetRightHandObject ();

        ExecuteEvents.ExecuteHierarchy(_go, rightHandPointer, WaveVR_ExecuteEvents.pointerHoverHandler);
    }

    private void OnTriggerHover_Left()
    {
        GameObject _go = GetLeftHandObject ();

        ExecuteEvents.ExecuteHierarchy(_go, leftHandPointer, WaveVR_ExecuteEvents.pointerHoverHandler);
    }

    private void OnTriggerDown_Right()
    {
        GameObject _go = GetRightHandObject ();
        if (CheckNullObject ("OnTriggerDown_Right()", _go))
            return;

        // Send Pointer Down. If not received, get handler of Pointer Click.
        rightHandPointer.pressPosition = rightHandPointer.position;
        rightHandPointer.pointerPressRaycast = rightHandPointer.pointerCurrentRaycast;
        rightHandPointer.pointerPress =
            ExecuteEvents.ExecuteHierarchy(_go, rightHandPointer, ExecuteEvents.pointerDownHandler)
            ?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(_go);

        Log.d (LOG_TAG, "OnTriggerDown_Right() send Pointer Down to " + rightHandPointer.pointerPress + ", current GameObject is " + _go);

        // If Drag Handler exists, send initializePotentialDrag event.
        rightHandPointer.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(_go);
        if (rightHandPointer.pointerDrag != null)
        {
            Log.d (LOG_TAG, "OnTriggerDown_Right() send initializePotentialDrag to " + rightHandPointer.pointerDrag + ", current GameObject is " + _go);
            ExecuteEvents.Execute(rightHandPointer.pointerDrag, rightHandPointer, ExecuteEvents.initializePotentialDrag);
        }

        // press happened (even not handled) object.
        rightHandPointer.rawPointerPress = _go;
        // allow to send Pointer Click event
        rightHandPointer.eligibleForClick = true;
        // reset the screen position of press, can be used to estimate move distance
        rightHandPointer.delta = Vector2.zero;
        // current Down, reset drag state
        rightHandPointer.dragging = false;
        rightHandPointer.useDragThreshold = true;
        // record the count of Pointer Click should be processed, clean when Click event is sent.
        rightHandPointer.clickCount = 1;
        // set clickTime to current time of Pointer Down instead of Pointer Click.
        // since Down & Up event should not be sent too closely. (< CLICK_TIME)
        rightHandPointer.clickTime = Time.unscaledTime;
    }

    private void OnTriggerDown_Left()
    {
        GameObject _go = GetLeftHandObject ();
        if (CheckNullObject ("OnTriggerDown_Left()", _go))
            return;

        // Send Pointer Down. If not received, get handler of Pointer Click.
        leftHandPointer.pressPosition = leftHandPointer.position;
        leftHandPointer.pointerPressRaycast = leftHandPointer.pointerCurrentRaycast;
        leftHandPointer.pointerPress =
            ExecuteEvents.ExecuteHierarchy(_go, leftHandPointer, ExecuteEvents.pointerDownHandler)
            ?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(_go);

        Log.d (LOG_TAG, "OnTriggerDown_Left() send Pointer Down to " + leftHandPointer.pointerPress + ", current GameObject is " + _go);

        // If Drag Handler exists, send initializePotentialDrag event.
        leftHandPointer.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(_go);
        if (leftHandPointer.pointerDrag != null)
        {
            Log.d (LOG_TAG, "OnTriggerDown_Left() send initializePotentialDrag to " + leftHandPointer.pointerDrag + ", current GameObject is " + _go);
            ExecuteEvents.Execute(leftHandPointer.pointerDrag, leftHandPointer, ExecuteEvents.initializePotentialDrag);
        }

        // press happened (even not handled) object.
        leftHandPointer.rawPointerPress = _go;
        // allow to send Pointer Click event
        leftHandPointer.eligibleForClick = true;
        // reset the screen position of press, can be used to estimate move distance
        leftHandPointer.delta = Vector2.zero;
        // current Down, reset drag state
        leftHandPointer.dragging = false;
        leftHandPointer.useDragThreshold = true;
        // record the count of Pointer Click should be processed, clean when Click event is sent.
        leftHandPointer.clickCount = 1;
        // set clickTime to current time of Pointer Down instead of Pointer Click.
        // since Down & Up event should not be sent too closely. (< CLICK_TIME)
        leftHandPointer.clickTime = Time.unscaledTime;
    }

    private void OnDrag_Right()
    {
        if (rightHandPointer.pointerDrag != null && !rightHandPointer.dragging)
        {
            Log.d (LOG_TAG, "OnDrag_Right() send BeginDrag to " + rightHandPointer.pointerDrag);
            ExecuteEvents.Execute(rightHandPointer.pointerDrag, rightHandPointer, ExecuteEvents.beginDragHandler);
            rightHandPointer.dragging = true;
        }

        // Drag notification
        if (rightHandPointer.dragging && rightHandPointer.pointerDrag != null)
        {
            // Before doing drag we should cancel any pointer down state
            if (rightHandPointer.pointerPress != rightHandPointer.pointerDrag)
            {
                Log.d (LOG_TAG, "OnDrag_Right() send Pointer Up to " + rightHandPointer.pointerPress);
                ExecuteEvents.Execute(rightHandPointer.pointerPress, rightHandPointer, ExecuteEvents.pointerUpHandler);

                // since Down state is cleaned, no Click should be processed.
                rightHandPointer.eligibleForClick = false;
                rightHandPointer.pointerPress = null;
                rightHandPointer.rawPointerPress = null;
            }

            /*Log.d (LOG_TAG, "OnDrag_Right() send Pointer Drag to " + rightHandPointer.pointerDrag +
                "camera: " + rightHandPointer.enterEventCamera +
                " (" + rightHandPointer.enterEventCamera.ScreenToWorldPoint (
                    new Vector3 (
                        rightHandPointer.position.x,
                        rightHandPointer.position.y,
                        rightHandPointer.pointerDrag.transform.position.z
                    )) +
                ")");*/
            ExecuteEvents.Execute(rightHandPointer.pointerDrag, rightHandPointer, ExecuteEvents.dragHandler);
        }
    }

    private void OnDrag_Left()
    {
        if (leftHandPointer.pointerDrag != null && !leftHandPointer.dragging)
        {
            Log.d (LOG_TAG, "OnDrag_Left() send BeginDrag to " + leftHandPointer.pointerDrag);
            ExecuteEvents.Execute(leftHandPointer.pointerDrag, leftHandPointer, ExecuteEvents.beginDragHandler);
            leftHandPointer.dragging = true;
        }

        // Drag notification
        if (leftHandPointer.dragging && leftHandPointer.pointerDrag != null)
        {
            // Before doing drag we should cancel any pointer down state
            if (leftHandPointer.pointerPress != leftHandPointer.pointerDrag)
            {
                Log.d (LOG_TAG, "OnDrag_Left() send Pointer Up to " + leftHandPointer.pointerPress);
                ExecuteEvents.Execute(leftHandPointer.pointerPress, leftHandPointer, ExecuteEvents.pointerUpHandler);

                // since Down state is cleaned, no Click should be processed.
                leftHandPointer.eligibleForClick = false;
                leftHandPointer.pointerPress = null;
                leftHandPointer.rawPointerPress = null;
            }

            /*Log.d (LOG_TAG, "OnDrag_Left() send Pointer Drag to " + leftHandPointer.pointerDrag +
                "camera: " + leftHandPointer.enterEventCamera +
                " (" +
                leftHandPointer.enterEventCamera.ScreenToWorldPoint(
                    new Vector3(
                        leftHandPointer.position.x,
                        leftHandPointer.position.y,
                        leftHandPointer.pointerDrag.transform.position.z
                    )) +
                ")");*/
            ExecuteEvents.Execute(leftHandPointer.pointerDrag, leftHandPointer, ExecuteEvents.dragHandler);
        }
    }

    private void OnTriggerUp_Right()
    {
        if (!rightHandPointer.eligibleForClick && !rightHandPointer.dragging)
        {
            // 1. no pending click
            // 2. no dragging
            // Mean user has finished all actions and do NOTHING in current frame.
            return;
        }

        GameObject _go = GetRightHandObject ();
        // _go may be different with rightHandPointer.pointerDrag so we don't check null

        if (rightHandPointer.pointerPress != null)
        {
            // In the frame of button is pressed -> unpressed, send Pointer Up
            Log.d (LOG_TAG, "OnTriggerUp_Right() send Pointer Up to " + rightHandPointer.pointerPress);
            ExecuteEvents.Execute (rightHandPointer.pointerPress, rightHandPointer, ExecuteEvents.pointerUpHandler);
        }
        if (rightHandPointer.eligibleForClick)
        {
            // In the frame of button from being pressed to unpressed, send Pointer Click if Click is pending.
            Log.d (LOG_TAG, "OnTriggerUp_Right() send Pointer Click to " + rightHandPointer.pointerPress);
            ExecuteEvents.Execute(rightHandPointer.pointerPress, rightHandPointer, ExecuteEvents.pointerClickHandler);
        } else if (rightHandPointer.dragging)
        {
            // In next frame of button from being pressed to unpressed, send Drop and EndDrag if dragging.
            Log.d (LOG_TAG, "OnTriggerUp_Right() send Pointer Drop to " + _go + ", EndDrag to " + rightHandPointer.pointerDrag);
            ExecuteEvents.ExecuteHierarchy(_go, rightHandPointer, ExecuteEvents.dropHandler);
            ExecuteEvents.Execute(rightHandPointer.pointerDrag, rightHandPointer, ExecuteEvents.endDragHandler);

            rightHandPointer.pointerDrag = null;
            rightHandPointer.dragging = false;
        }

        // Down of pending Click object.
        rightHandPointer.pointerPress = null;
        // press happened (even not handled) object.
        rightHandPointer.rawPointerPress = null;
        // clear pending state.
        rightHandPointer.eligibleForClick = false;
        // Click is processed, clearcount.
        rightHandPointer.clickCount = 0;
        // Up is processed thus clear the time limitation of Down event.
        rightHandPointer.clickTime = 0;
    }

    private void OnTriggerUp_Left()
    {
        if (!leftHandPointer.eligibleForClick && !leftHandPointer.dragging)
        {
            // 1. no pending click
            // 2. no dragging
            // Mean user has finished all actions and do NOTHING in current frame.
            return;
        }

        GameObject _go = GetLeftHandObject ();
        // _go may be different with leftHandPointer.pointerDrag so we don't check null

        if (leftHandPointer.pointerPress != null)
        {
            // In the frame of button is pressed -> unpressed, send Pointer Up
            Log.d (LOG_TAG, "OnTriggerUp_Left() send Pointer Up to " + leftHandPointer.pointerPress);
            ExecuteEvents.Execute (leftHandPointer.pointerPress, leftHandPointer, ExecuteEvents.pointerUpHandler);
        }
        if (leftHandPointer.eligibleForClick)
        {
            // In the frame of button from being pressed to unpressed, send Pointer Click if Click is pending.
            Log.d (LOG_TAG, "OnTriggerUp_Left() send Pointer Click to " + leftHandPointer.pointerPress);
            ExecuteEvents.Execute(leftHandPointer.pointerPress, leftHandPointer, ExecuteEvents.pointerClickHandler);
        } else if (leftHandPointer.dragging)
        {
            // In next frame of button from being pressed to unpressed, send Drop and EndDrag if dragging.
            Log.d (LOG_TAG, "OnTriggerUp_Left() send Pointer Drop to " + _go + ", EndDrag to " + leftHandPointer.pointerDrag);
            ExecuteEvents.ExecuteHierarchy(_go, leftHandPointer, ExecuteEvents.dropHandler);
            ExecuteEvents.Execute(leftHandPointer.pointerDrag, leftHandPointer, ExecuteEvents.endDragHandler);

            leftHandPointer.pointerDrag = null;
            leftHandPointer.dragging = false;
        }

        // Down of pending Click object.
        leftHandPointer.pointerPress = null;
        // press happened (even not handled) object.
        leftHandPointer.rawPointerPress = null;
        // clear pending state.
        leftHandPointer.eligibleForClick = false;
        // Click is processed, clearcount.
        leftHandPointer.clickCount = 0;
        // Up is processed thus clear the time limitation of Down event.
        leftHandPointer.clickTime = 0;
    }
    #endregion

    private void ChangeCanvasEventCamera(WVR_DeviceType _dt)
    {
        var _objects = GameObject.FindGameObjectsWithTag (CanvasTag);
        if (_objects == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("WaveVR_ControllerInputModule::ChangeCanvasEventCamera, objects with tag [" + CanvasTag + "] are not found.");
            #endif
            Log.e (LOG_TAG, "ChangeCanvasEventCamera, objects with tag [" + CanvasTag + "] are not found.");
            return;
        }

        Camera _event_camera = null;
        switch (_dt)
        {
        case ControllerIndex_Right:
            _event_camera = (Camera)RightController.GetComponent (typeof(Camera));
            break;
        case ControllerIndex_Left:
            _event_camera = (Camera)LeftController.GetComponent (typeof(Camera));
            break;
        default:
            break;
        }

        if (_event_camera == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("WaveVR_ControllerInputModule::ChangeCanvasEventCamera, no event camera!");
            #endif
            Log.e (LOG_TAG, "ChangeCanvasEventCamera, no event camera!");
            return;
        }

        foreach (GameObject o in _objects)
        {
            Canvas _canvas = (Canvas)o.GetComponent (typeof(Canvas));
            if (_canvas != null)
            {
                _canvas.worldCamera = _event_camera;
            }
        }
    }

    private bool eligibleForButtonClick_Right = false;
    private void Process_RightHand()
    {
        prevObject_right = GetRightHandObject ();

        GraphicRaycast_Right ();
        if (GetRightHandObject () == null)
            PhysicRaycast_Right ();
        OnTriggerEnterAndExit_Right ();

        GameObject _curGO = GetRightHandObject ();
        if (_curGO != null && _curGO == prevObject_right)
            OnTriggerHover_Right ();

        // right button not pressed -> pressed, only 1 frame
        bool btnPressDown = Input.GetMouseButtonDown(1);
        // right button pressed
        bool btnPressed = Input.GetMouseButton (1);
        bool btnPressUp = Input.GetMouseButtonUp (1);
        btnPressDown |= WaveVR_Controller.Input (ControllerIndex_Right).GetPressDown ((WVR_InputId)ButtonToTrigger);
        btnPressed |= WaveVR_Controller.Input (ControllerIndex_Right).GetPress ((WVR_InputId)ButtonToTrigger);
        btnPressUp |= WaveVR_Controller.Input (ControllerIndex_Right).GetPressUp ((WVR_InputId)ButtonToTrigger);

        if (btnPressDown)
            eligibleForButtonClick_Right = true;
        if (btnPressUp && eligibleForButtonClick_Right)
            onButtonClick (ControllerIndex_Right);

        /**
         * Button push-up process is: Down -> Up -> Click
         * Up is used to clear Down state.
         * If Down happened, Click is pending (eligibleForClick = true) and to happen while button unpressed.
         * If Down state is cleared, eligibleForClick should be cleared also.
         * 
         * Button pressed-unpressed process is: (Down->) Drag -> (Up->Click->) Drop -> EndDrag
         * In the frame of buttn unpressed, Pointer Click will be sent.
         * In next frame, Drop and EndDrag will be sent.
         **/
        if (!btnPressDown && btnPressed)
        {
            // button hold means to drag.
            OnDrag_Right ();
        } else if (Time.unscaledTime - rightHandPointer.clickTime < CLICK_TIME)
        {
            // Delay new events until CLICK_TIME has passed.
        } else if (btnPressDown && !rightHandPointer.eligibleForClick)
        {
            // 1. button not pressed -> pressed.
            // 2. no pending Click should be procced.
            OnTriggerDown_Right ();
        } else if (!btnPressed)
        {
            // 1. If Down before, send Up event and clear Down state.
            // 2. If Dragging, send Drop & EndDrag event and clear Dragging state.
            // 3. If no Down or Dragging state, do NOTHING.
            OnTriggerUp_Right ();
        }
    }

    private bool eligibleForButtonClick_Left = false;
    private void Process_LeftHand()
    {
        prevObject_left = GetLeftHandObject ();

        GraphicRaycast_Left ();
        if (GetLeftHandObject () == null)
            PhysicRaycast_Left ();
        OnTriggerEnterAndExit_Left ();

        GameObject _curGO = GetLeftHandObject ();
        if (_curGO != null && _curGO == prevObject_left)
            OnTriggerHover_Left ();

        // left button not pressed -> pressed, only 1 frame
        bool btnPressDown = Input.GetMouseButtonDown (0);
        // left button pressed
        bool btnPressed = Input.GetMouseButton (0);
        bool btnPressUp = Input.GetMouseButtonUp (0);
        btnPressDown |= WaveVR_Controller.Input (ControllerIndex_Left).GetPressDown ((WVR_InputId)ButtonToTrigger);
        btnPressed |= WaveVR_Controller.Input (ControllerIndex_Left).GetPress ((WVR_InputId)ButtonToTrigger);
        btnPressUp |= WaveVR_Controller.Input (ControllerIndex_Left).GetPressUp ((WVR_InputId)ButtonToTrigger);

        if (btnPressDown)
            eligibleForButtonClick_Left = true;
        if (btnPressUp && eligibleForButtonClick_Left)
            onButtonClick (ControllerIndex_Left);

        /**
         * Button push-up process is: Down -> Up -> Click
         * Up is used to clear Down state.
         * If Down happened, Click is pending (eligibleForClick = true) and to happen while button unpressed.
         * If Down state is cleared, eligibleForClick should be cleared also.
         * 
         * Button pressed-unpressed process is: (Down->) Drag -> (Up->Click->) Drop -> EndDrag
         * In the frame of buttn unpressed, Pointer Click will be sent.
         * In next frame, Drop and EndDrag will be sent.
         **/
        if (!btnPressDown && btnPressed)
        {
            // button hold means to drag.
            OnDrag_Left ();
        } else if (Time.unscaledTime - leftHandPointer.clickTime < CLICK_TIME)
        {
            // Delay new events until CLICK_TIME has passed.
        } else if (btnPressDown && !leftHandPointer.eligibleForClick)
        {
            // 1. button not pressed -> pressed.
            // 2. no pending Click should be procced.
            OnTriggerDown_Left ();
        } else if (!btnPressed)
        {
            // 1. If Down before, send Up event and clear Down state.
            // 2. If Dragging, send Drop & EndDrag event and clear Dragging state.
            // 3. If no Down or Dragging state, do NOTHING.
            OnTriggerUp_Left ();
        }
    }

    public override void Process()
    {
        bool _bRConnected = WaveVR_Controller.Input (ControllerIndex_Right).connected;
        bool _bLConnected = WaveVR_Controller.Input (ControllerIndex_Left).connected;

        /**
         * Left right & left hand actions be processed in current thread
         **/
        if (RightController != null && _bRConnected)
        {
            Process_RightHand ();
            UpdateReticlePointer_Right ();
            SetupReticleBeam_Right ();
        }
        if (LeftController != null && _bLConnected)
        {
            Process_LeftHand ();
            UpdateReticlePointer_Left ();
            SetupReticleBeam_Left ();
        }
    }

    #region Reticle Pointer
    private WaveVR_ControllerPointer reticlePointer_right = null, reticlePointer_left = null;
    private WaveVR_Beam beam_right = null, beam_left = null;

    private void SetupReticleBeam_Right()
    {
        if (reticlePointer_right == null)
        {
            reticlePointer_right = RightController.GetComponentInChildren<WaveVR_ControllerPointer> ();
            // Remove right reticle by default.
            if (reticlePointer_right != null)
                reticlePointer_right.removePointer ();
        }

        if (beam_right == null)
        {
            beam_right = RightController.GetComponentInChildren<WaveVR_Beam> ();
        }
    }

    private void SetupReticleBeam_Left()
    {
        if (reticlePointer_left == null)
        {
            reticlePointer_left = LeftController.GetComponentInChildren<WaveVR_ControllerPointer> ();
            // Remove left reticle by default.
            if (reticlePointer_left != null)
                reticlePointer_left.removePointer();
        }

        if (beam_left == null)
        {
            beam_left = LeftController.GetComponentInChildren<WaveVR_Beam> ();
        }
    }

    private void UpdateReticlePointer_Right()
    {
        if (reticlePointer_right != null && beam_right != null)
        {
            Vector3 _intersectionPosition = GetIntersectionPosition (rightHandPointer.enterEventCamera, rightHandPointer.pointerCurrentRaycast);
            GameObject _go = GetRightHandObject ();

            if (_go != prevObject_right)
            {
                if (_go != null)
                {
                    reticlePointer_right.SetPointerColor (new Color32 (11, 220, 249, 255));
                    reticlePointer_right.OnPointerEnter (rightHandPointer.enterEventCamera, _go, _intersectionPosition, true);
                    beam_right.SetEndOffset (_intersectionPosition, false);
                } else
                {
                    reticlePointer_right.SetPointerColor (Color.white);
                    reticlePointer_right.OnPointerExit(rightHandPointer.enterEventCamera, prevObject_right);
                    beam_right.ResetEndOffset ();
                }
            }
        }
    }

    private void UpdateReticlePointer_Left()
    {
        if (reticlePointer_left != null && beam_left != null)
        {
            Vector3 _intersectionPosition = GetIntersectionPosition (leftHandPointer.enterEventCamera, leftHandPointer.pointerCurrentRaycast);
            GameObject _go = GetLeftHandObject ();

            if (_go != prevObject_left)
            {
                if (_go != null)
                {
                    reticlePointer_left.SetPointerColor (new Color32 (11, 220, 249, 255));
                    reticlePointer_left.OnPointerEnter(leftHandPointer.enterEventCamera, _go, _intersectionPosition, true);
                    beam_left.SetEndOffset (_intersectionPosition, false);
                } else
                {
                    reticlePointer_left.SetPointerColor (Color.white);
                    reticlePointer_left.OnPointerExit(leftHandPointer.enterEventCamera, prevObject_left);
                    beam_left.ResetEndOffset ();
                }
            }
        }
    }
    #endregion
}
