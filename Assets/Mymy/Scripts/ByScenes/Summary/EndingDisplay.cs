using DrugVR_Scribe;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class EndingDisplay : MonoBehaviour
{
    [SerializeField]
    private Image m_EndingImage;
    [SerializeField]
    private Sprite m_EndingASprite;
    [SerializeField]
    private Sprite m_EndingBSprite;
    [SerializeField]
    private Sprite m_EndingCSprite;

    [SerializeField]
    private string m_NewLineSymbol;
    [SerializeField]
    private TextMeshProUGUI m_EndingText;
    [SerializeField]
    private string m_EndingAStr;
    [SerializeField]
    private string m_EndingBStr;
    [SerializeField]
    private string m_EndingCStr;

    [SerializeField]
    private UIFader m_Fader;

    private bool m_SceneChoice = false;


    /* MonoBehaviour */

    private void Start()
    {
        Sprite endingSpriteToUse;
        string endingStrToUse;

        switch (Scribe.EndingForPlayer)
        {
            case Ending.EndingA:
                endingSpriteToUse = m_EndingASprite;
                endingStrToUse = m_EndingAStr;
                break;
            case Ending.EndingB:
                endingSpriteToUse = m_EndingBSprite;
                endingStrToUse = m_EndingBStr;
                break;
            case Ending.EndingC:
            default:
                endingSpriteToUse = m_EndingCSprite;
                endingStrToUse = m_EndingCStr;
                break;
        }

        m_EndingImage.sprite = endingSpriteToUse;
        m_EndingText.text = endingStrToUse.Replace(m_NewLineSymbol, Environment.NewLine);

        StartCoroutine(m_Fader.InterruptAndFadeIn());
    }

    /* end of MonoBehaviour */
}
