using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc01Client : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc01S;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        nextSceneToLoadBase = nextSceneToLoad;
    }
}
