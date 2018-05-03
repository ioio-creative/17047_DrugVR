using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using wvr;

public class PartySelection : VrButtonBase
{
    public event Func<GameObject, IEnumerator> OnSelected;
    public AudioClip PartySFXAudioClip { get { return m_PartySFXAudioClip; } }

    [SerializeField]
    private AudioClip m_PartySFXAudioClip;


    /* MonoBehaviour */ 

    protected override void Update()
    {
        base.Update();
    }

    /* end of MonoBehaviour */

    private void RaiseOnSelectedEvent()
    {
        if (OnSelected != null)
        {
            StartCoroutine(OnSelected(this.gameObject));
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
