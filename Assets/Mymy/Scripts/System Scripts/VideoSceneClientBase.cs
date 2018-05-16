using DrugVR_Scribe;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoSceneClientBase : MonoBehaviour
{    
    protected DrugVR_SceneENUM nextSceneToLoadBase;
    protected static GameManager managerInst;

    [SerializeField]
    private string localVideoRelativePath =
        "Videos/outro.mp4";  // example

    protected VideoPlayer skyVideoPlayer;
    private string localVideoUrl;


    /* MonoBehaviour */

    protected virtual void Awake()
    {
        GameManager.SkyVideoPlayer = GetComponent<VideoPlayer>();
        skyVideoPlayer = GameManager.SkyVideoPlayer;
        skyVideoPlayer.skipOnDrop = true;
    }

    protected virtual void OnEnable()
    {
        managerInst = GameManager.Instance;
        managerInst.OnSceneVideoEnd += HandleSystemVideoEnd;

        skyVideoPlayer.prepareCompleted += HandleVideoPrepareCompleted;
    }

    protected virtual void OnDisable()
    {
        managerInst.OnSceneVideoEnd -= HandleSystemVideoEnd;

        skyVideoPlayer.prepareCompleted -= HandleVideoPrepareCompleted;
    }

    protected virtual void OnDestroy()
    {
        skyVideoPlayer = null;
        GameManager.SkyVideoPlayer = null;
    }

    protected virtual void OnStart()
    {
        localVideoUrl = Application.persistentDataPath + "/"
            + localVideoRelativePath;

        skyVideoPlayer.source = VideoSource.Url;
        skyVideoPlayer.url = localVideoUrl;
        skyVideoPlayer.Prepare();
    }

    /* end of MonoBehaviour */


    /* event handlers */

    protected void GoToNextScene()
    {
        managerInst.GoToScene(nextSceneToLoadBase);
    }

    protected virtual void HandleSystemVideoEnd(VideoPlayer source)
    {
        GoToNextScene();
    }

    private void HandleVideoPrepareCompleted(VideoPlayer video)
    {
        video.Play();
    }

    /* end of event handlers */
}
