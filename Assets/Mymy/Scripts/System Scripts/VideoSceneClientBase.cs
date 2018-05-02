using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoSceneClientBase : MonoBehaviour
{
    protected DrugVR_SceneENUM nextSceneToLoadBase;
    protected static GameManager managerInst;

    protected virtual void Awake()
    {
        GameManager.SkyVideoPlayer = GetComponent<VideoPlayer>();
    }

    protected void OnEnable()
    {
        managerInst = GameManager.Instance;
        managerInst.OnSceneVideoEnd += HandleSystemVideoEnd;
    }
    protected void OnDisable()
    {
        managerInst.OnSceneVideoEnd -= HandleSystemVideoEnd;
    }

    protected void OnDestroy()
    {
        GameManager.SkyVideoPlayer = null;
    }

    protected void HandleSystemVideoEnd(VideoPlayer source)
    {
        managerInst.GoToScene(nextSceneToLoadBase);
    }
}
