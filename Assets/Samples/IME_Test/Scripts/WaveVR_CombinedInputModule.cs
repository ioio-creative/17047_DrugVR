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

[CustomEditor(typeof(WaveVR_CombinedInputModule))]
public class WaveVR_CombinedInputModuleEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var myScript = target as WaveVR_CombinedInputModule;

        myScript.RightController = (GameObject)EditorGUILayout.ObjectField ("Right Controller", myScript.RightController, typeof(GameObject), true);
        myScript.RayObjectR = (GameObject)EditorGUILayout.ObjectField ("Ray of ControllerR", myScript.RayObjectR, typeof(GameObject), true);
        myScript.LeftController = (GameObject)EditorGUILayout.ObjectField ("Left Controller", myScript.LeftController, typeof(GameObject), true);
        myScript.RayObjectL = (GameObject)EditorGUILayout.ObjectField ("Ray of ControllerL", myScript.RayObjectL, typeof(GameObject), true);
        myScript.GazeCamera = (GameObject)EditorGUILayout.ObjectField ("Gaze Camera", myScript.GazeCamera, typeof(GameObject), true);
        myScript.TimeToGaze = (float)EditorGUILayout.FloatField ("Time to gaze", myScript.TimeToGaze);
        myScript.ButtonToTrigger = (EControllerButtons)EditorGUILayout.EnumPopup ("Button To Press", myScript.ButtonToTrigger);
        myScript.CanvasTag = EditorGUILayout.TextField ("Canvas Tag", myScript.CanvasTag);
    }
}
#endif

public class WaveVR_CombinedInputModule : PointerInputModule
{
    private static string LOG_TAG = "WaveVR_CombinedInputModule";

    #region Controller Input
    #region Developer specified parameters
    public GameObject RightController = null, LeftController = null;
    public GameObject RayObjectR = null, RayObjectL = null;
    public EControllerButtons ButtonToTrigger = EControllerButtons.Touchpad;
    [TextArea(3,10)]
    public string CanvasTag = null;
    #endregion

    private static WVR_DeviceType ControllerRIndex = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    private static WVR_DeviceType ControllerLIndex = WVR_DeviceType.WVR_DeviceType_Controller_Left;

    #region EventSystem data
    private PointerEventData pointerData;
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private IEnumerator dragUpdate;
    private GameObject prevOverGO = null;
    #endregion

    //private float HoldTime = 0.0f;

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
    private void CastToCenterOfScreen()
    {
        if (pointerData == null)
            pointerData = new PointerEventData (eventSystem);

        pointerData.Reset();

        // Cast ray
        pointerData.position = new Vector2 (0.5f * Screen.width, 0.5f * Screen.height);  // center of screen
        eventSystem.RaycastAll(pointerData, m_RaycastResultCache);

        // Get result of which ray casted
        RaycastResult raycastResult = FindFirstRaycast(m_RaycastResultCache);

        if (raycastResult.gameObject != null && raycastResult.worldPosition == Vector3.zero)
            raycastResult.worldPosition = GetIntersectionPosition(pointerData.enterEventCamera, raycastResult);

        // Update PointerEventData attributes
        pointerData.pointerCurrentRaycast = raycastResult;
        if (raycastResult.gameObject != null)
            pointerData.position = raycastResult.screenPosition;

        m_RaycastResultCache.Clear();
    }

