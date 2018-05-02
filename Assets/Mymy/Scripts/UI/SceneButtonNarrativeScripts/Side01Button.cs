using DrugVR_Scribe;
using UnityEngine;

[RequireComponent(typeof(SelectionProgress))]
public class Side01Button : MonoBehaviour
{    
    [SerializeField]
    private SelectionProgress m_SelectionProgress;
    [SerializeField]
    private bool m_Patience = false;

    private void Awake()
    {        
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
