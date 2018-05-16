using DrugVR_Scribe;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public enum SkyboxType
{
    Null,
    VideoSky,
    ImageSky
}


public class GameManager : MonoBehaviour
{
    /* constants */

    private const string FOCUS_CONTROLLER_OBJECT_NAME = "/VIVEFocusWaveVR/FocusController";
    private const string HEAD_OBJECT_NAME = "/VIVEFocusWaveVR/head";
    private const string CONTROLLER_MODEL_OBJECT_NAME = "/VIVEFocusWaveVR/FocusController/MIA_Ctrl";
    
    //Put Path under Resources here
    private static string APP_IMAGE_SKY_DATA_PATH = "StillImg/";
    //Put Path under App. persistant data path here
    private static string APP_VIDEO_SKY_DATA_PATH = "Videos/";
    public static string APP_IMGSEQUENCE_DATA_PATH = "ImgSequences/";

    /* end of constants */


    /* public references */

    public static GameManager Instance = null;

    private GameObject m_FocusControllerObject;
    public GameObject FocusControllerObject
    {
        get
        {
            if (m_FocusControllerObject == null)
            {
                m_FocusControllerObject = GameObject.Find(FOCUS_CONTROLLER_OBJECT_NAME);
            }
            return m_FocusControllerObject;
        }
    }

    private GameObject m_HeadObject;
    public GameObject HeadObject
    {
        get
        {
            if (m_HeadObject == null)
            {
                m_HeadObject = GameObject.Find(HEAD_OBJECT_NAME);
            }
            return m_HeadObject;
        }
    }

    private GameObject m_ControllerModelObject;
    public GameObject ControllerModelObject
    {
        get
        {
            if (m_ControllerModelObject == null)
            {
                m_ControllerModelObject = GameObject.Find(CONTROLLER_MODEL_OBJECT_NAME);
            }
            return m_ControllerModelObject;
        }
    }

    //Only one instance of this videoplayer can be obtained and present in any scenes
    private VideoPlayer m_SkyVideoPlayer;
    public VideoPlayer SkyVideoPlayer
    {
        get
        {
            if (m_SkyVideoPlayer == null)
            {
                m_SkyVideoPlayer = GetVideoPlayerInScene();
            }
            return m_SkyVideoPlayer;
        }

        set
        {
            if (m_SkyVideoPlayer != value)
            {
                m_SkyVideoPlayer = value;
            }
        }
    }    

    [SerializeField]
    private DrugVR_SceneENUM m_CurrentScene;
    public DrugVR_SceneENUM CurrentScene
    {
        get { return m_CurrentScene; }
        set
        {
            CurrentSceneScroll = Scribe.SceneDictionary[value];
            m_CurrentScene = value;
        }
    }

    public Scroll CurrentSceneScroll { get; private set; }
    public Material VideoSkyMat;
    public Material StillSkyMat;

    public bool FadeToBlack = true;

    public event Action<VideoPlayer> OnSceneVideoEnd;
    public delegate void SceneChange(DrugVR_SceneENUM nextScene);
    public static event SceneChange OnSceneChange = null;

    /* end of public references */


    /* privates */

    private Animator m_anim;
    private Image m_fadeImage;
    
    private WaveVR_DevicePoseTracker HMD;
    private WaveVR_ControllerPoseTracker Controller;
    private bool isLoadingScene = false;

    /* end of privates */


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

