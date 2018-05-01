using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// This class is similar to AudioClauseSelected, but simpler
public class BossAudioClauseSelected : VrButtonBase
{
    public event Action OnClicked;
    public AudioClauseSelection AudioClause { get { return m_AudioClause; } }

    [SerializeField]
    private Image m_ClauseImg;

    private AudioClauseSelection m_AudioClause = null;
    private bool m_IsHighLighted = false;

    private IEnumerator HandleClicked()
    {
        PlayOnSelectedClip();

        // "restore m_AudioClause back to its original position"
        StartCoroutine(m_AudioClause.InteruptAndFadeIn());
        m_AudioClause = null;

        if (base.IsDisappearOnSelected)
        {
            yield return StartCoroutine(base.InteruptAndFadeOut());
        }

        if (OnClicked != null)
        {
            OnClicked();
        }

        yield return null;
    }

    public void FillSlotWithAudioClause(
        AudioClauseSelection audioClause)
    {
        m_IsHighLighted = false;
        m_AudioClause = audioClause;
        m_ClauseImg.sprite = audioClause.ClauseImage.sprite;
        StartCoroutine(base.InteruptAndFadeIn());
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
