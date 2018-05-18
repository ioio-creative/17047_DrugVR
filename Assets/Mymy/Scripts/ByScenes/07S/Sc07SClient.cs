using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc07SClient : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc07B;
    [SerializeField]
    private SelectionStandard m_ExitButton;


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        managerInst.SkyVideoPlayer.isLooping = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        managerInst.SkyVideoPlayer.isLooping = false;
    }

    private void Start()
    {
        if (m_ExitButton != null)
        {
            m_ExitButton.OnSelectionComplete += HandleExitSelected;
        }
    }

    protected override void HandleSystemVideoEnd(VideoPlayer source)
    {
        // This scene won't go to next scene based on Video ends
    }

    private void HandleExitSelected()
    {
        Scribe.Side06 = true;
        GoToSceneOnChoice();
    }

    public void GoToSceneOnChoice()
    {
        if (Scribe.Side06 == false)
        {
            GoToMethScene();
        }
        else
        {
            GoToEndSceneOnChoice();
        }

    }

    private void GoToMethScene()
    {
        managerInst.GoToScene(DrugVR_SceneENUM.Sc07B);
    }

    private void GoToEndSceneOnChoice()
    {
        if (Scribe.Side05 && Scribe.Side06)
        {
            managerInst.GoToScene(DrugVR_SceneENUM.Sc08);               
        }
        else if (Scribe.Side05 || Scribe.Side06)
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
