using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioClauseSelection : VrButtonBase   
{
    public event Action<AudioClauseSelection> OnSelected;
    public Image ClauseImage { get { return m_ClauseImage; } }    
    public AudioClip AudioClauseClip { get { return m_AudioClauseClip; } }
            
    [SerializeField]
    private AudioClip m_AudioClauseClip;
    [SerializeField]
    private Image m_ClauseImage;


    /* MonoBehaviour */

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }
    
    /* end of MonoBehaviour */


    // called in base class's HandleDown()
    protected override void RaiseOnSelectedEvent()
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
        if (m_GazeOver)
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
