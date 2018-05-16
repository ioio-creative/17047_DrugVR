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
        managerInst.SkyVideoPlayer = null;
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
