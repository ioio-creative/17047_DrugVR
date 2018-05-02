using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sc01SClient : MonoBehaviour
{
    //private DrugVR_SceneENUM nextSceneToLoad;
    public static GameManager managerInst;

    private void Awake()
    {
        managerInst = GameManager.Instance;
    }

    public static void GoToSceneOnChoice()
    {
        if (Scribe.Side01)
        {
            managerInst.GoToScene(DrugVR_SceneENUM.Sc01A);
        }
        else
        {
            managerInst.GoToScene(DrugVR_SceneENUM.Sc01B);
        }
    }

}