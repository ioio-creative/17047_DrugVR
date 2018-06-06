using DrugVR_Scribe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudioControl : MonoBehaviour
{
    public enum AudioLoopType
    {
        Music,
        Ambience
    }

    public static BackgroundAudioControl Instance = null;

    [SerializeField]
    private AudioLoopPackage[] audioLoops;

    [Serializable]
    public class AudioLoopPackage
    {
        public AudioLoopType LoopType;
        public AudioClip Clip;
        public DrugVR_SceneENUM SceneToStartLoop;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
