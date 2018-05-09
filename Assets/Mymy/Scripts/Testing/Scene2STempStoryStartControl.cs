using UnityEngine;
using VRStandardAssets.Utils;

public class Scene2STempStoryStartControl : MonoBehaviour
{
    [SerializeField]
    private UIFader m_FaderToStart; 


	private void Start()
    {
        StartCoroutine(m_FaderToStart.InterruptAndFadeIn());
	}
}
