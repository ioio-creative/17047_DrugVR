using DrugVR_Scribe;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class AudioSentenceControl : VrButtonBase
{
    public bool IsBossOptionSelected { get { return m_IsBossOptionSelected; } }

    [SerializeField]
    private AudioClip m_Answer1FullClip;
    [SerializeField]
    private AudioClip m_Answer2FullClip;

    [SerializeField]
    private Sc04SClient m_Sc04SClient;
    [SerializeField]
    private Sprite m_PlayClipsBtnNormalSprite;
    [SerializeField]
    private Sprite m_PlayClipsBtnHighLightSprite;
    [SerializeField]
    private Image m_PlayClipsBtnImage;
    [SerializeField]
    private bool m_IsSelectionsDisappearOnSelectedOverride;
    [SerializeField]
    private GameObject m_HighlightSlotsContainer;
    [SerializeField]
    private GameObject m_ClauseAvailableOptionsContainer;
    [SerializeField]
    private GameObject m_SelectedClauseContainer;
    [SerializeField]
    private BossAudioClauseSelected m_BossSelectedClause;
    [SerializeField]
    private UIFader m_NormalSelectedClauseTextBgFader;

    private int m_HighlightedSlotFaderIdx;
    private UIFader[] m_HighlightSlotFaders;
    private AudioClauseSelection[] m_ClauseAvailableOptions;
    private AudioClauseSelected[] m_SelectedClauseSeq;  // fixed in length
    private int m_NumOfClausesRequiredInSentence;    

    

    private Coroutine m_PlayClipsInSeqRoutine = null;

    private bool m_IsBossOptionSelected = false;


    /* MonoBehaviour */

    protected override void Awake()
    {
        m_HighlightSlotFaders =
            m_HighlightSlotsContainer.GetComponentsInChildren<UIFader>();

        m_ClauseAvailableOptions =
            m_ClauseAvailableOptionsContainer.GetComponentsInChildren<AudioClauseSelection>();

        m_SelectedClauseSeq = m_SelectedClauseContainer.GetComponentsInChildren<AudioClauseSelected>();
        m_NumOfClausesRequiredInSentence = m_SelectedClauseSeq.Length;
    }

    private void Start()
    {
        HighlightNextAudioClauseToFill();
    }

    private void OnEnable()
    {
        foreach (AudioClauseSelection option in m_ClauseAvailableOptions)
        {
            option.OnSelected += HandleClauseOptionSelected;
        }

        foreach (AudioClauseSelected slot in m_SelectedClauseSeq)
        {
            slot.OnClicked += HandleClauseSlotOnClicked;
        }

        m_BossSelectedClause.OnClicked += HandleBossClauseSlotOnClicked;
    }

    private void OnDisable()
    {
        foreach (AudioClauseSelection option in m_ClauseAvailableOptions)
        {
            option.OnSelected -= HandleClauseOptionSelected;
        }

        foreach (AudioClauseSelected slot in m_SelectedClauseSeq)
        {
            slot.OnClicked -= HandleClauseSlotOnClicked;
        }

        m_BossSelectedClause.OnClicked -= HandleBossClauseSlotOnClicked;
    }

    /* end of MonoBehaviour */
    private AudioClauseSelected GetFirstNotYetFilledAudioClauseSlot()
    {
        return m_SelectedClauseSeq.FirstOrDefault(x => x.AudioClause == null);
    }

    private void HighlightNextAudioClauseToFill()
    {
        bool isThereMoreSlotsToFill = false;

        AudioClauseSelected nextAudioClauseSlotToFill = GetFirstNotYetFilledAudioClauseSlot();
        if (nextAudioClauseSlotToFill != null)
        {
            isThereMoreSlotsToFill = true;
            nextAudioClauseSlotToFill.SetHighLight();
            foreach (AudioClauseSelected slot in m_SelectedClauseSeq)
            {
                if (slot != nextAudioClauseSlotToFill)  // && slot.AudioClause == null)
                {
                    slot.UnsetHighLight();
                }
            }
        }        

        if (isThereMoreSlotsToFill)
        {            
            UnHighLightPlayClipsBtn();
        }
        else
        {
            HighLightPlayClipsBtn();
        }        
    }


    /* audios */

    private IEnumerator StartPlayClipsInSequence()
    {
        // boss option not selected yet
        if (!m_IsBossOptionSelected)
        {
            // if the coroutine has not been started
            // and all audio clause slots are filled
            if (m_PlayClipsInSeqRoutine == null &&
                GetFirstNotYetFilledAudioClauseSlot() == null)
            {
                m_PlayClipsInSeqRoutine =
                    StartCoroutine(PlayClipsInSequence());  
            }
        }
        // boss option selected already
        else
        {
            AudioClip clipToPlay = m_BossSelectedClause.AudioClause.AudioClauseWithToneClip;
            m_PlayClipsInSeqRoutine = 
                StartCoroutine(base.PlayAudioClipAndWaitWhilePlaying(clipToPlay));
        }

        yield return m_PlayClipsInSeqRoutine;
    }

    private IEnumerator PlayClipsInSequence()
    {
        AudioClip[] clips = m_SelectedClauseSeq.Select(x => x.AudioClause.AudioClauseWithToneClip).ToArray();

        foreach (AudioClip clip in clips)
        {
            yield return base.PlayAudioClipAndWaitWhilePlaying(clip);
        }

        m_PlayClipsInSeqRoutine = null;

        yield return null;
    }

    /* end of audios */


    /* controlling play clips button */

    private void HighLightPlayClipsBtn()
    {
        m_PlayClipsBtnImage.sprite = m_PlayClipsBtnHighLightSprite;
    }

    private void UnHighLightPlayClipsBtn()
    {
        m_PlayClipsBtnImage.sprite = m_PlayClipsBtnNormalSprite;
    }

    /* end of controlling this button */


    /* event handlers */

    // AudioClauseSelection.OnSelected()
    private void HandleClauseOptionSelected(AudioClauseSelection audioClauseSelection)
    {
        // boss option not selected yet
        if (!m_IsBossOptionSelected)
        {
            if (!audioClauseSelection.IsBossClause)
            {
                AudioClauseSelected firstNotYetFilledAudioClauseSlot =
                    GetFirstNotYetFilledAudioClauseSlot();

                if (firstNotYetFilledAudioClauseSlot)
                {
                    audioClauseSelection.PlayOnSelectedClip();

                    firstNotYetFilledAudioClauseSlot.FillSlotWithAudioClause(audioClauseSelection);
                    if (m_IsSelectionsDisappearOnSelectedOverride || audioClauseSelection.DisappearOnSelection)
                    {
                        StartCoroutine(audioClauseSelection.InterruptAndFadeOut());
                    }

                    HighlightNextAudioClauseToFill();
                }
                else
                {
                    audioClauseSelection.PlayOnOverClip();
                }
            }
            else
            {
                // set what appear and what disappear
                // basically, opposite to what happen in HandleBossClauseSlotOnClicked()

                m_IsBossOptionSelected = true;

                HighLightPlayClipsBtn();
                audioClauseSelection.PlayOnSelectedClip();
                
                StartCoroutine(m_NormalSelectedClauseTextBgFader.InterruptAndFadeOut());

                // fade out highlighted slot
                int highlightSlotIdx = 0;
                foreach (UIFader highlightSlotFader in m_HighlightSlotFaders)
                {
                    if (highlightSlotFader.Visible || highlightSlotFader.Fading)
                    {
                        StartCoroutine(highlightSlotFader.InterruptAndFadeOut());
                        m_HighlightedSlotFaderIdx = highlightSlotIdx;
                        break;
                    }
                    highlightSlotIdx++;
                }

                foreach (AudioClauseSelected slot in m_SelectedClauseSeq)
                {                    
                    if (slot.AudioClause != null)
                    {
                        StartCoroutine(slot.InterruptAndFadeOut());
                    }
                }

                m_BossSelectedClause.FillSlotWithAudioClause(audioClauseSelection);
                if (m_IsSelectionsDisappearOnSelectedOverride || audioClauseSelection.DisappearOnSelection)
                {
                    StartCoroutine(audioClauseSelection.InterruptAndFadeOut());
                }                
            }
        }
        // boss option selected already
        else
        {
            // selecting other audio clause option would result in no effect
            audioClauseSelection.PlayOnOverClip();
        }
    }

    // AudioClauseSelected.OnClicked()
    private void HandleClauseSlotOnClicked(AudioClauseSelected audioClauseSelected)
    {
        HighlightNextAudioClauseToFill();
        StartCoroutine(audioClauseSelected.InterruptAndFadeOut());
    }

    // BossAudioClauseSelected.OnClicked()
    // Reset when boss clause choice unmade 
    private void HandleBossClauseSlotOnClicked()
    {
        // set what appear and what disappear
        // basically, opposite to what happen in HandleClauseOptionSelected()

        m_IsBossOptionSelected = false;

        UnHighLightPlayClipsBtn();
        m_BossSelectedClause.PlayOnSelectedClip();

        StartCoroutine(m_NormalSelectedClauseTextBgFader.InterruptAndFadeIn());

        // fade in highlighted slot
        if (m_HighlightedSlotFaderIdx >= 0 && m_HighlightedSlotFaderIdx < m_HighlightSlotFaders.Length)
        {
            StartCoroutine(m_HighlightSlotFaders[m_HighlightedSlotFaderIdx].InterruptAndFadeIn());
        }

        foreach (AudioClauseSelected slot in m_SelectedClauseSeq)
        {            
            if (slot.AudioClause != null)
            {
                StartCoroutine(slot.InterruptAndFadeIn());
            }
        }

        if (m_IsSelectionsDisappearOnSelectedOverride || m_BossSelectedClause.DisappearOnSelection)
        {
            StartCoroutine(m_BossSelectedClause.InterruptAndFadeOut());
        }
    }

    /* end of event handlers */


    private bool ExtractAnswerSequence()
    {
        bool isGoodChoiceMade = false;

        if (!m_IsBossOptionSelected)
        {
            // 我戒咗差唔多一年喇，我希望你唔好擔心！
            int[] correctAnswer1 = new int[] { 0, 4, 6 };

            // 我最近返工太攰，所以先手震啫！
            int[] correctAnswer2 = new int[] { 1, 3, 8 };

            int[] selectedClauseIdices = new int[]
            {
                Array.IndexOf(m_ClauseAvailableOptions, m_SelectedClauseSeq[0].AudioClause),
                Array.IndexOf(m_ClauseAvailableOptions, m_SelectedClauseSeq[1].AudioClause),
                Array.IndexOf(m_ClauseAvailableOptions, m_SelectedClauseSeq[2].AudioClause)
            };

            isGoodChoiceMade =
                correctAnswer1.SequenceEqual(selectedClauseIdices) ||
                correctAnswer2.SequenceEqual(selectedClauseIdices);            
        }
        else
        {
            isGoodChoiceMade = false;
        }

        return isGoodChoiceMade;        
    }

    private IEnumerator HandleDownSequenceOfActions()
    {
        if (IsBossOptionSelected)
        {
            m_BossSelectedClause.ForceDisableButton();
        }
        else
        {
            foreach (AudioClauseSelected selectedAudio in m_SelectedClauseSeq)
            {
                selectedAudio.ForceDisableButton();
            }
        }

        bool isGoodChoiceMade = ExtractAnswerSequence();

        yield return StartCoroutine(StartPlayClipsInSequence());
        
        if (isGoodChoiceMade)
        {
            PlayOnSelectedClip();
        }
        else
        {
            PlayOnErrorClip();
        }

        Scribe.Side04 = isGoodChoiceMade;
        m_Sc04SClient.GoToSceneOnChoice();
    }


    /* IHandleUiButton interfaces */

    public override void HandleDown()
    {
        base.HandleDown();
        if (base.m_GazeOver)
        {
            // complete case
            if (GetFirstNotYetFilledAudioClauseSlot() == null || IsBossOptionSelected)
            {
                ForceDisableButton();
                StartCoroutine(HandleDownSequenceOfActions());
            }
            // incomplete case
            else
            {
                PlayOnOverClip();
            }
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
