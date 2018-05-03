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
    public static GameManager Instance = null;
    
    //Only one instance of this videoplayer can be obtained and present in any scenes
    public static VideoPlayer SkyVideoPlayer { get; set; }

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

    public event Action<VideoPlayer> OnSceneVideoEnd;


    //make sure that we only have a single instance of the game manager
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;          
        }
        else if (Instance != this)
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
        if (SkyVideoPlayer == null && CurrentSceneScroll.SceneSky == SkyboxType.VideoSky)
        {
            SkyVideoPlayer = GetVideoPlayerInScene();
        }
        StartCoroutine(ReadScroll(CurrentSceneScroll));
        //if (CurrentSceneScroll.SceneSky == SkyboxType.VideoSky) PlayVideo();
    }

    private void FixedUpdate()
    {
        #if UNITY_EDITOR
            if (Input.GetKey("f"))
            {
                GoToScene(++CurrentScene);
            } 
        #endif
    }

    private IEnumerator ReadScroll(Scroll scroll)
    {

        //m_video.enabled = false;
        //m_video.loopPointReached -= OnVideoEnd;

        //Based skybox type of scene, assign video /image texture to environment skybox
        //Also to set skybox rotation of the scene
        if (scroll.SceneSky == SkyboxType.VideoSky)
        {
            //m_video = FindObjectOfType<VideoPlayer>();
            //m_video.url = Path.Combine(Application.dataPath, scroll.SkyContentPath);
            RenderSettings.skybox = VideoSkyMat;
            DynamicGI.UpdateEnvironment();

            VideoSkyMat.SetFloat("_Rotation", scroll.SkyShaderDefaultRotation);
            SkyVideoPlayer.enabled = true;
            SkyVideoPlayer.loopPointReached += OnVideoEnd;

            StartCoroutine(WaitForVideoPrepared());
            if (Scroll.ParseZeroAndOne(scroll.VideoStart_ImgPath))
            {
                PlayVideo();
            }
            else
            {
                SkyVideoPlayer.sendFrameReadyEvents = true;
                SkyVideoPlayer.frameReady += OnNewVideoFrameArrived;
                PlayVideo();
            }

        }
        else if (scroll.SceneSky == SkyboxType.ImageSky)
        {
            SkyVideoPlayer = null;
            Texture2D stillSkyTex = new Texture2D(2, 2);

            stillSkyTex = (Texture2D)Resources.Load(scroll.VideoStart_ImgPath);

            //string path = "jar:file://" + Application.dataPath + "!/assets/" +
            //    "skybox/resources/" + scroll.SkyContentPath + ".png";
            //using (WWW www = new WWW(path))
            //{
            //    yield return www;
            //    www.LoadImageIntoTexture(stillSkyTex);
            //}


            RenderSettings.skybox = StillSkyMat;
            StillSkyMat.SetTexture("_MainTex", stillSkyTex);
            DynamicGI.UpdateEnvironment();

            StillSkyMat.SetFloat("_Rotation", scroll.SkyShaderDefaultRotation);            
        }        

        

        HMD.trackRotation = scroll.HMDRotationEnabled;

        //TO BE REVIEWED WHEN NEEDED
        //---------------POSE TRACKER MANAGER----------------
        // Consider a situation: no pose is updated and WaveVR_PoseTrackerManager is enabled <-> disabled multiple times.
        // At this situation, IncludedStates will be set to false forever since they are deactivated at 1st time OnEnable()
        // and the deactivated state will be updated to IncludedStates in 2nd time OnEnable().
        // To prevent this situation, activate IncludedObjects in OnDisable to restore the state Children GameObjects.

        //if (scroll.ControllerEnabled)
        //{
        //    Controller.gameObject.SetActive(scroll.ControllerEnabled);
        //    Controller.enabled = scroll.ControllerEnabled;
        //    Controller.TrackRotation = scroll.ControllerRotEnabled;
        //}
        //else
        //{
        //    Controller.gameObject.SetActive(scroll.ControllerEnabled);
        //    Controller.enabled = scroll.ControllerEnabled;
        //}
        yield return null;
    }


    private IEnumerator WaitForVideoPrepared()
    {
        yield return new WaitUntil(() => SkyVideoPlayer.isPrepared);
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
        m_anim = Instance.GetComponentInChildren<Animator>();
        m_fadeImage = Instance.GetComponentInChildren<Image>();

        //Trigger FadeOut on the animator so our image will fade out
        m_anim.SetTrigger("FadeOut");

        //wait until the fade image is entirely black (alpha=1) then load next scene
        yield return new WaitUntil(() => m_fadeImage.color.a == 1);
        string nextSceneName = CurrentSceneScroll.SceneName;
        SceneManager.LoadScene(nextSceneName);
        Scene nextScene = SceneManager.GetSceneByName(nextSceneName);
        Debug.Log("loading scene:" + nextScene.name);
        yield return new WaitUntil(() => nextScene.isLoaded);


        //yield return StartCoroutine(ReadScroll(CurrentSceneScroll));
        StartCoroutine(ReadScroll(CurrentSceneScroll));

        //trigger FadeIn on the animator so our image will fade back in 
        m_anim.SetTrigger("FadeIn");

        //wait until the fade image is completely transparent (alpha = 0) and control UI back on
        yield return new WaitUntil(() => m_fadeImage.color.a == 0);
        isLoadingScene = false;
    }

    //Find the video in the scene and pause it
    public static void PauseVideo()
    {
        if (!SkyVideoPlayer)
        {
            SkyVideoPlayer = GetVideoPlayerInScene();
        }
        SkyVideoPlayer.Pause();
    }

    //Find the video in the scene and play it
    public static void PlayVideo()
    {
        if (SkyVideoPlayer == null)
        {
            SkyVideoPlayer = GetVideoPlayerInScene();
        }
        SkyVideoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer source)
    {
        OnSceneVideoEnd(source);
    }

    private void OnNewVideoFrameArrived(VideoPlayer source, long frameIdx)
    {
        if (frameIdx >= 0)
        {
            PauseVideo();
            source.sendFrameReadyEvents = false;
            source.frameReady -= OnNewVideoFrameArrived;
        }
    }

    private static VideoPlayer GetVideoPlayerInScene()
    {
        return FindObjectOfType<VideoPlayer>();
    }
}
