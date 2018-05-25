using DrugVR_Scribe;
using System.Reflection;
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
    private UIFader m_Fader;

    private bool m_SceneChoice = false;


    /* MonoBehaviour */

    private void Start()
    {
        Sprite endingSpriteToUse;
        switch (Scribe.EndingForPlayer)
        {
            case Ending.EndingA:
                endingSpriteToUse = m_EndingASprite;
                break;
            case Ending.EndingB:
                endingSpriteToUse = m_EndingBSprite;
                break;
            case Ending.EndingC:
            default:
                endingSpriteToUse = m_EndingCSprite;
                break;
        }
        m_EndingImage.sprite = endingSpriteToUse;

        StartCoroutine(m_Fader.InterruptAndFadeIn());
    }

    /* end of MonoBehaviour */
}
