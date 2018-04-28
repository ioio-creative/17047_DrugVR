using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using wvr;

public abstract class VrButtonBase : MonoBehaviour,
    IHandleUiButton
{
    public bool IsDisappearOnSelected { get { return m_IsDisappearOnSelected; } }

    [SerializeField]
    private bool m_IsDisappearOnSelected;

    [SerializeField]
    private AudioSource m_Audio;
    // The clip to play when the user looks at the bar.
    [SerializeField]
    private AudioClip m_OnOverClip;
    [SerializeField]
    private AudioClip m_OnSelectedClip;
    [SerializeField]
    private AudioClip m_OnErrorClip;


    // Optional reference to a UIFader, used if the Selection needs to fade out.
    [SerializeField]
    private UIFader m_UIFader;
    // Optional reference to the Collider 
    // used to "block" the user's gaze, 
    // turned off when the UIFader is not visible.
    [SerializeField]
    private Collider m_Collider;

    [SerializeField]
    private Button m_Button;
    [SerializeField]
    private EventTrigger m_EventTrigger;

    [SerializeField]
    private WVR_DeviceType m_DeviceToListen = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    [SerializeField]
    private WVR_InputId m_InputToListen = WVR_InputId.WVR_InputId_16;

    private EventSystem m_CurrentEventSystem;

    protected bool m_GazeOver;
    protected bool m_ButtonPressed;


    /* MonoBehaviour */

    protected virtual void Awake()
    {
        m_CurrentEventSystem = EventSystem.current;
    }

    protected virtual void Update()
    {
        // If this selection is using a UIFader 
        // turn off the "interaction" when it's invisible.
        bool isEnableCollider = m_UIFader.Visible && !m_UIFader.Fading;
        SetInteractionEnabled(isEnableCollider);
        Debug.Log("is enable collider: " + isEnableCollider);
    }

    /* end of MonoBehaviour */


    private void SetInteractionEnabled(bool isEnabled)
    {
        m_EventTrigger.enabled = isEnabled;
        m_Button.enabled = isEnabled;
        m_Collider.enabled = isEnabled;
    }

    private void ResetSelectionHoverState()
    {
        // reset hover state of button
        // https://answers.unity.com/questions/883220/how-to-change-selected-button-in-eventsystem-or-de.html
        m_CurrentEventSystem.SetSelectedGameObject(null);
    }

    protected abstract void RaiseOnSelectedEvent();


    /* audios */

    public void PlayOnOverClip()
    {
        PlayAudioClip(m_OnOverClip);        
    }

    public void PlayOnSelectedClip()
    {
        PlayAudioClip(m_OnSelectedClip);
    }

    public void PlayOnErrorClip()
    {
        PlayAudioClip(m_OnErrorClip);        
    }

    public void PlayAudioClip(AudioClip aClip)
    {
        m_Audio.clip = aClip;
        m_Audio.Play();
    }

    /* end of audios */


    /* exposing UIFader interfaces */

    public IEnumerator WaitForFadeIn()
    {
        yield return StartCoroutine(m_UIFader.WaitForFadeIn());
    }

    public IEnumerator InteruptAndFadeIn()
    {
        yield return StartCoroutine(m_UIFader.InteruptAndFadeIn());
    }

    public IEnumerator CheckAndFadeIn()
    {
        yield return StartCoroutine(m_UIFader.CheckAndFadeIn());
    }

    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(m_UIFader.FadeIn());
    }

    public IEnumerator WaitForFadeOut()
    {
        yield return StartCoroutine(m_UIFader.WaitForFadeOut());
    }

    public IEnumerator InteruptAndFadeOut()
    {
        yield return StartCoroutine(m_UIFader.InteruptAndFadeOut());
    }

    public IEnumerator CheckAndFadeOut()
    {
        yield return StartCoroutine(m_UIFader.CheckAndFadeOut());
    }

    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(m_UIFader.FadeOut());
    }

    /* end of exposing UIFader interfaces */


    /* IHandleUiButton interfaces */

    public virtual void HandleDown()
    {
        Debug.Log("HandleDown: " + this.name);
        m_ButtonPressed = true;        
    }

    public virtual void HandleEnter()
    {
        Debug.Log("HandleEnter: " + this.name);
        m_GazeOver = true;

        // Play the clip appropriate when the user
        // starts looking at the selection.
        PlayOnOverClip();

        // Get button press state from controller device
        //if (WaveVR_Controller.Input(m_DeviceToListen).GetPress(m_InputToListen))
        //{
        //    m_ButtonPressed = true;
        //}
        //else
        //{
        //    m_ButtonPressed = false;
        //}

        //if (m_ButtonPressed)
        //{
        //    RaiseOnSelectedEvent();
        //}
    }

    public virtual void HandleExit()
    {
        Debug.Log("HandleExit: " + this.name);
        m_GazeOver = false;
        ResetSelectionHoverState();
    }

    public virtual void HandleUp()
    {
        Debug.Log("HandleUp: " + this.name);
        m_ButtonPressed = false;
    }

    /* end of IHandleUiButton interfaces */
}
