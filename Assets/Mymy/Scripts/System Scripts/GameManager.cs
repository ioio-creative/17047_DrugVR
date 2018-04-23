﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using DrugVR_Scribe;

public enum SkyboxType
{
    VideoSky,
    ImageSky
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private Scene m_scene;
    private VideoPlayer m_video;
    private Animator m_anim;
    private Image m_fadeImage;

    public DrugVR_SceneENUM currentScene;
    public SkyboxType nextSkyType = SkyboxType.ImageSky;
    public Material nextSkyMat;
    public string ActiveSceneName
    {
        get { return activeSceneName; }
    }
    private string activeSceneName;


    public bool FadeToBlack = true;

    private bool isLoadingScene = false;

    #region Scribe Fields
    public bool Side01
    {
        get { return Scribe.side01; }
        set { Scribe.side01 = value; }
    }
    public bool Side02
    {
        get { return Scribe.side02; }
        set { Scribe.side02 = value; }
    }
    public bool Side03
    {
        get { return Scribe.side01; }
        set { Scribe.side01 = value; }
    }
    public bool Side04
    {
        get { return Scribe.side05; }
        set { Scribe.side05 = value; }
    }
    public bool Side05
    {
        get { return Scribe.side05; }
        set { Scribe.side05 = value; }
    }
    #endregion

    private void OnEnable()
    {
        if (!m_video)
        {
            m_video = FindObjectOfType<VideoPlayer>();
            m_video.loopPointReached += OnVideoEnd;
        }
    }

    private void OnDisable()
    {
        if (!m_video)
        {
            m_video = FindObjectOfType<VideoPlayer>();
            m_video.loopPointReached -= OnVideoEnd;
        }
    }
    //make sure that we only have a single instance of the game manager
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
    }

    private void Start()
    {
        m_scene = SceneManager.GetActiveScene();
        activeSceneName = m_scene.buildIndex + " - " + m_scene.name;
    }

    private void FixedUpdate()
    {
        if(Input.GetKey("f") == true)
        {
            GoToNextScene();       
        }
    }

    public void ReadScroll(Scroll scroll)
    {
        currentScene = scroll.Scene;
        nextSkyType = scroll.Skybox;
    }

    public void GoToNextScene()
    {
        if(!isLoadingScene) GoToScene(Scribe.SceneDictionary[++currentScene]);
    }

    public void GoToScene(DrugVR_SceneENUM sceneEnum)
    {
        GoToScene(Scribe.SceneDictionary[sceneEnum]);
    }
    

    //Select scene is called from either the menu manager or hotspot manager, and is used to load the desired scene
    public void GoToScene(string sceneToLoad)
    {
        //if we want to use the fading between scenes, start the coroutine here
        if (FadeToBlack)
        {
            isLoadingScene = true;
            StartCoroutine(FadeOutAndIn(sceneToLoad, nextSkyType));
        }
        //if we dont want to use fading, just load the next scene
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        //set the active scene to the next scene
        activeSceneName = sceneToLoad;
    }

    IEnumerator FadeOutAndIn(string sceneToLoad, SkyboxType skyboxType)
    {
        //get references to animatior and image component from children Game Object 
        m_anim = instance.GetComponentInChildren<Animator>();
        m_fadeImage = instance.GetComponentInChildren<Image>();

        //Trigger FadeOut on the animator so our image will fade out
        m_anim.SetTrigger("FadeOut");

        //wait until the fade image is entirely black (alpha=1) then load next scene
        yield return new WaitUntil(() => m_fadeImage.color.a == 1);
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        Scene nextScene = SceneManager.GetSceneByName(sceneToLoad);
        Debug.Log("loading scene:" + nextScene.name);
        yield return new WaitUntil(() => nextScene.isLoaded);

        if (skyboxType == SkyboxType.VideoSky)
        {
            //grab video and wait until it is loaded and prepared before starting the fade out
            m_video = FindObjectOfType<VideoPlayer>();
            yield return new WaitUntil(() => m_video.isPrepared);
        }
        else if (skyboxType == SkyboxType.ImageSky)
        {
            RenderSettings.skybox = nextSkyMat;
        }

        //SceneManager.UnloadSceneAsync(scene);
        m_scene = nextScene;
        activeSceneName = nextScene.name;
        //trigger FadeIn on the animator so our image will fade back in 
        m_anim.SetTrigger("FadeIn");

        //wait until the fade image is completely transparent (alpha = 0) and control UI back on
        yield return new WaitUntil(() => m_fadeImage.color.a == 0);
    }

    //Find the video in the scene and pause it
    public void PauseVideo()
    {
        if (!m_video)
        {
            m_video = FindObjectOfType<VideoPlayer>();
        }
        m_video.Pause();
    }

    //Find the video in the scene and play it
    public void PlayVideo()
    {
        if (!m_video)
        {
            m_video = FindObjectOfType<VideoPlayer>();
        }
        m_video.Play();
    }

    private void OnVideoEnd(VideoPlayer source)
    {
        GoToNextScene();
    }
}
