using UnityEngine;
using VRStandardAssets.Utils;

public class SummaryArrowDisplay : MonoBehaviour
{
    [SerializeField]
    private UIFader m_Fader;

	private void Start()
    {
        StartCoroutine(m_Fader.FadeIn());
	}	
}
