using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class AudioClauseSelected : VrButtonBase    
{
    public event Action<AudioClauseSelected> OnClicked;
    public AudioClauseSelection AudioClause { get { return m_AudioClause; } }
    public bool IsHighLighted { get { return m_IsHighLighted; } }

    [SerializeField]
    private UIFader m_HighLightFader;
    [SerializeField]
    private Image m_ClauseImg;
    [SerializeField]
    private bool m_IsSetClauseImgSpriteToNullWhenStart;

    private AudioClauseSelection m_AudioClause = null;
    private bool m_IsHighLighted = false;


    /* MonoBehaviour */

    private void Start()
    {
        if (m_IsSetClauseImgSpriteToNullWhenStart)
        {
            m_ClauseImg.sprite = null;
        }
    }

    /* end of MonoBehaviour */


    private IEnumerator HandleClicked()
    {
        PlayOnSelectedClip();

        // "restore m_AudioClause back to its original position"
        StartCoroutine(m_AudioClause.InteruptAndFadeIn());
        m_AudioClause = null;

        if (base.DisappearOnSelection)
        {
            yield return StartCoroutine(base.InteruptAndFadeOut());            
        }

        if (OnClicked != null)
        {
            OnClicked(this);
        }

        yield return null;
    }

    public void FillSlotWithAudioClause(
        AudioClauseSelection audioClause)
    {
        m_AudioClause = audioClause;
        m_ClauseImg.sprite = audioClause.ClauseImage.sprite;
        StartCoroutine(base.InteruptAndFadeIn());        
    }

    public void SetHighLight()
    {
        if (!m_IsHighLighted)
        {
            m_IsHighLighted = true;
            StartCoroutine(m_HighLightFader.InterruptAndFadeIn());
        }
    }

    public void UnsetHighLight()
    {
        if (m_IsHighLighted)
        {
            m_IsHighLighted = false;
            StartCoroutine(m_HighLightFader.InterruptAndFadeOut());
        }
    }


    /* IHandleUiButton interfaces */

    public override void HandleDown()
    {
        base.HandleDown();
        if (base.m_GazeOver)
        {
            StartCoroutine(HandleClicked());
        }
    }

    public override void HandleEnter()
    {
        base.HandleEnter();
    }

    public override void HandleExit()
    {
        base.HandleExit();
    }

    public override void HandleUp()
    {
        base.HandleUp();
    }

    /* end of IHandleUiButton interfaces */
}
