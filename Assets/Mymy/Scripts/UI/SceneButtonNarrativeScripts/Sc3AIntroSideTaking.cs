using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

[RequireComponent(typeof (SelectionProgress))]
public class Sc3AIntroSideTaking : MonoBehaviour
{
    [SerializeField]
    private Sc03AClient m_3AClientRef;
    [SerializeField]
    private SelectionProgress m_SelectionProgress;
    [SerializeField]
    private UIFader m_ButtonsContainer;
    [SerializeField]
    private bool m_DoTheDishes = false;

    private void Awake()
    {
        m_3AClientRef = m_3AClientRef ?? GetComponentInParent<Sc03AClient>();
        m_SelectionProgress = m_SelectionProgress ?? GetComponent<SelectionProgress>();
    }

    private void OnEnable()
    {
        m_SelectionProgress.OnSelectionComplete += HandleSelectionComplete;
    }

    private void OnDisable()
    {
        m_SelectionProgress.OnSelectionComplete -= HandleSelectionComplete;
    }

    private void HandleSelectionComplete()
    {
        if (m_DoTheDishes)
        {
            StartCoroutine(m_ButtonsContainer.FadeOut());
            m_3AClientRef.FadeOutSphere();
        }
        else
        {
            Scribe.Side03 = m_DoTheDishes;
            Sc03AClient.GoToSceneOnChoice();
        }
    }
}
