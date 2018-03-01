using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    private Scene scene;
    private VideoPlayer video;
    private Animator anim;
    private Image fadeImage;

    [Header("Scene Management")]
    public string nextScene;
    public string ActiveScene
    {
        get { return activeScene; }
    }
    private string activeScene;

    [Space]
    [Header("UI Settings")]

    public bool useFade;
    public GameObject fadeOverlay;
    public GameObject ControlUI;

    //make sure that we only have a single instance of the game manager
    void Awake()
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
