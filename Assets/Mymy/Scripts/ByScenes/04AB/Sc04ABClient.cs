using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc04ABClient : VideoSceneClientBase
{
    private const DrugVR_SceneENUM nextSceneToLoadWhenSide02IsTrue = 
        DrugVR_SceneENUM.Sc05A;
    private const DrugVR_SceneENUM nextSceneToLoadWhenSide02IsFalse = 
        DrugVR_SceneENUM.Sc05B;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = Scribe.Side02 ? nextSceneToLoadWhenSide02IsTrue : nextSceneToLoadWhenSide02IsFalse;
    }
}