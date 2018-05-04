﻿using DrugVR_Scribe;
using UnityEngine;

public class AddLighterToFocusController : MonoBehaviour
{
    private const string ControllerPosTrkManGameObjectName = "/VIVEFocusWaveVR/FocusController";
    private const string OriginalControllerModelGameObjectName = "/VIVEFocusWaveVR/FocusController/MIA_Ctrl";

    private GameObject controllerPosTrkMan;
    private GameObject originalControllerModel;
    private LaserPointer controllerLaser;

    private WaveVR_ControllerPoseTracker controllerPT;

    private void Start ()
    {
        GameManager.OnSceneChange += HandleSceneChange;

        controllerPosTrkMan = GameObject.Find(ControllerPosTrkManGameObjectName);
        originalControllerModel = GameObject.Find(OriginalControllerModelGameObjectName);
        controllerLaser = FindObjectOfType<LaserPointer>();

        controllerPT = controllerPosTrkMan.GetComponent<WaveVR_ControllerPoseTracker>();
        controllerPT.TrackRotation = false;
        gameObject.transform.parent = controllerPosTrkMan.transform;
        controllerLaser.enabled = false;
        controllerLaser.IsEnableReticle = false;
        originalControllerModel.SetActive(false);
        originalControllerModel.transform.localScale = Vector3.zero;        
    }

    private void OnDisable()
    {
        controllerPT.TrackRotation = true;
        originalControllerModel.SetActive(true);
        originalControllerModel.transform.localScale = Vector3.one;
        controllerLaser.enabled = true;
        controllerLaser.IsEnableReticle = true;
    }

    private void HandleSceneChange(DrugVR_SceneENUM nextScene)
    {
        enabled = false;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
