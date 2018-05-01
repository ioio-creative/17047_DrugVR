using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc05ABClient : VideoSceneClientBase
{
    [SerializeField]
    private DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc06;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = nextSceneToLoad;
    }
}