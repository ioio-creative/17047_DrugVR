using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (SelectionProgress))]
public class Sc3AIntroSideTaking : MonoBehaviour
{
    [SerializeField]
    private Sc03AClient m_3AClientRef;
    [SerializeField]
    private SelectionProgress m_SelectionProgress;
    [SerializeField]
    private GameObject m_ButtonsContainer;
    [SerializeField]
    private bool m_DoTheDishes = false;

    private void Awake()
    {
        if (m_3AClientRef == null) m_3AClientRef = GetComponentInParent<Sc03AClient>();
        if (m_SelectionProgress == null) m_SelectionProgress = GetComponent<SelectionProgress>();
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
            m_3AClientRef.FadeOutSphere();
        }
        else
        {
            Sc03AClient.managerInst.Side03 = m_DoTheDishes;
            Sc03AClient.GoToSceneOnChoice();
        }
    }
}
