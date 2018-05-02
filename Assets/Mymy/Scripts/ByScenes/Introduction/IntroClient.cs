using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroClient : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc01;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        nextSceneToLoadBase = nextSceneToLoad;
    }
}
