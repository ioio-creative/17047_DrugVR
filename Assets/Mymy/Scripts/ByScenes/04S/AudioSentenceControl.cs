using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class AudioSentenceControl : VrButtonBase
{
    [SerializeField]
    private Sprite m_PlayClipsBtnNormalSprite;
    [SerializeField]
    private Sprite m_PlayClipsBtnHighLightSprite;
    [SerializeField]
    private Image m_PlayClipsBtnImage;
    [SerializeField]
    private bool m_IsSelectionsDisappearOnSelectedOverride;
    [SerializeField]
    private GameObject m_ClauseAvailableOptionsContainer;
    [SerializeField]
    private GameObject m_SelectedClauseContainer;
    [SerializeField]
    private BossAudioClauseSelected m_BossSelectedClause;
    [SerializeField]
    private UIFader m_NormalSelectedClauseTextBgFader;

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

    private Coroutine m_PlayClipsInSeqRoutine = null;

    private bool m_IsBossOptionSelected = false;


    /* MonoBehaviour */

    protected override void Awake()
    {
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


    private void HighlightNextAudioClauseToFill()
    {
        bool isThereMoreSlotsToFill = false;

        AudioClauseSelected nextAudioClauseSlotToFill = m_FirstNotYetFilledAudioClauseSlot;
        if (nextAudioClauseSlotToFill != null)
        {
            isThereMoreSlotsToFill = true;
            nextAudioClauseSlotToFill.SetHighLight();
            foreach (AudioClauseSelected slot in m_SelectedClauseSeq)
            {
                if (slot != nextAudioClauseSlotToFill && slot.AudioClause == null)
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

    private void StartPlayClipsInSequence()
    {
        // boss option not selected yet
        if (!m_IsBossOptionSelected)
        {
            // if the coroutine has not been started
            // and all audio clause slots are filled
            if (m_PlayClipsInSeqRoutine == null &&
                m_FirstNotYetFilledAudioClauseSlot == null)
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
    private void HandleClauseOptionSelected(AudioClauseSelection audioClauseSelected)
    {
        // boss option not selected yet
        if (!m_IsBossOptionSelected)
        {
            if (!audioClauseSelected.IsBossClause)
            {
                AudioClauseSelected firstNotYetFilledAudioClauseSlot =
                    m_FirstNotYetFilledAudioClauseSlot;

                if (firstNotYetFilledAudioClauseSlot)
                {
                    audioClauseSelected.PlayOnSelectedClip();

                    firstNotYetFilledAudioClauseSlot.FillSlotWithAudioClause(audioClauseSelected);
                    if (m_IsSelectionsDisappearOnSelectedOverride || audioClauseSelected.IsDisappearOnSelected)
                    {
                        StartCoroutine(audioClauseSelected.InteruptAndFadeOut());
                    }

                    HighlightNextAudioClauseToFill();
                }
                else
                {
                    audioClauseSelected.PlayOnErrorClip();
                }
            }
            else
            {
                // set what appear and what disappear
                // basically, opposite to what happen in HandleBossClauseSlotOnClicked()

                m_IsBossOptionSelected = true;

                HighLightPlayClipsBtn();
                audioClauseSelected.PlayOnSelectedClip();
                
                StartCoroutine(m_NormalSelectedClauseTextBgFader.InteruptAndFadeOut());
                foreach (AudioClauseSelected slot in m_SelectedClauseSeq)
                {
                    StartCoroutine(slot.InteruptAndFadeOut());
                }
                m_BossSelectedClause.FillSlotWithAudioClause(audioClauseSelected);
                if (m_IsSelectionsDisappearOnSelectedOverride || audioClauseSelected.IsDisappearOnSelected)
                {
                    StartCoroutine(audioClauseSelected.InteruptAndFadeOut());
                }                
            }
        }
        // boss option selected already
        else
        {
            // selecting other audio clause option would result in no effect
            audioClauseSelected.PlayOnErrorClip();
        }
    }

    // AudioClauseSelected.OnClicked()
    private void HandleClauseSlotOnClicked(AudioClauseSelected audioClauseSelected)
    {
        HighlightNextAudioClauseToFill();
    }

    // BossAudioClauseSelected.OnClicked()
    private void HandleBossClauseSlotOnClicked()
    {
        // set what appear and what disappear
        // basically, opposite to what happen in HandleClauseOptionSelected()

        m_IsBossOptionSelected = false;

        UnHighLightPlayClipsBtn();
        m_BossSelectedClause.PlayOnSelectedClip();

        StartCoroutine(m_NormalSelectedClauseTextBgFader.InteruptAndFadeIn());
        foreach (AudioClauseSelected slot in m_SelectedClauseSeq)
        {
            StartCoroutine(slot.InteruptAndFadeIn());
        }

        if (m_IsSelectionsDisappearOnSelectedOverride || m_BossSelectedClause.IsDisappearOnSelected)
        {
            StartCoroutine(m_BossSelectedClause.InteruptAndFadeOut());
        }
    }

    /* end of event handlers */


    /* IHandleUiButton interfaces */

    public override void HandleDown()
    {
        base.HandleDown();
        if (base.m_GazeOver)
        {
            StartPlayClipsInSequence();
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
