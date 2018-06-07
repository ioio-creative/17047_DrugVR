using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc07SClient : VideoSceneClientBase
{
    private const DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc07B;
    [SerializeField]
    private SelectionStandard m_ExitButton;
    private UIImageAnimationControl m_ExitUIAnimCtrl;


    protected override void Awake()
    {
        base.Awake();
        m_ExitUIAnimCtrl = m_ExitButton.GetComponentInChildren<UIImageAnimationControl>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        managerInst.SkyVideoPlayer.isLooping = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (managerInst)
        {
            //prevent exception when quit app during this scene and 
            managerInst.SkyVideoPlayer.isLooping = false;
        }
    }

    private void Start()
    {
        if (m_ExitButton != null)
        {
            DeactivateExitButton();
        }
        
    }

    public void ActivateExitButton()
    {
        m_ExitButton.UnforceDisableButton();
        m_ExitUIAnimCtrl.ActivateUIAnimation();
        m_ExitButton.OnSelectionComplete += HandleExitSelected;
        StartCoroutine(m_ExitButton.InterruptAndFadeIn());
    }

    public void DeactivateExitButton()
    {
        m_ExitButton.ForceDisableButton();
        m_ExitUIAnimCtrl.DeactivateUIAnimation();
        m_ExitButton.OnSelectionComplete -= HandleExitSelected;
        StartCoroutine(m_ExitButton.InterruptAndFadeOut());
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
        switch (Scribe.EndingForPlayer)
        {
            case Ending.EndingA:
                managerInst.GoToScene(DrugVR_SceneENUM.Sc08);
                break;
            case Ending.EndingB:
                //ToDo: Different sound cues based on Side05 and Side06
                managerInst.GoToScene(DrugVR_SceneENUM.Sc09);
                break;
            case Ending.EndingC:
            default:
                managerInst.GoToScene(DrugVR_SceneENUM.Sc10);
                break;
        }
    }
}
