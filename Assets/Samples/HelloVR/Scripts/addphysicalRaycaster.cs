using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;
using UnityEngine.EventSystems;

public class addphysicalRaycaster : MonoBehaviour {
    private PhysicsRaycaster physicalRaycaster = null;
    public WVR_DeviceType device = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    public LayerMask eventMask = ~0;
	public GameObject evntSystem;
    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

    void OnEnable()
    {
		

        WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.CONTROLLER_MODEL_LOADED, ControllerLoader);
#if UNITY_EDITOR
        WaveVR_ControllerLoader.onControllerModelLoaded += internal_controllerLoadedHandler;
#endif
    }

    void OnDisable()
    {
        WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.CONTROLLER_MODEL_LOADED, ControllerLoader);
#if UNITY_EDITOR
        WaveVR_ControllerLoader.onControllerModelLoaded -= internal_controllerLoadedHandler;
#endif
    }

    void ControllerLoader(params object[] args)
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

    void internal_controllerLoadedHandler(GameObject go)
    {
		WVR_DeviceType type=go.GetComponent<ControllerConnectionStateReactor>().type;
		switch(type){
		case WVR_DeviceType.WVR_DeviceType_Controller_Left:
			evntSystem.GetComponent<WaveVR_ControllerInputModule> ().LeftController = go;
			break;
		case WVR_DeviceType.WVR_DeviceType_Controller_Right:
			evntSystem.GetComponent<WaveVR_ControllerInputModule>().RightController=go;
			break;
		default:
			break;
		}
		ControllerLoader (type,go);



        // change controller beam color
        /*
        GameObject beam = null;
        var ch = go.transform.childCount;
        bool found = false;

        for (var i=0; i < ch; i++)
        {
            beam = go.transform.GetChild(i).gameObject;
            if (beam.name == "Beam")
            {
                found = true;
                break;
            }
        }

        if (found)
        {
            var mesh = beam.GetComponent<MeshRenderer>();
            Material[] mat = mesh.materials;

            foreach (var cmat in mat)
            {
                cmat.color = Color.yellow;
            }
        }
        */
    }
 }