    private void CastToRayHitObject(WVR_DeviceType _dt)
    {
        GameObject _rayobject = null;
        switch (_dt)
        {
        case WVR_DeviceType.WVR_DeviceType_Controller_Right:
            _rayobject = RayObjectR;
            break;
        case WVR_DeviceType.WVR_DeviceType_Controller_Left:
            _rayobject = RayObjectL;
            break;
        default:
            break;
        }

        if (_rayobject == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("CastToRayHitObject, no WaveVR_Raycast object!");
            #endif

            Log.e (LOG_TAG, "CastToRayHitObject, no WaveVR_Raycast object!");
            return;
        } else
        {
            if (_rayobject.GetComponent<WaveVR_Raycast> () == null)
            {
                #if UNITY_EDITOR
                Debug.Log ("CastToRayHitObject, _rayobject doesn't contain WaveVR_Raycast compoment!");
                #endif

                Log.e (LOG_TAG, "CastToRayHitObject, _rayobject doesn't contain WaveVR_Raycast compoment!");
                return;
            }
        }

        if (pointerData == null)
            pointerData = new PointerEventData (eventSystem);

        pointerData.Reset ();

        Vector2 pos = new Vector2();
        RaycastResult raycastResult = new RaycastResult();

        // Get GameObject hit by raycast
        WaveVR_Raycast ray = _rayobject.GetComponent <WaveVR_Raycast> ();
        GameObject go = null;

        if (ray.raycastObject != null)
        {
            go = ray.raycastObject;
            pos = ray.raycastObject.transform.position;
        }

        raycastResult.gameObject = go;

        Log.d (LOG_TAG, "CastToRayHitObject, raycastResult = " + raycastResult + ", cast GameObject = " + (raycastResult.gameObject != null ? raycastResult.gameObject.name : "none"));

        pointerData.position = pos;
        pointerData.pointerCurrentRaycast = raycastResult;
    }
    #endregion

    private GameObject GetCurrentGameObject()
    {
        if (pointerData != null)
        {
            return pointerData.pointerCurrentRaycast.gameObject;
        }

        return null;
    }

    private void OnTriggerButtonOnClick()
    {
        GameObject go = GetCurrentGameObject ();
        if (go != null)
        {
            // Trigger onClick of a button
            Button btn = go.GetComponent<Button> ();
            if (btn != null)
                btn.onClick.Invoke ();
        }
    }

    #region EventSystem
    /**
     * @brief trigger "Pointer Down" of EventTrigger
     **/
    private void OnTriggerDown()
    {
        var go = GetCurrentGameObject();
        if (go == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("OnTriggerDown, can NOT find casted gameobject.");
            #endif
            Log.e (LOG_TAG, "OnTriggerDown, can NOT find casted gameobject.");
            return;
        }

        // Send pointer down event.
        pointerData.pressPosition = pointerData.position;
        pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
        pointerData.pointerPress = ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.pointerDownHandler);

