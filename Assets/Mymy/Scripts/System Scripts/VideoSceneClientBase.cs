using DrugVR_Scribe;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoSceneClientBase : MonoBehaviour
{
    protected DrugVR_SceneENUM nextSceneToLoadBase;
    protected static GameManager managerInst;


    /* MonoBehaviour */

    protected virtual void Awake()
    {
        GameManager.SkyVideoPlayer = GetComponent<VideoPlayer>();
        GameManager.SkyVideoPlayer.skipOnDrop = true;
    }

    protected virtual void OnEnable()
    {
        managerInst = GameManager.Instance;
        managerInst.OnSceneVideoEnd += HandleSystemVideoEnd;
    }

    protected virtual void OnDisable()
    {
        managerInst.OnSceneVideoEnd -= HandleSystemVideoEnd;
    }

    protected virtual void OnDestroy()
    {
        GameManager.SkyVideoPlayer = null;
    }

    /* end of MonoBehaviour */


    /* event handlers */

    protected virtual void HandleSystemVideoEnd(VideoPlayer source)
    {
        managerInst.GoToScene(nextSceneToLoadBase);
    }
}
