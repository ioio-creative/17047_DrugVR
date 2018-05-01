using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc04ABClient : VideoSceneClientBase
{
    [SerializeField]
    private DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc05A;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoad = Scribe.Side02 ? DrugVR_SceneENUM.Sc05A : DrugVR_SceneENUM.Sc05B;
        nextSceneToLoadBase = nextSceneToLoad;
    }
}