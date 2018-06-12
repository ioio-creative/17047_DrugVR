using DrugVR_Scribe;
using System;
using TMPro;
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
    private string m_NewLineSymbol;
    [SerializeField]
    private TextMeshProUGUI m_ChoiceText;
    [SerializeField]
    private string m_TrueChoiceStr;
    [SerializeField]
    private string m_FalseChoiceStr;

    [SerializeField]
    private UIFader m_Fader;

    private bool m_SceneChoice = false;


    /* MonoBehaviour */

    private void Start()
    {
        //Double check m_SceneChoiceName DOES NOT set to "Side02" in editor:
        //Side02 (text message reply) doesn't affect major narrative change
        m_SceneChoice = Scribe.GetSideValueByName(m_SceneChoiceName);

        m_ChoiceImage.sprite = m_SceneChoice ?
            m_TrueChoiceSprite : m_FalseChoiceSprite;

        m_ChoiceText.text = (m_SceneChoice ?
            m_TrueChoiceStr : m_FalseChoiceStr)
            .Replace(m_NewLineSymbol, Environment.NewLine);
        
        StartCoroutine(m_Fader.InterruptAndFadeIn());
    }

    /* end of MonoBehaviour */
}
