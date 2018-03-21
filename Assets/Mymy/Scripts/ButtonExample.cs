using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using wvr;

public class ButtonExample : MonoBehaviour,
    IHandleUiButton
{
    private bool m_GazeOver;
    private bool m_ButtonPressed;


    [SerializeField]
    private bool m_IsDisappearOnSelected;
    [SerializeField]
    private UIFader m_UIFader;
    [SerializeField]
    private Collider m_Collider;
    [SerializeField]
    private Button m_Button;
    [SerializeField]
    private EventTrigger m_EventTrigger;
    [SerializeField]
    private AudioSource m_Audio;
    [SerializeField]
    private AudioClip m_OnOverClip;
    [SerializeField]
    private WVR_DeviceType m_DeviceToListen = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    [SerializeField]
    private WVR_InputId m_InputToListen = WVR_InputId.WVR_InputId_16;



    /* MonoBehaviour */
	
	private void Update()
    {
        // If this selection is using a UIFader 
        // turn off the "interaction" when it's invisible.
        bool isEnableCollider = m_UIFader.Visible && !m_UIFader.Fading;
        SetInteractionEnabled(isEnableCollider);
    }

    /* end of MonoBehaviour */


    private void SetInteractionEnabled(bool isEnabled)
    {
        m_EventTrigger.enabled = isEnabled;
        m_Button.enabled = isEnabled;
        m_Collider.enabled = isEnabled;
    }

    private void PlayOnOverClip()
    {
        m_Audio.clip = m_OnOverClip;
        m_Audio.Play();
    }


    /* IHandleUiButton interfaces */

    public void HandleDown()
    {
        Debug.Log("HandleDown: AudioClauseSelection");
        m_ButtonPressed = true;

        if (m_GazeOver)
        {
            //StartPlayClipsInSequence();
        }
    }

    public void HandleEnter()
    {
        Debug.Log("HandleEnter: AudioClauseSelection");
        m_GazeOver = true;

        // Play the clip appropriate when the user
        // starts looking at the selection.
        PlayOnOverClip();

        // Get button press state from controller device
        if (WaveVR_Controller.Input(m_DeviceToListen).GetPress(m_InputToListen))
        {
            m_ButtonPressed = true;
        }
        else
        {
            m_ButtonPressed = false;
        }

        if (m_ButtonPressed)
        {
            //StartPlayClipsInSequence();
        }
    }

    public void HandleExit()
    {
        Debug.Log("HandleExit: AudioClauseSelection");
        m_GazeOver = false;
    }

    public void HandleUp()
    {
        Debug.Log("HandleUp: AudioClauseSelection");
        m_ButtonPressed = false;
    }

    /* end of IHandleUiButton interfaces */
}
