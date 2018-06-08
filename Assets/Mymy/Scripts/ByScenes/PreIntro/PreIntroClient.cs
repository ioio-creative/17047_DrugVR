using DrugVR_Scribe;
using UnityEngine;
using VRStandardAssets.Utils;

public class PreIntroClient : MonoBehaviour
{
    private const DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Intro;

    [SerializeField]
    private UIFader[] m_FadersToStart;

    [SerializeField]
    private SelectionStandard m_StartButton;

    
    /* MonoBehaviour */

    private void OnEnable()
    {
        m_StartButton.OnSelectionDown += HandleStartButtonSelectionComplete;
    }

    private void OnDisable()
    {
        m_StartButton.OnSelectionDown -= HandleStartButtonSelectionComplete;
    }

    private void Start()
    {
        foreach (UIFader fader in m_FadersToStart)
        {
            StartCoroutine(fader.InterruptAndFadeIn());
        }
    }

    /* end of MonoBehaviour */


    /* event handlers */

    private void HandleStartButtonSelectionComplete()
    {
        GameManager.Instance.GoToScene(nextSceneToLoad);
    }

    /* end of event handlers */
}
