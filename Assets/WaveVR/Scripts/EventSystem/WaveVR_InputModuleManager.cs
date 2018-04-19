using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using wvr;
using WaveVR_Log;
using UnityEngine.SceneManagement;

public class WaveVR_InputModuleManager : MonoBehaviour
{
    private const string LOG_TAG = "WaveVR_InputModuleManager";

    private void PrintDebugLog(string msg)
    {
        #if UNITY_EDITOR
        Debug.Log(LOG_TAG + " " + msg);
        #endif
        Log.d (LOG_TAG, msg);
    }

    #region Gaze parameters
    [System.Serializable]
    public class CGazeInputModule
    {
        public bool EnableGaze = false;
        public bool progressRate = true;  // The switch to show how many percent to click by TimeToGaze
        public float RateTextZPosition = 0.5f;
        public bool progressCounter = true;  // The switch to show how long to click by TimeToGaze
        public float CounterTextZPosition = 0.5f;
        public float TimeToGaze = 2.0f;
        public EGazeInputEvent InputEvent = EGazeInputEvent.PointerSubmit;
        public GameObject Head = null;
    }

    public CGazeInputModule Gaze;
    #endregion

    #region Controller Input Module parameters
    [System.Serializable]
    public class CControllerInputModule
    {
        public bool EnableController = true;
        public GameObject RightController;
        public LayerMask RightRaycastMask = ~0;
        public GameObject LeftController;
        public LayerMask LeftRaycastMask = ~0;
        public EControllerButtons ButtonToTrigger = EControllerButtons.Touchpad;
        public ERaycastMode RaycastMode = ERaycastMode.Mouse;
        public ERaycastStartPoint RaycastStartPoint = ERaycastStartPoint.CenterOfEyes;
        [Tooltip("Will be obsoleted soon!")]
        public string CanvasTag = "EventCanvas";
    }

    public CControllerInputModule Controller;
    #endregion

    private static WaveVR_InputModuleManager instance = null;
    public static WaveVR_InputModuleManager Instance {
        get
        {
            return instance;
        }
    }

    private GameObject Head = null;
    private GameObject eventSystem = null;
    private GazeInputModule gazeInputModule = null;
    private WaveVR_Reticle gazePointer = null;
    private WaveVR_ControllerInputModule controllerInputModule = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        if (EventSystem.current == null)
        {
            EventSystem _es = FindObjectOfType<EventSystem> ();
            if (_es != null)
            {
                eventSystem = _es.gameObject;
                PrintDebugLog ("Start() find current EventSystem: " + eventSystem.name);
            }

            if (eventSystem == null)
            {
                PrintDebugLog ("Start() could not find EventSystem, create new one.");
                eventSystem = new GameObject ("EventSystem", typeof(EventSystem));
                eventSystem.AddComponent<GazeInputModule> ();
            }
        } else
        {
            eventSystem = EventSystem.current.gameObject;
        }

        // Standalone Input Module
        StandaloneInputModule _sim = eventSystem.GetComponent<StandaloneInputModule> ();
        if (_sim != null)
            _sim.enabled = false;

        // Gaze Input Module
        gazeInputModule = eventSystem.GetComponent<GazeInputModule> ();
        if (Gaze.EnableGaze)
        {
            if (gazeInputModule == null)
                CreateGazeInputModule ();
            else
                SetGazeInputModuleParameters ();
        } else
        {
            // Deactivate gaze pointer to prevent showing pointer in scene.
            ActivateGazePointer (false);
        }

