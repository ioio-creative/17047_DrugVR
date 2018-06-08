using DrugVR_Scribe;
using UnityEngine;

public class Sc06Client : VideoSceneClientBase
{    
    private const DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc06A;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = nextSceneToLoad;
    }
}
