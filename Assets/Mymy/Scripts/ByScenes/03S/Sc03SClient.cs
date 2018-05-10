using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using VRStandardAssets.Utils;

public class Sc03SClient : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc04;
    [SerializeField]
    private SelectionStandard[] m_ComicOrMeth;

    private UIFader m_ComicAndMethFader;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = nextSceneToLoad;
        m_ComicAndMethFader = GetComponent<UIFader>();
        foreach (SelectionStandard vrButton in m_ComicOrMeth)
        {
            vrButton.OnSelectionComplete += HandleButtonSelected;
        }
        
    }

    private void GoToSceneOnChoice()
    {
        GameManager.Instance.GoToScene(nextSceneToLoad);
    }

    private void HandleButtonSelected()
    {
        m_ComicAndMethFader.OnFadeOutComplete += HandleButtonFadeOutCompleted;
        StartCoroutine(m_ComicAndMethFader.InterruptAndFadeOut());

    }

    private void HandleButtonFadeOutCompleted()
    {
        GameManager.PlayVideo();
    }

}
