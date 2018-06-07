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
    public AudioSource[] BackgroundAudioSrcs
    {
        get
        {
            return new AudioSource[] { m_AmbAudioSrc, m_MusicAudioSrc };
        }
    }

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

    private void HandleSceneChange(DrugVR_SceneENUM nextScene)
    {
        IEnumerable<AudioLoopPackage> applicableAudioLoops =
            m_AudioLoops.Where(x => x.ScenesToLoop.Contains(nextScene));

        AudioLoopPackage package = applicableAudioLoops.SingleOrDefault(x => x.LoopType == AudioLoopType.Ambience);
        if (package == null)
        {
            //Mute audio source when no clip is assigned to this scene
            StartCoroutine(FadeOutAndMuteAudioSrcRoutine(m_AmbAudioSrc));
        }
        else
        {
            CheckAndPlayClip(m_AmbAudioSrc, package.Clip);
        }

        package = applicableAudioLoops.SingleOrDefault(x => x.LoopType == AudioLoopType.Music);
        if (package == null)
        {
            //Mute audio source when no clip is assigned to this scene
            StartCoroutine(FadeOutAndMuteAudioSrcRoutine(m_MusicAudioSrc));
        }
        else
        {
            CheckAndPlayClip(m_MusicAudioSrc, package.Clip);
        }
    }

    private void CheckAndPlayClip(AudioSource audioSrc, AudioClip audioClip)
    {
        if (audioSrc.isPlaying && audioSrc.clip == audioClip)
        {
            //We leave the audio to play when it matches the package already
        }
        else if (!audioSrc.isPlaying && audioSrc.clip == audioClip)
        {
            //if correct clip already assigned then we simply play and fade in
            audioSrc.mute = false;
            audioSrc.Play();
            StartCoroutine(AudioUtils.FadeInAudioToOne(audioSrc, 1f));
        }
        else
        {
            //Otherwise we replace and play clip
            StartCoroutine(AudioClipReplaceAndPlayRoutine(audioSrc, audioClip));
        }
    }

    private IEnumerator FadeOutAndMuteAudioSrcRoutine(AudioSource audioSrc)
    {
        yield return StartCoroutine(AudioUtils.FadeOutAudioToZero(audioSrc, 0.5f));
        audioSrc.mute = true;
        audioSrc.Stop();
    }

    private IEnumerator AudioClipReplaceAndPlayRoutine(AudioSource audioSrc, AudioClip audioClip)
    {
        if (!audioSrc.mute)
        {
            yield return StartCoroutine(FadeOutAndMuteAudioSrcRoutine(audioSrc));
        }
        audioSrc.clip = audioClip;
        audioSrc.mute = false;
        audioSrc.Play();
        StartCoroutine(AudioUtils.FadeInAudioToOne(audioSrc, 1f));
    }

    
}
