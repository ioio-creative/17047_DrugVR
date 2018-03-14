using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddLighterToFocusController : MonoBehaviour
{
    public MY_WaveVR_ControllerPoseTracker FocusControllerGO;
    public GameObject OriginalControllerModel;
    public LaserPointer ControllerLaser;
    
    // Use this for initialization
	void Awake ()
    {
        gameObject.transform.parent = FocusControllerGO.transform;
        OriginalControllerModel.SetActive(false);
        ControllerLaser.enabled = false;
        FocusControllerGO.lockRotation = true;
        OriginalControllerModel.transform.localScale = Vector3.zero;

    }

    private void OnDestroy()
    {
        OriginalControllerModel.SetActive(true);
        OriginalControllerModel.transform.localScale = Vector3.one;
        ControllerLaser.enabled = true;
        FocusControllerGO.lockRotation = false;
    }
}
