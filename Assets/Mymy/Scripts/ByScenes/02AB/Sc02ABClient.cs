using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc02ABClient : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc02S;

    private void Awake()
    {
        nextSceneToLoadBase = nextSceneToLoad;
        GameManager.SkyVideoPlayer = GetComponent<VideoPlayer>();
    }
}
