using DrugVR_Scribe;
using System;
using System.Collections;
using System.Linq;
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
    public delegate void SceneChange(DrugVR_SceneENUM nextScene);
    public static event SceneChange OnSceneChange = null;

    /* constants */

    private const string FOCUS_CONTROLLER_OBJECT_NAME = "/VIVEFocusWaveVR/FocusController";
    private const string HEAD_OBJECT_NAME = "/VIVEFocusWaveVR/head";
    
    // Put Path under Resources here
    private static string APP_IMAGE_SKY_DATA_PATH = "StillImg/";
    // Put Path under App. persistant data path here
    private static string APP_VIDEO_SKY_DATA_PATH = "Videos/";
    public static string APP_IMGSEQUENCE_DATA_PATH = "ImgSequences/";

    /* end of constants */


    /* public references */

    public static GameManager Instance = null;

    [SerializeField]
    private Canvas m_DebugCanvas;
    [SerializeField]
    private Text m_DebugText;
    [SerializeField]
    private bool m_IsShowDebugCanvas;
    public bool IsShowingDebugCanvas
    {
        get { return m_IsShowDebugCanvas; }
        set
        {
            m_IsShowDebugCanvas = value;
            if (m_IsShowDebugCanvas)
            {
                m_DebugCanvas.gameObject.SetActive(true);
            }
            else
            {
                m_DebugCanvas.gameObject.SetActive(false);
            }
        }
    }

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

    private MyControllerSwtich m_ControllerSwitch;
    public MyControllerSwtich ControllerSwitch
    {
        get
        {
            if (m_ControllerSwitch == null)
            {
                m_ControllerSwitch = FindObjectOfType<MyControllerSwtich>();
            }
            return m_ControllerSwitch;
        }
        set { m_ControllerSwitch = value; }
    }

    private SceneMenuControl m_MenuControl;

    public SceneMenuControl MenuControl
    {
        get
        {
            if (m_MenuControl == null)
            {
                m_MenuControl = GetComponentInChildren<SceneMenuControl>();
            }
            return m_MenuControl;
        }
        set { m_MenuControl = value; }
    }

    public event Action<VideoPlayer> OnSceneVideoEnd;
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

    public bool FadeBetweenScenes = true;

    /* end of public references */


    /* privates */

    private Animator m_anim;
    private Image m_fadeImage;
    
    private WaveVR_DevicePoseTracker HMD;
    private WaveVR_ControllerPoseTracker Controller;
    private bool isLoadingScene = false;

    /* end of privates */

    /* MonoBehaviour */
    
    //make sure that we only have a single instance of the game manager
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;

            // The following won't be run by other instances, so that they are run once only.
            APP_VIDEO_SKY_DATA_PATH = Application.persistentDataPath + "/" + APP_VIDEO_SKY_DATA_PATH;
            APP_IMGSEQUENCE_DATA_PATH = Application.persistentDataPath + "/" + APP_IMGSEQUENCE_DATA_PATH;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //get references to animatior and image component from children Game Object 
        m_anim = Instance.GetComponentInChildren<Animator>();
        m_fadeImage = Instance.GetComponentInChildren<Image>();

        //Debug Canvas
        if (m_IsShowDebugCanvas)
        {
            m_DebugCanvas.gameObject.SetActive(true);
        }
        else
        {
            m_DebugCanvas.gameObject.SetActive(false);
        }
    }

    private IEnumerator Start()
    {
        DebugLog(Application.persistentDataPath);

        CurrentScene = m_CurrentScene;

        //This is to fail-safe check in case CurrentScene does not match current scene
        if (SceneManager.GetActiveScene().name != CurrentSceneScroll.SceneName)
        {
            CurrentScene = Scribe.SceneDictionary.Single(x => x.Value.SceneName == SceneManager.GetActiveScene().name).Key;
        }

        HMD = FindObjectOfType<WaveVR_Render>().gameObject.GetComponent<WaveVR_DevicePoseTracker>();
        Controller = FindObjectOfType<WaveVR_ControllerPoseTracker>();

        yield return StartCoroutine(Scribe.LoadTxtDataWWW());

        StartCoroutine(ReadScroll(CurrentSceneScroll));
        if (OnSceneChange != null)
        {
            OnSceneChange(CurrentScene);
        }
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

    private void Update()
    {
        //DebugLog(CurrentSceneScroll.VideoAutoPlay.ToString());
        //DebugLog(File.Exists(APP_VIDEO_SKY_DATA_PATH + CurrentSceneScroll.Video_ImgPath).ToString());
        //DebugLog(APP_VIDEO_SKY_DATA_PATH + CurrentSceneScroll.Video_ImgPath);
        //DebugLog(CurrentSceneScroll.SceneSky.ToString());
    }

    /* end of MonoBehaviour */

    private IEnumerator ReadScroll(Scroll scroll)
    {
        //TODO: Try to impletment HMD rotation offest to sky rotation

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

            yield return StartCoroutine(WaitForVideoPrepared());
            SkyVideoPlayer.skipOnDrop = true;

            if (scroll.VideoAutoPlay)
            {
                SkyVideoPlayer.GetComponent<AudioSource>().volume = 1;
                PlayVideo();
            }
            else
            {
                SkyVideoPlayer.sendFrameReadyEvents = true;
                SkyVideoPlayer.frameReady += OnNewVideoFrameArrived;
                SkyVideoPlayer.GetComponent<AudioSource>().mute = true;
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

            //DebugLog("Reading sky image: " + (stillSkyTex != null));
            //DebugLog(APP_IMAGE_SKY_DATA_PATH + scroll.Video_ImgPath);

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
    public void GoToScene(DrugVR_SceneENUM sceneEnum, float delayInSeconds = 0f)
    {
        CurrentScene = sceneEnum;
        if (!isLoadingScene)
        {
            //if we want to use the fading between scenes, start the coroutine here
            if (FadeBetweenScenes)
            {
                isLoadingScene = true;
                StartCoroutine(SceneChangeWithFadeOutIn(sceneEnum, delayInSeconds));
            }
            //if we dont want to use fading, just load the next scene
            else
            {
                PauseVideo();
                SceneManager.LoadScene(CurrentSceneScroll.SceneName);

                ReadScroll(CurrentSceneScroll);               
            }
            if (OnSceneChange != null)
            {
                OnSceneChange(sceneEnum);
            }
        }

    }

    //public IEnumerator Routine;
    //private IEnumerator DoSomething()
    //{
    //    yield return StartCoroutine(Routine);
    //}

    private IEnumerator SceneChangeWithFadeOutIn(DrugVR_SceneENUM nextSceneEnum, float delayTime = 0)
    {
        if (m_anim.GetCurrentAnimatorStateInfo(0).IsName("FadeIn"))
        {
            yield return StartCoroutine(FadeOutRoutine());
        }

        StopVideo();
        //TODO
        Resources.UnloadUnusedAssets();
        GC.Collect();

        string nextSceneName = CurrentSceneScroll.SceneName;
        SceneManager.LoadScene(nextSceneName);       
        if (OnSceneChange != null)
        {
            OnSceneChange(nextSceneEnum);
        }
        Scene nextScene = SceneManager.GetSceneByName(nextSceneName);
        Debug.Log("loading scene:" + nextScene.name);
        yield return new WaitUntil(() => nextScene.isLoaded);

        if (delayTime > 0f)
        {
            yield return new WaitForSecondsRealtime(delayTime);
        }

        //yield return StartCoroutine(ReadScroll(CurrentSceneScroll));
        StartCoroutine(ReadScroll(CurrentSceneScroll));
        CurrentScene = nextSceneEnum;

        //trigger FadeIn on the animator so our scene will fade back in 
        if (m_anim.GetCurrentAnimatorStateInfo(0).IsName("FadeOut"))
        {
            yield return StartCoroutine(FadeInRoutine());
        }
        isLoadingScene = false;
    }
    
    //Find the video in the scene and play/pause/stop it
    public void PlayVideo()
    {
        SkyVideoPlayer.Play();
    }
    public void PauseVideo()
    {
        SkyVideoPlayer.Pause();
    }
    public void StopVideo()
    {
        SkyVideoPlayer.Stop();
    }
    

    private void OnVideoEnd(VideoPlayer source)
    {
        if (OnSceneVideoEnd != null)
        {
            OnSceneVideoEnd(source);
        }
    }

    private void OnNewVideoFrameArrived(VideoPlayer source, long frameIdx)
    {
        //Pause Video on the first Frame
        if (frameIdx >= 0)
        {
            PauseVideo();
            source.GetComponent<AudioSource>().mute = false;
            source.sendFrameReadyEvents = false;
            source.frameReady -= OnNewVideoFrameArrived;
        }
    }

    private VideoPlayer GetVideoPlayerInScene()
    {
        return GetComponentInChildren<VideoPlayer>();
    }

    //FadeOut == fade image fade in
    public IEnumerator FadeOutToBlackRoutine()
    {   
        yield return StartCoroutine(FadeToColorRoutine(Color.black));
    }

    public IEnumerator FadeOutToWhiteRoutine()
    {
        yield return StartCoroutine(FadeToColorRoutine(Color.white));
    }

    public IEnumerator FadeToColorRoutine(Color color)
    {
        m_fadeImage.color = color;
        yield return StartCoroutine(FadeOutRoutine());
    }

    public IEnumerator FadeOutRoutine()
    {       
        m_anim.SetTrigger("FadeOut");
        //wait until the fade image is entirely black (alpha=1) then load next scene
        yield return new WaitUntil(() => m_fadeImage.color.a == 1);
    }

    public IEnumerator FadeInRoutine()
    {
        m_anim.SetTrigger("FadeIn");
        //wait until the fade image is completely transparent (alpha = 0) and control UI back on
        yield return new WaitUntil(() => m_fadeImage.color.a == 0);
    }


    /* debugging */

    public void DebugLog(object obj)
    {
        if (m_IsShowDebugCanvas && m_DebugText != null && m_DebugText.isActiveAndEnabled)
        {
            m_DebugText.text = obj.ToString();
        }
    }

    /* end of debugging */
}
