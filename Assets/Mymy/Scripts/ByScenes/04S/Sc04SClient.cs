using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc04SClient : MonoBehaviour
{
    public static GameManager managerInst;

    private void Awake()
    {
        managerInst = GameManager.Instance;
    }

    public static void GoToSceneOnChoice()
    {
        if (managerInst.Side04)
        {
            managerInst.GoToScene(DrugVR_SceneENUM.Sc04A);
        }
        else
        {
            managerInst.GoToScene(DrugVR_SceneENUM.Sc04B);
        }
    }
}
