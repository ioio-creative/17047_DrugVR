using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc07Client : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc07S;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        nextSceneToLoadBase = nextSceneToLoad;
    }
}
