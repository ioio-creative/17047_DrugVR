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
        m_SceneChoice = Scribe.GetSideValueByName(m_SceneChoiceName);

        Sprite spriteToUse;
        string strToUse;

        if (m_SceneChoice)
        {
            spriteToUse = m_TrueChoiceSprite;
            strToUse = m_TrueChoiceStr;
        }
        else
        {
            spriteToUse = m_FalseChoiceSprite;
            strToUse = m_FalseChoiceStr;
        }

        m_ChoiceImage.sprite = spriteToUse;
        m_ChoiceText.text = strToUse.Replace(m_NewLineSymbol, Environment.NewLine);

        StartCoroutine(m_Fader.InterruptAndFadeIn());
    }

    /* end of MonoBehaviour */
}
