using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoSceneClientBase : MonoBehaviour
{
    protected DrugVR_SceneENUM nextSceneToLoadBase;
    protected GameManager managerInst;

    private void OnEnable()
    {
        managerInst = GameManager.Instance;

        managerInst.OnSceneVideoEnd += HandleSystemVideoEnd;
    }
    private void OnDisable()
    {
        managerInst.OnSceneVideoEnd -= HandleSystemVideoEnd;
    }

    private void OnDestroy()
    {
        GameManager.SkyVideoPlayer = null;
    }

    private void HandleSystemVideoEnd(VideoPlayer source)
    {
        managerInst.GoToScene(nextSceneToLoadBase);
    }
}
