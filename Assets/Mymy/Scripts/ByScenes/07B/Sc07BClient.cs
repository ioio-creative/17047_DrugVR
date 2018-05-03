using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc07BClient : VideoSceneClientBase
{
    [SerializeField]
    private DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc09;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = nextSceneToLoad;
    }

    protected override void HandleSystemVideoEnd(VideoPlayer source)
    {
        GoToEndSceneOnChoice();
    }

    public static void GoToEndSceneOnChoice()
    {
        if (Scribe.Side05 || Scribe.Side06)
        {
            //ToDo: Different sound cues based on Side05 and Side06

            managerInst.GoToScene(DrugVR_SceneENUM.Sc09);
        }
        else if (!Scribe.Side05 || !Scribe.Side06)
        {
            managerInst.GoToScene(DrugVR_SceneENUM.Sc10);
        }

    }
}