        APP_VIDEO_SKY_DATA_PATH = Application.persistentDataPath + "/" + APP_VIDEO_SKY_DATA_PATH;
        APP_IMGSEQUENCE_DATA_PATH = Application.persistentDataPath + "/" + APP_IMGSEQUENCE_DATA_PATH;
    }

    private void Start()
    {
        CurrentScene = m_CurrentScene;

        HMD = FindObjectOfType<WaveVR_Render>().gameObject.GetComponent<WaveVR_DevicePoseTracker>();
        Controller = FindObjectOfType<WaveVR_ControllerPoseTracker>();
        


        if (SkyVideoPlayer == null && CurrentSceneScroll.SceneSky == SkyboxType.VideoSky)
        {
            SkyVideoPlayer = GetVideoPlayerInScene();
        }
        StartCoroutine(ReadScroll(CurrentSceneScroll));

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

            SkyVideoPlayer.source = VideoSource.Url;
            SkyVideoPlayer.url = APP_VIDEO_SKY_DATA_PATH + scroll.Video_ImgPath;
            SkyVideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            SkyVideoPlayer.controlledAudioTrackCount = 1;
            SkyVideoPlayer.SetTargetAudioSource(0, SkyVideoPlayer.GetComponent<AudioSource>());
            SkyVideoPlayer.Prepare();

            StartCoroutine(WaitForVideoPrepared());
            SkyVideoPlayer.skipOnDrop = true;

            if (scroll.VideoAutoPlay)
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

            stillSkyTex = (Texture2D)Resources.Load(APP_IMAGE_SKY_DATA_PATH + scroll.Video_ImgPath);

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
        yield return null;
    }


    private IEnumerator WaitForVideoPrepared()
    {
        yield return new WaitUntil(() => SkyVideoPlayer.isPrepared);
    }

    
    //Select scene is called from either the menu manager or hotspot manager, and is used to load the desired scene
    public void GoToScene(DrugVR_SceneENUM sceneEnum)
    {
        CurrentScene = sceneEnum;
        if (!isLoadingScene)
        {
            //if we want to use the fading between scenes, start the coroutine here
            if (FadeToBlack)
            {
                isLoadingScene = true;
                StartCoroutine(SceneChangeWithFadeOutIn(sceneEnum));
            }
            //if we dont want to use fading, just load the next scene
            else
            {
                SceneManager.LoadScene(CurrentSceneScroll.SceneName);

                ReadScroll(CurrentSceneScroll);
                if (OnSceneChange != null)
                {
                    OnSceneChange(sceneEnum);
                }
            }
        }

    }

    private IEnumerator SceneChangeWithFadeOutIn(DrugVR_SceneENUM nextSceneEnum)
    {                
        //get references to animatior and image component from children Game Object 
        m_anim = Instance.GetComponentInChildren<Animator>();
        m_fadeImage = Instance.GetComponentInChildren<Image>();

        //Trigger FadeOut on the animator so our image will fade out
        m_anim.SetTrigger("FadeOut");

        //wait until the fade image is entirely black (alpha=1) then load next scene
        yield return new WaitUntil(() => m_fadeImage.color.a == 1);
        string nextSceneName = CurrentSceneScroll.SceneName;
        SceneManager.LoadScene(nextSceneName);
        if (OnSceneChange != null)
        {
            OnSceneChange(nextSceneEnum);
        }
        Scene nextScene = SceneManager.GetSceneByName(nextSceneName);
        Debug.Log("loading scene:" + nextScene.name);
        yield return new WaitUntil(() => nextScene.isLoaded);


        //yield return StartCoroutine(ReadScroll(CurrentSceneScroll));
        StartCoroutine(ReadScroll(CurrentSceneScroll));
        CurrentScene = nextSceneEnum;
        //trigger FadeIn on the animator so our image will fade back in 
        m_anim.SetTrigger("FadeIn");

        //wait until the fade image is completely transparent (alpha = 0) and control UI back on
        yield return new WaitUntil(() => m_fadeImage.color.a == 0);
        isLoadingScene = false;
    }

    //Find the video in the scene and pause it
    public void PauseVideo()
    {
        SkyVideoPlayer.Pause();
    }

    //Find the video in the scene and play it
    public void PlayVideo()
    {
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

    private VideoPlayer GetVideoPlayerInScene()
    {
        return GetComponentInChildren<VideoPlayer>();
    }
}
