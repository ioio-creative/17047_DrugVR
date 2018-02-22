// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using wvr;
using WaveVR_Log;

public class WaveVR_Init : MonoBehaviour
{
    private const string LOG_TAG = "WVR_Init";

    /// <summary>
    /// The singleton instance of the <see cref="WaveVR_Init"/> class, there only be one instance in a scene.
    /// </summary>
    public static WaveVR_Init Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WaveVR_Init>();
                if (_instance == null)
                {
                    Log.d(LOG_TAG, "WaveVR_Init create an instance");
                    _instance = new GameObject("[WaveVR]").AddComponent<WaveVR_Init>();
                }
            }
            return _instance;
        }
    }
    private static WaveVR_Init _instance;

    void signalSurfaceState(string msg) {
        WaveVR_Render.signalSurfaceState(msg);
    }

    void Start()
    {
        if (WaveVR.Instance != null)
        {
            WaveVR.Instance.onLoadLevel ();

            // if system boots with default controller role left, set left-handed mode to true
            if (Interop.WVR_GetDefaultControllerRole () == WVR_DeviceType.WVR_DeviceType_Controller_Left)
            {
                WaveVR_Controller.SetLeftHandedMode ();
                #if UNITY_EDITOR
                Debug.Log ("Start() Set left-handed mode to " + WaveVR_Controller.IsLeftHanded);
                #endif
                Log.i (LOG_TAG, "Start() Set left-handed mode to " + WaveVR_Controller.IsLeftHanded);
            }
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        if (_instance != this)
        {
            Log.w(LOG_TAG, "Has another [WaveVR] object in a scene. Destory this.");
            Destroy(this);
            return;
        }

#if UNITY_EDITOR
        if (Application.isEditor) return;
#endif
        if (WaveVR.Instance != null)
        {
            Log.d(LOG_TAG, "Initialized");
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Application.isEditor) return;
#endif

        bool ret = false;
        do
        {
            WVR_Event_t vrevent = new WVR_Event_t();
            ret = Interop.WVR_PollEventQueue(ref vrevent);
            if (ret)
                processVREvent(vrevent);
        } while (ret);
    }

    private void processVREvent(WVR_Event_t vrEvent)
    {
        // Process events used by plugin
        switch ((WVR_EventType)vrEvent.common.type)
        {
            case WVR_EventType.WVR_EventType_IpdUpdate:
                {
                    WaveVR_Utils.Event.Send("IpdChanged");
                    if (WaveVR_Render.Instance != null)
                        WaveVR_Render.Expand(WaveVR_Render.Instance);
                }
                break;
            case WVR_EventType.WVR_EventType_DeviceStatusUpdate:
               {
                    WaveVR_Utils.Event.Send("TrackedDeviceUpdated", vrEvent.device.common.type);
               }
               break;
            case WVR_EventType.WVR_EventType_BatteryStatus_Update:
                {
                    WaveVR_Utils.Event.Send("BatteryStatus_Update");
                }
                break;
        case WVR_EventType.WVR_EventType_TouchpadSwipe_LeftToRight:
        case WVR_EventType.WVR_EventType_TouchpadSwipe_RightToLeft:
        case WVR_EventType.WVR_EventType_TouchpadSwipe_DownToUp:
        case WVR_EventType.WVR_EventType_TouchpadSwipe_UpToDown:
            WaveVR_Utils.Event.Send("SWIPE_EVENT", vrEvent.common.type);
            break;
        case WVR_EventType.WVR_EventType_Settings_Controller:
            if (WaveVR.Instance != null)
            {
                WaveVR_Controller.SetLeftHandedMode ();
                Log.i (LOG_TAG, "Set left-handed mode to " + WaveVR_Controller.IsLeftHanded);
            }
            break;
        default:
            break;
        }

        // Send event to developer for all kind of event if developer don't want to add callbacks for every event.
        WaveVR_Utils.Event.Send(WaveVR_Utils.Event.ALL_VREVENT, vrEvent);

        // Send event to developer by name.
        WaveVR_Utils.Event.Send(vrEvent.common.type.ToString(), vrEvent);
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}

