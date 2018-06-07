using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc07SClient : VideoSceneClientBase
{
    private const DrugVR_SceneENUM partyMethSceneToLoad = DrugVR_SceneENUM.Sc07B;
    private DrugVR_SceneENUM endSceneToLoad;

    [SerializeField]
    private SelectionStandard m_ExitButton;
    private UIImageAnimationControl m_ExitUIAnimCtrl;

    [SerializeField]
    private AudioSource m_AudioSrc;
    [SerializeField]
    private AudioClip m_PoliceSingleClip;
    [SerializeField]
    private AudioClip m_ExitDoorWhiteClip;
    [SerializeField]
    private AudioClip m_ExitDoorBlackClip;

    protected override void Awake()
    {
        base.Awake();
        m_ExitUIAnimCtrl = m_ExitButton.GetComponentInChildren<UIImageAnimationControl>();

        if (m_AudioSrc == null)
        {
            m_AudioSrc = GetComponent<AudioSource>();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        managerInst.SkyVideoPlayer.isLooping = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (managerInst)
        {
            //prevent exception when quit app during this scene and 
            managerInst.SkyVideoPlayer.isLooping = false;
        }
    }

    private void Start()
    {
        if (m_ExitButton != null)
        {
            DeactivateExitButton();
        }
        
    }

    public void ActivateExitButton()
    {
        m_ExitButton.UnforceDisableButton();
        m_ExitUIAnimCtrl.ActivateUIAnimation();
        m_ExitButton.OnSelectionComplete += HandleExitSelected;
        StartCoroutine(m_ExitButton.InterruptAndFadeIn());
    }

    public void DeactivateExitButton()
    {
        m_ExitButton.ForceDisableButton();
        m_ExitUIAnimCtrl.DeactivateUIAnimation();
        m_ExitButton.OnSelectionComplete -= HandleExitSelected;
        StartCoroutine(m_ExitButton.InterruptAndFadeOut());
    }

    protected override void HandleSystemVideoEnd(VideoPlayer source)
    {
        // This scene won't go to next scene based on Video ends
    }

    private void HandleExitSelected()
    {
        Scribe.Side06 = true;
        StartCoroutine(ExitDoorRoutine());      
    }

    private IEnumerator ExitDoorRoutine()
    {       
        if (Scribe.EndingForPlayer == Ending.EndingA)
        {
            m_AudioSrc.clip = m_ExitDoorWhiteClip;
            m_AudioSrc.Play();
            StartCoroutine(GameManager.Instance.FadeOutToWhiteRoutine());
            StartCoroutine(AudioUtils.FadeOutAudioToZero(BackgroundAudioControl.Instance.BackgroundAudioSrcs, 2f));
            StartCoroutine(AudioUtils.FadeOutAudioToZero(GameManager.Instance.SkyVideoPlayer.GetComponent<AudioSource>(), 2f));
            yield return new WaitWhile(() => m_AudioSrc.isPlaying);
        }
        else
        {
            m_AudioSrc.clip = m_ExitDoorBlackClip;
            m_AudioSrc.Play();
            StartCoroutine(GameManager.Instance.FadeOutToBlackRoutine());
            StartCoroutine(AudioUtils.FadeOutAudioToZero(GameManager.Instance.SkyVideoPlayer.GetComponent<AudioSource>(), 2f));
            yield return new WaitWhile(() => m_AudioSrc.isPlaying);
            m_AudioSrc.clip = m_PoliceSingleClip;
            m_AudioSrc.Play();
            yield return new WaitWhile(() => m_AudioSrc.isPlaying);
        }

        GoToSceneOnChoice();
    }

    public void GoToSceneOnChoice()
    {
        if (Scribe.Side06 == false)
        {
            GoToMethScene();
        }
        else
        {
            GoToEndSceneOnChoice();
        }
    }

    private void GoToMethScene()
    {
        managerInst.GoToScene(partyMethSceneToLoad);
    }

    private void GoToEndSceneOnChoice()
    {
        endSceneToLoad = Scribe.EndingSceneENUM();
        managerInst.GoToScene(endSceneToLoad, 2f);
    }
}
