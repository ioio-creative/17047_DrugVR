using UnityEngine;
using VRStandardAssets.Utils;

public class Scene4ATempStoryStartControl : MonoBehaviour
{
    [SerializeField]
    private GameObject m_FadersToStartContainer;


    private UIFader[] m_FadersToStart;


    private void Awake()
    {
        m_FadersToStart =
            m_FadersToStartContainer.GetComponentsInChildren<UIFader>();
    }

    private void Start()
    {
        foreach (UIFader fader in m_FadersToStart)
        {
            StartCoroutine(fader.InteruptAndFadeIn());
        }
    }
}
