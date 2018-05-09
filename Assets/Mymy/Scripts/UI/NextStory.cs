using System.Collections;
using UnityEngine;
using VRStandardAssets.Utils;

public class NextStory : MonoBehaviour
{
    [SerializeField]
    private UIFader lastUiFader;

    [SerializeField]
    private SelectionProgress lastSelectionProgress;

    [SerializeField]
    private UIFader nextUiFader;

    [SerializeField]
    private SelectionProgress nextSelectionProgress;

    private void OnEnable()
    {
        lastSelectionProgress.OnSelectionComplete += HandleEventComplete;
    }

    private void OnDisable()
    {
        lastSelectionProgress.OnSelectionComplete -= HandleEventComplete;
    }

    private void HandleEventComplete()
    {
        StartCoroutine(InitialiseNextStory());
    }

    private IEnumerator InitialiseNextStory()
    {
        yield return StartCoroutine(lastUiFader.InterruptAndFadeOut());
        yield return StartCoroutine(nextUiFader.InterruptAndFadeIn());        
    }
}
