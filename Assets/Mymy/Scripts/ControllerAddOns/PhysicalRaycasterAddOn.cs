using UnityEngine;
using UnityEngine.EventSystems;
using wvr;

// based on addphysicalRaycaster
// added assignment of evntSystem on Start()
public class PhysicalRaycasterAddOn : MonoBehaviour
{
    private PhysicsRaycaster physicalRaycaster = null;
    public WVR_DeviceType device = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    public LayerMask eventMask = ~0;
    public GameObject evntSystem;
       
    private void Awake()
    {
        if (evntSystem == null)
        {
            evntSystem = GameObject.Find("/System/EventSystem");
        }
    }
    
    private void OnEnable()
    {       
        WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.CONTROLLER_MODEL_LOADED, ControllerLoader);
#if UNITY_EDITOR
        WaveVR_ControllerLoader.onControllerModelLoaded += internal_controllerLoadedHandler;
#endif
    }

    private void OnDisable()
    {
        WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.CONTROLLER_MODEL_LOADED, ControllerLoader);
#if UNITY_EDITOR
        WaveVR_ControllerLoader.onControllerModelLoaded -= internal_controllerLoadedHandler;
#endif
    }

    private void ControllerLoader(params object[] args)
    {
        WVR_DeviceType type = (WVR_DeviceType)args[0];
        GameObject controller = (GameObject)args[1];

        if (device != type) return;
        physicalRaycaster = controller.AddComponent<PhysicsRaycaster>();
        physicalRaycaster.eventMask = eventMask;

        switch (type)
        {
            case WVR_DeviceType.WVR_DeviceType_Controller_Left:
                evntSystem.GetComponent<WaveVR_ControllerInputModule>().LeftController = controller;
                break;
            case WVR_DeviceType.WVR_DeviceType_Controller_Right:
                evntSystem.GetComponent<WaveVR_ControllerInputModule>().RightController = controller;
                break;
            default:
                break;
        }
    }

    private void internal_controllerLoadedHandler(GameObject go)
    {
        WVR_DeviceType type = go.GetComponent<ControllerConnectionStateReactor>().type;
        switch (type)
        {
            case WVR_DeviceType.WVR_DeviceType_Controller_Left:
                evntSystem.GetComponent<WaveVR_ControllerInputModule>().LeftController = go;
                break;
            case WVR_DeviceType.WVR_DeviceType_Controller_Right:
                evntSystem.GetComponent<WaveVR_ControllerInputModule>().RightController = go;
                break;
            default:
                break;
        }

        ControllerLoader(type, go);        
    }
}