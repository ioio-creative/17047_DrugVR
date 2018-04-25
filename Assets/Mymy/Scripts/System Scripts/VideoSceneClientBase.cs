using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoSceneClientBase : MonoBehaviour
{
    [SerializeField]
    private DrugVR_SceneENUM nextSceneToLoad;
    private GameManager managerInst;

    private void Awake()
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
        managerInst.GoToScene(nextSceneToLoad);
    }
}
