using DrugVR_Scribe;
using UnityEngine;
using UnityEngine.Video;

public class Sc07BClient : VideoSceneClientBase
{
    private const DrugVR_SceneENUM normalEndSceneToLoad = DrugVR_SceneENUM.Sc09;
    private const DrugVR_SceneENUM badEndSceneToLoad = DrugVR_SceneENUM.Sc10;

    protected override void Awake()
    {
        base.Awake();       
    }

    protected override void HandleSystemVideoEnd(VideoPlayer source)
    {
        GoToEndSceneOnChoice();
    }

    public void GoToEndSceneOnChoice()
    {
        DrugVR_SceneENUM endSceneToLoad = normalEndSceneToLoad;
        if (Scribe.Side05 || Scribe.Side06)
        {
            //ToDo: Different sound cues based on Side05 and Side06
            endSceneToLoad = normalEndSceneToLoad;
        }
        else if (!Scribe.Side05 || !Scribe.Side06)
        {
            endSceneToLoad = badEndSceneToLoad;
        }

        managerInst.GoToScene(endSceneToLoad);
    }
}