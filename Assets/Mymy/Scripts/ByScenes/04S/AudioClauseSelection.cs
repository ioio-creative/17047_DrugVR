using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioClauseSelection : VrButtonBase   
{
    public event Action<AudioClauseSelection> OnSelected;
    public AudioClip AudioClauseWithToneClip { get { return m_AudioClauseWithToneClip; } }
    public Image ClauseImage { get { return m_ClauseImage; } }    
    public bool IsBossClause { get { return m_IsBossClause; } }
                
    [SerializeField]
    private AudioClip m_AudioClauseWithToneClip;
    [SerializeField]
    private Image m_ClauseImage;
    [SerializeField]
    private bool m_IsBossClause;


    /* MonoBehaviour */

    /* end of MonoBehaviour */


    private void RaiseOnSelectedEvent()
    {
        if (OnSelected != null)
        {
            OnSelected(this);
        }
    }


    /* IHandleUiButton interfaces */

    public override void HandleDown()
    {
        base.HandleDown();
        if (base.m_GazeOver)
        {
            RaiseOnSelectedEvent();
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
