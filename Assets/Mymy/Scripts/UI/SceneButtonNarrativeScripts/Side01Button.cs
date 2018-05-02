using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelectionProgress))]
public class Side01Button : MonoBehaviour
{
    [SerializeField]
    private Sc01SClient m_Sc01SClientRef;
    [SerializeField]
    private SelectionProgress m_SelectionProgress;
    [SerializeField]
    private bool m_Patience = false;

    private void Awake()
    {
        if (m_Sc01SClientRef == null) m_Sc01SClientRef = GetComponentInParent<Sc01SClient>();
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
        Scribe.Side01 = m_Patience;
        Sc01SClient.GoToSceneOnChoice();
    }
}
