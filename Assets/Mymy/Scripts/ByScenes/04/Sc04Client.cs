using DrugVR_Scribe;
using UnityEngine;

public class Sc04Client : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc04S;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = nextSceneToLoad;
    }

    public static void GoToSceneOnChoice()
    {
        //if (Scribe.Side04)
        //{
        //    managerInst.GoToScene(DrugVR_SceneENUM.Sc01A);
        //}
        //else
        //{
        //    managerInst.GoToScene(DrugVR_SceneENUM.Sc01B);
        //}
    }
}
