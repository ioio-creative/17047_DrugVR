using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc02ABClient : VideoSceneClientBase
{
    private const DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc02S;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = nextSceneToLoad;
    }
}
