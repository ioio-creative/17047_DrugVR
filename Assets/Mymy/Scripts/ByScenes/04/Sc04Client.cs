using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc04Client : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc04S;

    private void Awake()
    {
        nextSceneToLoadBase = nextSceneToLoad;
        GameManager.SkyVideoPlayer = GetComponent<VideoPlayer>();
    }
}
