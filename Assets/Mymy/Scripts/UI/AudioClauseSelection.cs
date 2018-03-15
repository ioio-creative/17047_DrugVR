using System;
using UnityEngine;
using UnityEngine.UI;
using wvr;

[RequireComponent(typeof(AudioSource))]
public class AudioClauseSelection : MonoBehaviour,
    IHandleUiButton
{
    public event Action<AudioClip, Text> OnSelected;


    [SerializeField]
    private bool m_HideOnStart;
    [SerializeField]
    private bool m_IsDisappearOnSelected;
    [SerializeField]
    private GameObject m_SelectionCanvas;
    [SerializeField]
    private AudioClip m_OnOverClip;  
    [SerializeField]
    private AudioClip m_OnSelectedClip;
    [SerializeField]
    private AudioClip m_AudioClauseClip;
    [SerializeField]
    private Text m_AudioClauseText;
    [SerializeField]
    private WVR_DeviceType m_DeviceToListen = WVR_DeviceType.WVR_DeviceType_Controller_Right;    
    [SerializeField]
    private WVR_InputId m_InputToListen = WVR_InputId.WVR_InputId_16;

    private AudioSource m_Audio;
    private bool m_GazeOver;
    private bool m_ButtonPressed;
    private bool m_IsSelectionActive;


    /* MonoBehaviour */

    private void Awake()
    {
        m_Audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (m_HideOnStart)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    /* end of MonoBehaviour */


    public void Show()
    {
        m_SelectionCanvas.SetActive(true);
        m_IsSelectionActive = true;
    }

    public void Hide()
    {
        m_SelectionCanvas.SetActive(false);
        m_IsSelectionActive = false;
    }

    private void PlayOnOverClip()
    {
        m_Audio.clip = m_OnOverClip;
        m_Audio.Play();
    }

    private void PlayOnSelectedClip()
    {
        m_Audio.clip = m_OnSelectedClip;
        m_Audio.Play();
    }

    private void RaiseOnSelectedEvent()
    {
        PlayOnSelectedClip();

        if (OnSelected != null)
        {
            OnSelected(m_AudioClauseClip, m_AudioClauseText);
        }
    }


    /* IHandleUiButton interfaces */

    public void HandleDown()
    {
        Debug.Log("HandleDown: AudioClauseSelection");
        m_ButtonPressed = true;
        
        if (m_GazeOver)
        {
            RaiseOnSelectedEvent();
        }
    }

    public void HandleEnter()
    {
        Debug.Log("HandleEnter: AudioClauseSelection");
        m_GazeOver = true;
        if (m_IsSelectionActive)
        {
            // Play the clip appropriate when the user
            // starts looking at the selection.
            PlayOnOverClip();
        }

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
            RaiseOnSelectedEvent();
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
