using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using VRStandardAssets.Utils;

public class Sc03SClient : VideoSceneClientBase
{
    private const DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc04;
    [SerializeField]
    private SelectionStandard[] m_ComicOrMeth;

    private UIFader[] m_ComicAndMethFaders;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = nextSceneToLoad;
        m_ComicAndMethFaders = m_ComicOrMeth.Select( x => x.GetComponent<UIFader>()).ToArray();
        foreach (SelectionStandard vrButton in m_ComicOrMeth)
        {
            vrButton.OnSelectionDown += HandleButtonSelected;
        }
        
    }

    private void GoToSceneOnChoice()
    {
        managerInst.GoToScene(nextSceneToLoad);
    }

    private void HandleButtonSelected()
    {
        foreach (UIFader fader in m_ComicAndMethFaders)
        {
            StartCoroutine(fader.InterruptAndFadeOut());
        }
        StartCoroutine(ButtonsFadeOutCompletedRoutine());
    }

    private IEnumerator ButtonsFadeOutCompletedRoutine()
    {
        yield return StartCoroutine(UIFader.WaitUntilFadersFadedOut(m_ComicAndMethFaders));
        managerInst.PlayVideo();
    }

}