        // Controller Input Module
        controllerInputModule = eventSystem.GetComponent<WaveVR_ControllerInputModule> ();
        if (Controller.EnableController)
        {
            if (controllerInputModule == null)
                CreateControllerInputModule ();
            else
                SetControllerInputModuleParameters ();
        }
    }

    private void ActivateGazePointer(bool active)
    {
        if (gazePointer == null)
            gazePointer = Gaze.Head.GetComponentInChildren<WaveVR_Reticle> ();
        if (gazePointer != null)
            gazePointer.gameObject.SetActive (active);
    }

    private void CreateGazeInputModule()
    {
        if (gazeInputModule == null)
        {
            // Before initializing variables of input modules, disable EventSystem to prevent the OnEnable() of input modules being executed.
            eventSystem.SetActive (false);

            gazeInputModule = eventSystem.AddComponent<GazeInputModule> ();
            SetGazeInputModuleParameters ();

            // Enable EventSystem after initializing input modules.
            eventSystem.SetActive (true);
        }
    }

    private void SetGazeInputModuleParameters()
    {
        if (gazeInputModule != null)
        {
            ActivateGazePointer (true);

            gazeInputModule.enabled = false;
            gazeInputModule.progressRate = Gaze.progressRate;
            gazeInputModule.RateTextZPosition = Gaze.RateTextZPosition;
            gazeInputModule.progressCounter = Gaze.progressCounter;
            gazeInputModule.CounterTextZPosition = Gaze.CounterTextZPosition;
            gazeInputModule.TimeToGaze = Gaze.TimeToGaze;
            gazeInputModule.InputEvent = Gaze.InputEvent;
            gazeInputModule.Head = Gaze.Head;
            gazeInputModule.enabled = true;
        }
    }

    private void CreateControllerInputModule()
    {
        if (controllerInputModule == null)
        {
            // Before initializing variables of input modules, disable EventSystem to prevent the OnEnable() of input modules being executed.
            eventSystem.SetActive (false);

            controllerInputModule = eventSystem.AddComponent<WaveVR_ControllerInputModule> ();
            SetControllerInputModuleParameters ();

            // Enable EventSystem after initializing input modules.
            eventSystem.SetActive (true);
        }
    }

    private void SetControllerInputModuleParameters()
    {
        if (controllerInputModule != null)
        {
            controllerInputModule.enabled = false;
            controllerInputModule.RightController = Controller.RightController;
            controllerInputModule.RightRaycastMask = Controller.RightRaycastMask;
            controllerInputModule.LeftController = Controller.LeftController;
            controllerInputModule.LeftRaycastMask = Controller.LeftRaycastMask;
            controllerInputModule.ButtonToTrigger = Controller.ButtonToTrigger;
            controllerInputModule.RaycastMode = Controller.RaycastMode;
            controllerInputModule.RaycastStartPoint = Controller.RaycastStartPoint;
            controllerInputModule.CanvasTag = Controller.CanvasTag;
            controllerInputModule.enabled = true;
        }
    }

    private void SetActiveGaze(bool value)
    {
        if (gazeInputModule != null)
            gazeInputModule.enabled = value;
        else
        {
            if (value)
                CreateGazeInputModule ();
        }
    }

    private void SetActiveController(bool value)
    {
        if (controllerInputModule != null)
            controllerInputModule.enabled = value;
        else
        {
            if (value)
                CreateControllerInputModule ();
        }
    }

    private bool IsAnyControllerConnected()
    {
        bool _result = false;

        foreach (WVR_DeviceType _dt in Enum.GetValues(typeof(WVR_DeviceType)))
        {
            if (_dt == WVR_DeviceType.WVR_DeviceType_HMD)
                continue;

            if (WaveVR_Controller.Input (_dt).connected)
            {
                _result = true;
                break;
            }
        }

        return _result;
    }

    public void Update()
    {
        if (WaveVR_Render.Instance != null)
            Head = WaveVR_Render.Instance.gameObject;

        if (Head != null)
        {
            gameObject.transform.localPosition = Head.transform.localPosition;
            gameObject.transform.localRotation = Head.transform.localRotation;
        }

        if (Gaze.EnableGaze && Controller.EnableController)
        {
            if (IsAnyControllerConnected ())
            {
                // One or more controller connected, using controller input module, disable gaze input module.
                SetActiveGaze (false);
                SetActiveController (true);
            } else
            {
                // No controller connected, using gaze input module.
                SetActiveGaze (true);
                SetActiveController (false);
            }
        } else if (Gaze.EnableGaze)
        {
            // Only using gaze input module
            SetActiveGaze (true);
            SetActiveController (false);
        } else if (Controller.EnableController)
        {
            // Only using controller input module
            SetActiveGaze (false);
            SetActiveController (true);
        } else
        {
            SetActiveGaze (false);
            SetActiveController (false);
        }
    }

    public ERaycastMode GetRaycastMode()
    {
         if (Controller != null)
             return Controller.RaycastMode;
         else if (controllerInputModule != null)
             return controllerInputModule.RaycastMode;
         else
             return ERaycastMode.Beam;
    }
}
