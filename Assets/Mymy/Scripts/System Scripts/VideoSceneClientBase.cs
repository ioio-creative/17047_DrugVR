using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoSceneClientBase : MonoBehaviour
{
    protected DrugVR_SceneENUM nextSceneToLoadBase;
    private GameManager managerInst;

    protected void AwakeBase()
    {
        managerInst = GameManager.Instance;
    }

    private void OnEnable()
    {
        managerInst.OnSystemVideoEnd += HandleSystemVideoEnd;
    }
    private void OnDisable()
    {
        managerInst.OnSystemVideoEnd -= HandleSystemVideoEnd;
    }

    private void HandleSystemVideoEnd(VideoPlayer source)
    {
        managerInst.GoToScene(nextSceneToLoadBase);
    }
}
