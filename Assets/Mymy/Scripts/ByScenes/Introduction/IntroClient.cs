using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroClient : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc01;

    private void Awake()
    {
        nextSceneToLoadBase = nextSceneToLoad;
        GameManager.SkyVideoPlayer = GetComponent<VideoPlayer>();
    }
}
