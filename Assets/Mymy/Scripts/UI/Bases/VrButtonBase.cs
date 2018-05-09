using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using wvr;

public abstract class VrButtonBase : MonoBehaviour,
    IHandleUiButton
{ 
    [SerializeField]
    private bool m_ResetButtonStateAfterPressed = true;
    // Whether the selection should disappear instantly once it's been clicked.
    [SerializeField]
    private bool m_DisappearOnSelection;
    public bool DisappearOnSelection { get { return m_DisappearOnSelection; } }

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
    protected Button m_Button;
    [SerializeField]
    private EventTrigger m_EventTrigger;

    [SerializeField]
    private WVR_DeviceType m_DeviceToListen = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    [SerializeField]
    private WVR_InputId m_InputToListen = WVR_InputId.WVR_InputId_Alias1_Touchpad;

    private EventSystem m_CurrentEventSystem;

    protected bool m_GazeOver;
    protected bool m_ButtonPressed;

    private bool m_IsForceDisableButton = false;


    /* MonoBehaviour */

    protected virtual void Awake()
    {
        m_CurrentEventSystem = EventSystem.current;
        if (m_Audio == null)
        {
            m_Audio = GetComponent<AudioSource>();
        }
    }

    protected virtual void Update()
    {
        // If this selection is using a UIFader 
        // turn off the "interaction" when it's invisible.
        bool isEnableCollider = !m_IsForceDisableButton && (m_UIFader.Visible && !m_UIFader.Fading);
        SetInteractionEnabled(isEnableCollider);
    }

    /* end of MonoBehaviour */

    public void ForceDisableButton()
    {
        m_IsForceDisableButton = true;
    }

    public void UnforceDisableButton()
    {
        m_IsForceDisableButton = false;
    }

    private void SetInteractionEnabled(bool isEnabled)
    {
        SetIsBlockRaycast(isEnabled);
        m_UIFader.SetIsBlockRaycast(isEnabled);
        m_EventTrigger.enabled = isEnabled;
        m_Button.enabled = isEnabled;
        m_Collider.enabled = isEnabled;
    }

    private void ResetSelectionHoverState()
    {
        // reset hover state of button
        // https://answers.unity.com/questions/883220/how-to-change-selected-button-in-eventsystem-or-de.html
        if (m_CurrentEventSystem)
        {
            m_CurrentEventSystem.SetSelectedGameObject(null);
        }        
    }


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

    private void PlayAudioClip(AudioClip aClip)
    {
        m_Audio.clip = aClip;
        if (m_Audio.clip != null)
        {
            m_Audio.Play();
        }
    }

    public IEnumerator PlayAudioClipAndWaitWhilePlaying(AudioClip aClip)
    {
        PlayAudioClip(aClip);
        yield return WaitWhileAudioIsPlaying();
    }

    private IEnumerator WaitWhileAudioIsPlaying()
    {
        yield return new WaitWhile(() => m_Audio.isPlaying);
    }

    /* end of audios */


    /* exposing UIFader interfaces */

    public IEnumerator WaitForFadeIn()
    {
        yield return StartCoroutine(m_UIFader.WaitForFadeIn());
    }

    public IEnumerator InteruptAndFadeIn()
    {
        yield return StartCoroutine(m_UIFader.InterruptAndFadeIn());
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
        yield return StartCoroutine(m_UIFader.InterruptAndFadeOut());
    }

    public IEnumerator CheckAndFadeOut()
    {
        yield return StartCoroutine(m_UIFader.CheckAndFadeOut());
    }

    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(m_UIFader.FadeOut());
    }

    public void SetIsBlockRaycast(bool isBlockRaycast)
    {
        m_UIFader.SetIsBlockRaycast(isBlockRaycast);
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
        if (m_ResetButtonStateAfterPressed)
        {
            ResetSelectionHoverState();
        }
    }

    public virtual void HandleUp()
    {
        Debug.Log("HandleUp: " + this.name);
        m_ButtonPressed = false;
    }

    /* end of IHandleUiButton interfaces */
}
