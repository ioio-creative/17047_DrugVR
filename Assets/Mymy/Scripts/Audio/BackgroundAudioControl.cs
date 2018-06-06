using DrugVR_Scribe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundAudioControl : MonoBehaviour
{
    public enum AudioLoopType
    {
        Ambience,
        Music        
    }

    public static BackgroundAudioControl Instance = null;

    [SerializeField]
    private AudioLoopPackage[] m_AudioLoops;

    [Serializable]
    public class AudioLoopPackage
    {
        public AudioLoopType LoopType;
        public AudioClip Clip;
        public DrugVR_SceneENUM[] ScenesToLoop;
    }

    [SerializeField]
    private AudioSource m_AmbAudioSrc;
    [SerializeField]
    private AudioSource m_MusicAudioSrc;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;          
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
        
        m_AmbAudioSrc.volume = 0;
        m_AmbAudioSrc.mute = true;
        m_AmbAudioSrc.loop = true;
        m_MusicAudioSrc.volume = 0;
        m_AmbAudioSrc.mute = true;
        m_MusicAudioSrc.loop = true;
    }

    private void OnEnable()
    {
        GameManager.OnSceneChange += HandleSceneChange;
    }

    private void OnDisable()
    {
        GameManager.OnSceneChange -= HandleSceneChange;
    }

    private void OnDestroy()
    {
        Debug.Log("BGACtrl Destroy");
    }

    private void HandleSceneChange(DrugVR_SceneENUM nextScene)
    {
        IEnumerable<AudioLoopPackage> applicableAudioLoops =
            m_AudioLoops.Where(x => x.ScenesToLoop.Contains(nextScene));


        //We check if AudioLoopPackage is applicable to this scene, including cases when jumping scenes via menu
        foreach (AudioLoopPackage applicableAudioLoop in applicableAudioLoops)
        {
            AudioSource _audioSrc;
            //TODO Play loop
            switch (applicableAudioLoop.LoopType)
            {
                case AudioLoopType.Ambience:
                default:
                    _audioSrc = m_AmbAudioSrc;
                    break;
                case AudioLoopType.Music:
                    _audioSrc = m_MusicAudioSrc;
                    break;
            }

            if (_audioSrc.isPlaying && _audioSrc.clip == applicableAudioLoop.Clip)
            {
                //We leave the audio to play when it matches the package already
                continue;
            }
            else if (!_audioSrc.isPlaying && _audioSrc.clip == applicableAudioLoop.Clip)
            {
                _audioSrc.mute = false;
                _audioSrc.Play();
                StartCoroutine(AudioUtils.FadeInAudioToOne(_audioSrc, 1f));
            }
            else
            {
                StartCoroutine(AudioTransitionRoutine(_audioSrc, applicableAudioLoop));
            }
        }
    }

    private IEnumerator AudioTransitionRoutine(AudioSource audioSrc, AudioLoopPackage loopPkg)
    {
        if (!audioSrc.mute)
        {
            yield return StartCoroutine(AudioUtils.FadeOutAudioToZero(audioSrc, 0.5f));
            audioSrc.mute = true;
            audioSrc.Stop();
        }
        audioSrc.clip = loopPkg.Clip;
        audioSrc.mute = false;
        audioSrc.Play();
        StartCoroutine(AudioUtils.FadeInAudioToOne(audioSrc, 1f));
    }
}
