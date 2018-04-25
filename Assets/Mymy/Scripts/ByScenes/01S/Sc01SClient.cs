using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc01SClient : MonoBehaviour
{
    [SerializeField]
    private DrugVR_SceneENUM nextSceneToLoad;

    private GameManager managerInst = GameManager.Instance;

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
