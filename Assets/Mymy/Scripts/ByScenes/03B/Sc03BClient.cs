﻿using DrugVR_Scribe;

public class Sc03BClient : VideoSceneClientBase
{    
    private const DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc04;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = nextSceneToLoad;
    }
}