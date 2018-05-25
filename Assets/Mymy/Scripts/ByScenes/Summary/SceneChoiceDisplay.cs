using DrugVR_Scribe;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class SceneChoiceDisplay : MonoBehaviour
{
    // example, refer to Scribe.cs
    [SerializeField]
    private string m_SceneChoiceName = "Side01";
    [SerializeField]
    private Image m_ChoiceImage;
    [SerializeField]
    private Sprite m_TrueChoiceSprite;
    [SerializeField]
    private Sprite m_FalseChoiceSprite;
    [SerializeField]
    private UIFader m_Fader;

    private bool m_SceneChoice = false;


    /* MonoBehaviour */

    private void Start()
    {       
        m_SceneChoice = Scribe.GetSideValueByName(m_SceneChoiceName);

        m_ChoiceImage.sprite = m_SceneChoice ?
            m_TrueChoiceSprite : m_FalseChoiceSprite;

        StartCoroutine(m_Fader.InterruptAndFadeIn());
    }

    /* end of MonoBehaviour */
}
