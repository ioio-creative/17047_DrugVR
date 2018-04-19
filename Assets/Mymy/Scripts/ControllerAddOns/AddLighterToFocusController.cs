using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddLighterToFocusController : MonoBehaviour
{
    public WaveVR_PoseTrackerManager ControllerPosTrkMan;
    public GameObject OriginalControllerModel;
    public LaserPointer ControllerLaser;

    private WaveVR_ControllerPoseTracker controllerPT;

    void Start ()
    {
        controllerPT = ControllerPosTrkMan.gameObject.GetComponent<WaveVR_ControllerPoseTracker>();
        controllerPT.TrackRotation = false;
        gameObject.transform.parent = ControllerPosTrkMan.transform;
        ControllerLaser.enabled = false;
        ControllerLaser.EnableReticle = false;
        OriginalControllerModel.SetActive(false);
        OriginalControllerModel.transform.localScale = Vector3.zero;

    }

    private void OnDestroy()
    {
        controllerPT.TrackRotation = true;
        OriginalControllerModel.SetActive(true);
        OriginalControllerModel.transform.localScale = Vector3.one;
        ControllerLaser.enabled = true;
        ControllerLaser.EnableReticle = true;
    }
}
