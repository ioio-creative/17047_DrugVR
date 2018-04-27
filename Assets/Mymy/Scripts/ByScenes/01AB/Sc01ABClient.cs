using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc01ABClient : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc02A;

    private void Awake()
    {
        nextSceneToLoadBase = nextSceneToLoad;
        GameManager.SkyVideoPlayer = GetComponent<VideoPlayer>();
    }
}
