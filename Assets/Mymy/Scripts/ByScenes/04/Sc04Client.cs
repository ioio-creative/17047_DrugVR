using DrugVR_Scribe;
using UnityEngine;

public class Sc04Client : VideoSceneClientBase
{
    private const DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc04S;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = nextSceneToLoad;
    }
}