        // Save the pending click state.
        pointerData.eligibleForClick = true;
        pointerData.delta = Vector2.zero;
        pointerData.dragging = false;
        pointerData.useDragThreshold = true;
    }

    /**
     * @brief trigger "Pointer Enter" and "Pointer Exit" of EventTrigger
     **/
    private void OnTriggerEnterAndExit()
    {
        var go = GetCurrentGameObject();
        if (go == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("OnTriggerEnterAndExit, can NOT find casted gameobject.");
            #endif
            Log.e (LOG_TAG, "OnTriggerEnterAndExit, can NOT find casted gameobject.");
            return;
        }

        if (prevOverGO != go)
        {
            if (prevOverGO != null)
                ExecuteEvents.ExecuteHierarchy (prevOverGO, pointerData, ExecuteEvents.pointerExitHandler);
            ExecuteEvents.ExecuteHierarchy (go, pointerData, ExecuteEvents.pointerEnterHandler);
            prevOverGO = go;

            // Save the pending click state.
            pointerData.eligibleForClick = true;
            pointerData.delta = Vector2.zero;
        }
    }

    /**
     * @brief trigger "Pointer Exit" of EventTrigger
     **/
    private void OnTriggerExit()
    {
        var go = GetCurrentGameObject();
        if (go == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("OnTriggerExit, can NOT find casted gameobject.");
            #endif
            Log.e (LOG_TAG, "OnTriggerExit, can NOT find casted gameobject.");
            return;
        }

        if (prevOverGO == go)
        {
            ExecuteEvents.ExecuteHierarchy (prevOverGO, pointerData, ExecuteEvents.pointerExitHandler);
            prevOverGO = null;

            // Save the pending click state.
            pointerData.eligibleForClick = true;
            pointerData.delta = Vector2.zero;
        }
    }

    private void OnTriggerClick ()
    {
        var go = GetCurrentGameObject();
        if (go == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("OnTriggerClick, can NOT find casted gameobject.");
            #endif
            Log.e (LOG_TAG, "OnTriggerClick, can NOT find casted gameobject.");
            return;
        }

        // Send pointer down event.
        pointerData.pressPosition = pointerData.position;
        pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
        pointerData.pointerPress = ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.pointerClickHandler);

        // Save the pending click state.
        pointerData.eligibleForClick = false;
        pointerData.delta = Vector2.zero;
        pointerData.dragging = false;
        pointerData.useDragThreshold = true;
        pointerData.clickCount = 1;
        pointerData.clickTime = Time.unscaledTime;
    }

    private void OnTriggerDragBegin() {
        var go = GetCurrentGameObject();
        if (go == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("OnTriggerDragBegin, can NOT find casted gameobject.");
            #endif
            Log.e (LOG_TAG, "OnTriggerDragBegin, can NOT find casted gameobject.");
            return;
        }

        // Save the drag handler as well
        pointerData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(go);
        if (pointerData.pointerDrag != null)
        {
            ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.initializePotentialDrag);
            pointerData.dragging = false;
            pointerData.useDragThreshold = true;
        }

        //bool moving = pointerData.IsPointerMoving();

        //if (moving && pointerData.pointerDrag != null && !pointerData.dragging) {
        if (pointerData.pointerDrag != null && !pointerData.dragging)
        {
            #if UNITY_EDITOR
            Debug.Log("OnTriggerDragBegin, beginDragHandler, pointerDrag = " + pointerData.pointerDrag);
            #endif
            Log.d (LOG_TAG, "OnTriggerDragBegin, beginDragHandler, pointerDrag = " + pointerData.pointerDrag);
            ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.beginDragHandler);
            pointerData.dragging = true;
        }

        // Drag notification
        //if (moving && pointerData.pointerDrag != null && pointerData.dragging) {
        if (pointerData.pointerDrag != null && pointerData.dragging)
        {
            if (pointerData.pointerPress != null)
            {
                // Before doing drag we should cancel any pointer down state and clear selection.
                if (pointerData.pointerPress != pointerData.pointerDrag)
                {
                    #if UNITY_EDITOR
                    Debug.Log ("OnTriggerDragBegin, pointerUpHandler, pointerPress = " + pointerData.pointerPress);
                    #endif
                    Log.d (LOG_TAG, "OnTriggerDragBegin, pointerUpHandler, pointerPress = " + pointerData.pointerPress);
                    ExecuteEvents.Execute (pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);

                    pointerData.eligibleForClick = false;
                    pointerData.pointerPress = null;
                    pointerData.rawPointerPress = null;
                }
            }

            #if UNITY_EDITOR
            Debug.Log("OnTriggerDragBegin, start dragging... pointerDrag = " + pointerData.pointerDrag);
            #endif
            //ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.dragHandler);
            Log.d (LOG_TAG, "OnTriggerDragBegin, start dragging... pointerDrag = " + pointerData.pointerDrag);

            pointerData.rawPointerPress = go;
            pointerData.eligibleForClick = false;

            StartCoroutine ("DraggingPointer");
        }
    }

    private void OnDragging()
    {
        if (pointerData.pointerDrag != null)
        {
            pointerData.position = new Vector2(0.5f * Screen.width, 0.5f * Screen.height);  // center of screen
            ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.dragHandler);
        }
    }

    IEnumerator DraggingPointer()
    {
        while (true) {
            // This must be done at the end of the frame to ensure that all GameObjects had a chance
            // to read transient controller state (e.g. events, etc) for the current frame before
            // it gets reset.
            yield return waitForEndOfFrame;
            OnDragging();
        }
    }

    private void OnTriggerDragEnd()
    {
        if (pointerData.pointerDrag != null)
        {
            #if UNITY_EDITOR
            Debug.Log("OnTriggerDragEnd, end draging... pointerDrag = " + pointerData.pointerDrag);
            #endif
            Log.d (LOG_TAG, "OnTriggerDragEnd, end draging... pointerDrag = " + pointerData.pointerDrag);

            StopCoroutine ("DraggingPointer");
            ExecuteEvents.Execute (pointerData.pointerDrag, pointerData, ExecuteEvents.endDragHandler);

            pointerData.rawPointerPress = null;
            pointerData.eligibleForClick = true;
        }
    }

    private void OnTriggerDrop()
    {
        var go = GetCurrentGameObject();
        if (go == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("WaveVR_CombinedInputModule::OnTriggerDrop, can NOT find dragged gameobject.");
            #endif
            Log.e (LOG_TAG, "OnTriggerDrop, can NOT find dragged gameobject.");
            return;
        }

        #if UNITY_EDITOR
        Debug.Log ("OnDrop");
        #endif
        Log.d (LOG_TAG, "OnDrop");

        pointerData.position = new Vector2(0.5f * Screen.width, 0.5f * Screen.height);  // center of screen
        ExecuteEvents.Execute (go, pointerData, ExecuteEvents.dropHandler);

        pointerData.rawPointerPress = null;
        pointerData.eligibleForClick = true;
    }
    #endregion

    private void ChangeCanvasEventCamera(WVR_DeviceType _dt)
    {
        var _objects = GameObject.FindGameObjectsWithTag (CanvasTag);
        if (_objects == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("WaveVR_CombinedInputModule::ChangeCanvasEventCamera, objects with tag [" + CanvasTag + "] are not found.");
            #endif
            Log.e (LOG_TAG, "ChangeCanvasEventCamera, objects with tag [" + CanvasTag + "] are not found.");
            return;
        }

        Camera _event_camera = null;
        switch (_dt)
        {
        case WVR_DeviceType.WVR_DeviceType_Controller_Right:
            _event_camera = (Camera)RightController.GetComponent (typeof(Camera));
            break;
        case WVR_DeviceType.WVR_DeviceType_Controller_Left:
            _event_camera = (Camera)LeftController.GetComponent (typeof(Camera));
            break;
        default:
            _event_camera = (Camera)GazeCamera.GetComponent (typeof(Camera));
            break;
        }

        if (_event_camera == null)
        {
            #if UNITY_EDITOR
            Debug.Log ("WaveVR_CombinedInputModule::ChangeCanvasEventCamera, no event camera!");
            #endif
            Log.e (LOG_TAG, "ChangeCanvasEventCamera, no event camera!");
            return;
        }

        foreach (GameObject o in _objects)
        {
            Canvas _canvas = (Canvas)o.GetComponent (typeof(Canvas));
            if (_canvas != null)
            {
                if (_dt != WVR_DeviceType.WVR_DeviceType_HMD)
                {
                    Log.d (LOG_TAG, "Change " + _canvas.name + " canvas event camera to " + _dt);
                }
                _canvas.worldCamera = _event_camera;
            }
        }
    }

    private void ProcessControllerInput()
    {
        bool rightTriggerDown = false, rightTriggerUp = false, leftTriggerDown = false, leftTriggerUp = false;

        /// We don't need to set #if UNITY_EDITOR condition.
        /// In editor mode, XXXTriggerDown value will be overwritten by WaveVR_Controller
        /// In WaveVR_Controller.Update, it checks whether editor mode or not.
        /// In editor mode -> call to emulator provider, otherwise call to SDK.
        rightTriggerDown    = Input.GetMouseButtonDown(1);  // mouse right key
        rightTriggerUp      = Input.GetMouseButtonUp(1);
        leftTriggerDown     = Input.GetMouseButtonDown(0);  // mouse left key
        leftTriggerUp       = Input.GetMouseButtonUp(0);

        // Right controller touchpad clicked
        if (RightController != null)
        {
            rightTriggerDown |= WaveVR_Controller.Input (ControllerRIndex).GetPress ((WVR_InputId)ButtonToTrigger);
            rightTriggerUp |= WaveVR_Controller.Input (ControllerRIndex).GetPressUp ((WVR_InputId)ButtonToTrigger);
        }
        // Left controller touchpad clicked
        if (LeftController != null)
        {
            leftTriggerDown |= WaveVR_Controller.Input (ControllerLIndex).GetPress ((WVR_InputId)ButtonToTrigger);
            leftTriggerUp |= WaveVR_Controller.Input (ControllerLIndex).GetPressUp ((WVR_InputId)ButtonToTrigger);
        }

        /// Keys can be pressed:
        /// - menu key: WaveVR_Controller.Input (index).GetPressDown (WVR_InputId.WVR_InputId_Alias1_Menu)
        /// - grep key: WaveVR_Controller.Input (index).GetPressDown (WVR_InputId.WVR_InputId_Alias1_Grip)
        /// - trigger key: WaveVR_Controller.Input (index).GetPressDown (WVR_InputId.WVR_InputId_Alias1_Trigger)
        /// Keys can be touched:
        /// - touchpad: WaveVR_Controller.Input(index).GetTouchDown(WVR_InputId.WVR_InputId_Alias1_Touchpad)
        /// - trigger key:  WaveVR_Controller.Input(index).GetTouchDown(WVR_InputId.WVR_InputId_Alias1_Trigger)

        if ((rightTriggerDown || rightTriggerUp || leftTriggerDown || leftTriggerUp)/* && !pointerData.eligibleForClick*/)
        {
            // Before casting , change event camera of canvas.
            if (rightTriggerDown || rightTriggerUp)
                ChangeCanvasEventCamera (ControllerRIndex);
            if (leftTriggerDown || leftTriggerUp)
                ChangeCanvasEventCamera (ControllerLIndex);
            // Statically casting first.
            CastToCenterOfScreen ();
            // If nothing being casted statically, continuing casting dynamically.
            if (GetCurrentGameObject () == null)
            {
                if (rightTriggerDown || rightTriggerUp)
                    CastToRayHitObject (ControllerRIndex);
                if (leftTriggerDown || leftTriggerUp)
                    CastToRayHitObject (ControllerLIndex);
            }

            if (rightTriggerDown || leftTriggerDown)
            {
                OnTriggerEnterAndExit ();   // Trigger "Enter" to current object and "Exit" to prev. object
            }
            else if (rightTriggerUp || leftTriggerUp)
            {
                OnTriggerClick();
                OnTriggerExit ();   // Trigger "Exit" to current object
            }
        }
    }
    #endregion

    public override void Process()
    {
        #if UNITY_EDITOR
        ProcessGazeInput();

        if (!Application.isEditor)
        #endif
        {
            if (WaveVR_Controller.Input (WVR_DeviceType.WVR_DeviceType_Controller_Right).connected ||
                WaveVR_Controller.Input (WVR_DeviceType.WVR_DeviceType_Controller_Left).connected)
            {
                // Controller(s) connected
                ProcessControllerInput ();
            } else
            {
                ProcessGazeInput ();
            }
        }
    }

    #region Gaze Input
    public GameObject GazeCamera = null;
    public float TimeToGaze = 3.0f;
    private float gazeTime = 0.0f;
    private void OnTriggeGaze()
    {
        bool sendEvent = false;
        // The gameobject to which raycast positions
        var currentOverGO = pointerData.pointerCurrentRaycast.gameObject;

        if (pointerData.pointerEnter == null && currentOverGO == null) {
            UpdateReticle(currentOverGO, pointerData);
            return;
        }

        if (pointerData.pointerEnter != currentOverGO)
        {
            HandlePointerExitAndEnter (pointerData, currentOverGO);
            //ExecuteEvents.ExecuteHierarchy (pointerData.pointerEnter, pointerData, ExecuteEvents.pointerExitHandler);
            //ExecuteEvents.ExecuteHierarchy (currentOverGO, pointerData, ExecuteEvents.pointerEnterHandler);
            pointerData.pointerEnter = currentOverGO;

            gazeTime = Time.unscaledTime;
            UpdateReticle(currentOverGO, pointerData);
        }
        //if (EventSystem.current.IsPointerOverGameObject () && currentOverGO != null)
        else
        {
            float elapsedTime = Time.unscaledTime;
            if (elapsedTime - gazeTime > TimeToGaze)
            {
                #if UNITY_EDITOR
                Debug.Log ("Selected: {" + currentOverGO.name + "} over " + TimeToGaze + " seconds.");
                #endif
                Log.d (LOG_TAG, "Selected: {" + currentOverGO.name + "} over " + TimeToGaze + " seconds.");
                sendEvent = true;
                gazeTime = Time.unscaledTime;
            }
        }

        // Standalone Input Module information
        pointerData.eligibleForClick = true;    // means clicked
        pointerData.delta = Vector2.zero;
        pointerData.dragging = false;
        pointerData.useDragThreshold = true;

        DeselectIfSelectionChanged (currentOverGO, pointerData);

        if (sendEvent)
        {
            pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
            pointerData.pressPosition = pointerData.position;
            var newPressedGO = ExecuteEvents.ExecuteHierarchy(currentOverGO, pointerData, ExecuteEvents.pointerClickHandler);

            // Determine if double-click
            float time = Time.unscaledTime;
            if (newPressedGO == pointerData.lastPress)
            {
                var diffTime = time - pointerData.clickTime;
                if (diffTime < 0.3f)    // double-click less than 0.3s
                    ++pointerData.clickCount;
                else
                    pointerData.clickCount = 1;
            } else
            {
                pointerData.clickCount = 1;
            }

            pointerData.pointerPress = newPressedGO;
            pointerData.clickTime = time;
        }
        pointerData.rawPointerPress = currentOverGO;
    }

    private void GazeControl()
    {
        CastToCenterOfScreen ();
        OnTriggeGaze();
    }

    private GameObject GetCurrentGameObject(PointerEventData pointerData) {
        if (pointerData != null && pointerData.enterEventCamera != null)
            return pointerData.pointerCurrentRaycast.gameObject;

        return null;
    }

    private Vector3 GetIntersectionPosition(PointerEventData pointerData) {
        if (null == pointerData.enterEventCamera)
            return Vector3.zero;

        float intersectionDistance = pointerData.pointerCurrentRaycast.distance + pointerData.enterEventCamera.nearClipPlane;
        Vector3 intersectionPosition = pointerData.enterEventCamera.transform.position + pointerData.enterEventCamera.transform.forward * intersectionDistance;
        return intersectionPosition;
    }

    private void UpdateReticle (GameObject preGazedObject, PointerEventData pointerEvent) {
        WaveVR_Reticle gazePointer = Object.FindObjectOfType<WaveVR_Reticle>();
        if (gazePointer == null)
            return;

        GameObject curGazeObject = GetCurrentGameObject(pointerEvent);
        Vector3 intersectionPosition = GetIntersectionPosition(pointerEvent);
        //Ray reticle_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isInteractive = pointerEvent.pointerPress != null || ExecuteEvents.GetEventHandler<IPointerClickHandler>(curGazeObject) != null;

        if (curGazeObject == preGazedObject) {
            if (curGazeObject != null) {
                gazePointer.OnGazeStay(pointerEvent.enterEventCamera, curGazeObject, intersectionPosition, isInteractive);
            } else {
                gazePointer.OnGazeExit(pointerEvent.enterEventCamera, preGazedObject);
                return;
            }
        } else {
            if (preGazedObject != null) {
                gazePointer.OnGazeExit(pointerEvent.enterEventCamera, preGazedObject);
            }
            if (curGazeObject != null) {
                gazePointer.OnGazeEnter(pointerEvent.enterEventCamera, curGazeObject, intersectionPosition, isInteractive);
            }
        }
    }

    private void ProcessGazeInput()
    {
        ChangeCanvasEventCamera (WVR_DeviceType.WVR_DeviceType_HMD);
        GazeControl ();
    }

    public override void DeactivateModule()
    {
        base.DeactivateModule ();
        ClearSelection ();
    }
    #endregion
}
