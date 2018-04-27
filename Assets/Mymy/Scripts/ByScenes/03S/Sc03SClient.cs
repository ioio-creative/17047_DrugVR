using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc03SClient : MonoBehaviour
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc02A;

    private void Awake()
    {

    }

    private void GoToSceneOnChoice()
    {
        GameManager.Instance.GoToScene(nextSceneToLoad);
    }

}
