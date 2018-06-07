using DrugVR_Scribe;
using UnityEngine;
using UnityEngine.Video;

public class Sc07BClient : VideoSceneClientBase
{
    private DrugVR_SceneENUM endSceneToLoad;

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
        endSceneToLoad = Scribe.EndingSceneENUM();

        //ToDo: Different sound cues based on Side05 and Side06


        managerInst.GoToScene(endSceneToLoad);
    }
}