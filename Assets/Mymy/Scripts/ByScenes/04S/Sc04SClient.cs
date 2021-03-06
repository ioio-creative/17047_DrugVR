﻿using DrugVR_Scribe;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class Sc04SClient : MonoBehaviour
{   
    [SerializeField]
    private GameObject[] m_FaderToStartContainers;
    private UIFader[] m_FadersToStart;

    private GameManager m_ManagerInst;


    /* MonoBehaviour */

    private void Awake()
    {
        m_ManagerInst = GameManager.Instance;

        List<UIFader> faders = new List<UIFader>();
        foreach (GameObject faderToStartContainer in m_FaderToStartContainers)
        {
            // check if the game object contains UIFader component
            UIFader fader = faderToStartContainer.GetComponent<UIFader>();

            if (fader)
            {
                faders.Add(fader);
            }
            // if the game object does not contain UIFader component,
            // check the game object's children
            else
            {
                faders.AddRange(faderToStartContainer.GetComponentsInChildren<UIFader>());
            }
        }
        m_FadersToStart = faders.ToArray();
    }

    private void Start()
    {
        foreach (UIFader fader in m_FadersToStart)
        {
            StartCoroutine(fader.InterruptAndFadeIn());
        }
    }

    /* end of MonoBehaviour */


    public void GoToSceneOnChoice()
    {
        if (Scribe.Side04)
        {
            m_ManagerInst.GoToScene(DrugVR_SceneENUM.Sc04A);
        }
        else
        {
            m_ManagerInst.GoToScene(DrugVR_SceneENUM.Sc04B);
        }
    }
}
