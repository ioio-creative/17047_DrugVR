using DrugVR_Scribe;
using TMPro;
using UnityEngine;
using VRStandardAssets.Utils;

public class OutroResultText : MonoBehaviour
{
    [SerializeField]
    private string[] m_SceneChoiceNames;
    [SerializeField]
    private UIFader m_Fader;
    [SerializeField]
    private TextMeshProUGUI m_ResultText;    


    /* MonoBehaviour */

    private void Start()
    {
        m_ResultText.text = Scribe.SummaryStr;
        StartCoroutine(m_Fader.InterruptAndFadeIn());
	}

    /* end of MonoBehaviour */  
}
