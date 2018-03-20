using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using wvr;

[RequireComponent(typeof(AudioSource))]
public class AudioSentenceControl : MonoBehaviour,
    IHandleUiButton
{
    [SerializeField]
    private bool m_HideOnStart;
    [SerializeField]
    private bool m_IsDisappearOnSelected;
    [SerializeField]
    private GameObject m_SelectionCanvas;
    [SerializeField]
    private AudioClip m_OnOverClip;
    [SerializeField]
    private GameObject m_ClauseAvailableOptionsContainer;
    [SerializeField]
    private GameObject m_SelectedClauseContainer;
    [SerializeField]
    private WVR_DeviceType m_DeviceToListen = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    [SerializeField]
    private WVR_InputId m_InputToListen = WVR_InputId.WVR_InputId_16;


    private AudioClauseSelection[] m_ClauseAvailableOptions;
    private AudioClauseSelected[] m_SelectedClauseSeq;  // fixed in length
    private int m_NumOfClausesRequiredInSentence;

    private AudioClauseSelected m_FirstNotYetFilledAudioClauseSlot
    {
        get
        {            
            return m_SelectedClauseSeq.FirstOrDefault(x => x.AudioClause == null);
        }
    }

    private AudioSource m_Audio;
    private bool m_GazeOver;
    private bool m_ButtonPressed;
    private bool m_IsSelectionActive;
    private Coroutine m_PlayClipsInSeqRoutine = null;


    /* MonoBehaviour */

    private void Awake()
    {        
        m_Audio = GetComponent<AudioSource>();
        m_ClauseAvailableOptions =
            m_ClauseAvailableOptionsContainer.GetComponentsInChildren<AudioClauseSelection>();

        m_SelectedClauseSeq = m_SelectedClauseContainer.GetComponentsInChildren<AudioClauseSelected>();
        m_NumOfClausesRequiredInSentence = m_SelectedClauseSeq.Length;
    }

    private void OnEnable()
    {
		foreach (AudioClauseSelection option in m_ClauseAvailableOptions)
        {
            option.OnSelected += HandleClauseOptionSelected;
        }
	}

    private void OnDisable()
    {
        foreach (AudioClauseSelection option in m_ClauseAvailableOptions)
        {
            option.OnSelected -= HandleClauseOptionSelected;
        }
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


    private void Show()
    {
        m_SelectionCanvas.SetActive(true);
        m_IsSelectionActive = true;
    }

    private void Hide()
    {
        m_SelectionCanvas.SetActive(false);
        m_IsSelectionActive = false;
    }

    private void PlayOnOverClip()
    {
        m_Audio.clip = m_OnOverClip;
        m_Audio.Play();
    }

    private void StartPlayClipsInSequence()
    {
        //if (m_PlayClipsInSeqRoutine == null &&
        //    m_SelectedClauseClipSeq.Length == m_NumOfClausesRequiredInSentence)
        //{
        //    m_PlayClipsInSeqRoutine =
        //        StartCoroutine(PlayClipsInSequence());
        //}
    }

    private IEnumerator PlayClipsInSequence()
    {
        //foreach (AudioClip clip in m_SelectedClauseClipSeq)
        //{
        //    m_Audio.clip = clip;
        //    m_Audio.Play();
        //    yield return new WaitWhile(() => m_Audio.isPlaying);
        //}

        //m_PlayClipsInSeqRoutine = null;

        yield return null;
    }


    /* AudioClauseSelection.OnSelected() event handler */

    private void HandleClauseOptionSelected(AudioClauseSelection audioClauseSelected)
    {
        AudioClauseSelected firstNotYetFilledAudioClauseSlot = 
            m_FirstNotYetFilledAudioClauseSlot;

        if (firstNotYetFilledAudioClauseSlot)
        {
            firstNotYetFilledAudioClauseSlot.FillSlotWithAudioClause(audioClauseSelected);
            if (audioClauseSelected.IsDisappearOnSelected)
            {

            }
        }
    }

    /* end of AudioClauseSelection.OnSelected(AudioClip) event handler */


    /* IHandleUiButton interfaces */

    public void HandleDown()
    {
        Debug.Log("HandleDown: AudioClauseSelection");
        m_ButtonPressed = true;

        if (m_GazeOver)
        {
            StartPlayClipsInSequence();
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
            StartPlayClipsInSequence();
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
