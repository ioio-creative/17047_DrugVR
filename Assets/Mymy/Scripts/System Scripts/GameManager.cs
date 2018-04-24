using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using DrugVR_Scribe;
using System;
using System.IO;

public enum SkyboxType
{
    Null,
    VideoSky,
    ImageSky
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private VideoPlayer m_video;
    private Animator m_anim;
    private Image m_fadeImage;

    public DrugVR_SceneENUM CurrentScene;
    private Scroll CurrentSceneScroll;

    public Material VideoSkyMat;
    public Material StillSkyMat;


    public bool FadeToBlack = true;

    private WaveVR_DevicePoseTracker HMD;
    private WaveVR_ControllerPoseTracker Controller;
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


    //make sure that we only have a single instance of the game manager
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;          
            m_video = GetVideoPlayerInScene();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }               
    }

    private void Start()
    {
        HMD = FindObjectOfType<WaveVR_Render>().gameObject.GetComponent<WaveVR_DevicePoseTracker>();
        Controller = FindObjectOfType<WaveVR_ControllerPoseTracker>();

        CurrentSceneScroll = Scribe.SceneDictionary[CurrentScene];
        //yield return StartCoroutine(ReadScroll(CurrentSceneScroll));
        ReadScroll(CurrentSceneScroll);
        //if (CurrentSceneScroll.SceneSky == SkyboxType.VideoSky) PlayVideo();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey("f") == true)
        {
            GoToNextScene();       
        }
    }

    private void ReadScroll(Scroll scroll)
    {
        m_video.enabled = false;
        m_video.loopPointReached -= OnVideoEnd;

        //Based skybox type of scene, assign video /image texture to environment skybox
        //Also to set skybox rotation of the scene
        if (scroll.SceneSky == SkyboxType.VideoSky)
        {
            m_video.url = Path.Combine(Application.dataPath, scroll.SkyContentPath);
            RenderSettings.skybox = VideoSkyMat;
            VideoSkyMat.SetFloat("_Rotation", scroll.SkyShaderDefaultRotation);
            m_video.enabled = true;
            m_video.loopPointReached += OnVideoEnd;

            StartCoroutine(VideoSwitch());
            PlayVideo();

        }
        else if (scroll.SceneSky == SkyboxType.ImageSky)
        {
            RenderSettings.skybox = StillSkyMat;
            Texture2D stillSkyTex = (Texture2D)Resources.Load(scroll.SkyContentPath);
            StillSkyMat.SetTexture("_MainTex", stillSkyTex);
            StillSkyMat.SetFloat("_Rotation", scroll.SkyShaderDefaultRotation);            
        }        

        DynamicGI.UpdateEnvironment();

        HMD.trackRotation = scroll.HMDRotationEnabled;

        Controller.TrackRotation = scroll.ControllerRotEnabled;
        Controller.gameObject.SetActive(scroll.ControllerEnabled);
        Controller.enabled = scroll.ControllerEnabled;

    }

    private IEnumerator VideoSwitch()
    {
        yield return new WaitUntil(() => m_video.isPrepared);
    }

    public void GoToNextScene()
    {
        if(!isLoadingScene) GoToScene(++CurrentScene);
    }

    
    //Select scene is called from either the menu manager or hotspot manager, and is used to load the desired scene
    public void GoToScene(DrugVR_SceneENUM sceneEnum)
    {
        if (!isLoadingScene)
        {
            //if we want to use the fading between scenes, start the coroutine here
            if (FadeToBlack)
            {
                isLoadingScene = true;
                StartCoroutine(FadeOutAndIn(sceneEnum));
            }
            //if we dont want to use fading, just load the next scene
            else
            {
                string sceneToLoadName = Scribe.SceneDictionary[sceneEnum].SceneName;
                SceneManager.LoadScene(sceneToLoadName);
            }
        }

    }

    private IEnumerator FadeOutAndIn(DrugVR_SceneENUM nextSceneEnum)
    {
        CurrentSceneScroll = Scribe.SceneDictionary[nextSceneEnum];
        
        //get references to animatior and image component from children Game Object 
        m_anim = instance.GetComponentInChildren<Animator>();
        m_fadeImage = instance.GetComponentInChildren<Image>();

        //Trigger FadeOut on the animator so our image will fade out
        m_anim.SetTrigger("FadeOut");

        //wait until the fade image is entirely black (alpha=1) then load next scene
        yield return new WaitUntil(() => m_fadeImage.color.a == 1);
        string nextSceneName = CurrentSceneScroll.SceneName;
        SceneManager.LoadScene(nextSceneName);
        Scene nextScene = SceneManager.GetSceneByName(nextSceneName);
        Debug.Log("loading scene:" + nextScene.name);
        //yield return new WaitUntil(() => nextScene.isLoaded);


        //yield return StartCoroutine(ReadScroll(CurrentSceneScroll));
        ReadScroll(CurrentSceneScroll);

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
            m_video = GetVideoPlayerInScene();
        }
        m_video.Pause();
    }

    //Find the video in the scene and play it
    public void PlayVideo()
    {
        if (!m_video)
        {
            m_video = GetVideoPlayerInScene();
        }
        m_video.Play();
    }

    private void OnVideoEnd(VideoPlayer source)
    {
        GoToNextScene();
    }

    private VideoPlayer GetVideoPlayerInScene()
    {
        return GetComponentInChildren<VideoPlayer>();
    }
}
