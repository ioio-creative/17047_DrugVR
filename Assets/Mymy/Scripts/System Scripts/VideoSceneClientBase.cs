using DrugVR_Scribe;
using UnityEngine;
using UnityEngine.Video;

public class VideoSceneClientBase : MonoBehaviour
{    
    protected DrugVR_SceneENUM nextSceneToLoadBase;
    protected GameManager managerInst;

    /* MonoBehaviour */

    protected virtual void Awake()
    {
        managerInst = GameManager.Instance;
        managerInst.SkyVideoPlayer = GetComponent<VideoPlayer>();
        if (managerInst.SkyVideoPlayer.isLooping)
        {
            managerInst.SkyVideoPlayer.isLooping = false;
        }
    }

    protected virtual void OnEnable()
    {       
        managerInst.OnSceneVideoEnd += HandleSystemVideoEnd;
    }

    protected virtual void OnDisable()
    {
        managerInst.OnSceneVideoEnd -= HandleSystemVideoEnd;
    }

    protected virtual void OnDestroy()
    {
        if (managerInst != null)
        {
            if (managerInst.SkyVideoPlayer.isPlaying)
            {
                managerInst.StopVideo();
            }
            if (managerInst.SkyVideoPlayer.isLooping)
            {
                managerInst.SkyVideoPlayer.isLooping = false;
            }
            //This line is obsolete since we no longer have different VideoPlayer for each scene
            //managerInst.SkyVideoPlayer = null; 
        }
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

    /* end of event handlers */
}
