using System.Collections;
using UnityEngine;
using VRStandardAssets.Utils;

public class StoryStartControl : MonoBehaviour
{
    [SerializeField]
    private float m_SecondsToWaitBeforeStart;

    [SerializeField]
    private UIFader m_StartFader;

    [SerializeField]
    private SelectionProgress m_StartSelectionProgess;

    private IEnumerator Start()
    {
        //InputTracking.Recenter();

        yield return new WaitForSeconds(m_SecondsToWaitBeforeStart);
        
        yield return StartCoroutine(m_StartFader.InteruptAndFadeIn());
        
        yield return StartCoroutine(m_StartSelectionProgess.WaitForSelectionToFill());
        Debug.Log("Start selection progress completes.");
        //yield return StartCoroutine(m_StartFader.InteruptAndFadeOut());
    }
}
