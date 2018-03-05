using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public enum SkyboxType
{
    VideoSky,
    ImageSky
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private Scene scene;
    private VideoPlayer video;
    private Animator anim;
    private Image fadeImage;

    public string nextSceneName;
    public SkyboxType nextSkyType = SkyboxType.ImageSky;
    public Material nextSkyMat;
    public string ActiveSceneName
    {
        get { return activeSceneName; }
    }
    private string activeSceneName;


    public bool FadeToBlack = true;
    public GameObject fadeImageObj;

    private bool isLoadingScene = false;

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
        scene = SceneManager.GetActiveScene();
        activeSceneName = scene.buildIndex + " - " + scene.name;
    }

    private void FixedUpdate()
    {
        if(Input.GetKey("f") == true)
        {
            SelectNextScene();           
        }
    }

    public void SelectNextScene()
    {
        if(!isLoadingScene) SelectScene(nextSceneName);
    }


    //Select scene is called from either the menu manager or hotspot manager, and is used to load the desired scene
    public void SelectScene(string sceneToLoad)
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
        //get references to animatior and image component 
        anim = fadeImageObj.GetComponent<Animator>();
        fadeImage = fadeImageObj.GetComponent<Image>();

        //Trigger FadeOut on the animator so our image will fade out
        anim.SetTrigger("FadeOut");

        //wait until the fade image is entirely black (alpha=1) then load next scene
        yield return new WaitUntil(() => fadeImage.color.a == 1);
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        Scene nextScene = SceneManager.GetSceneByName(sceneToLoad);
        Debug.Log("loading scene:" + nextScene.name);
        yield return new WaitUntil(() => nextScene.isLoaded);

        if (skyboxType == SkyboxType.VideoSky)
        {
            //grab video and wait until it is loaded and prepared before starting the fade out
            video = FindObjectOfType<VideoPlayer>();
            yield return new WaitUntil(() => video.isPrepared);
        }
        else if (skyboxType == SkyboxType.ImageSky)
        {
            RenderSettings.skybox = nextSkyMat;
        }

        //SceneManager.UnloadSceneAsync(scene);
        scene = nextScene;
        activeSceneName = nextScene.name;
        //trigger FadeIn on the animator so our image will fade back in 
        anim.SetTrigger("FadeIn");

        //wait until the fade image is completely transparent (alpha = 0) and control UI back on
        yield return new WaitUntil(() => fadeImage.color.a == 0);
    }

    //Find the video in the scene and pause it
    public void PauseVideo()
    {
        if (!video)
        {
            video = FindObjectOfType<VideoPlayer>();
        }
        video.Pause();
    }

    //Find the video in the scene and play it
    public void PlayVideo()
    {
        if (!video)
        {
            video = FindObjectOfType<VideoPlayer>();
        }
        video.Play();
    }
}
