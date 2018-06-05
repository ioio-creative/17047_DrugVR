using DrugVR_Scribe;
using UnityEngine;

[RequireComponent(typeof(SelectionProgress))]
public class Side01Button : MonoBehaviour
{
    [SerializeField]
    private Sc01SClient m_sc01SClient;
    [SerializeField]
    private SelectionProgress m_SelectionProgress;
    [SerializeField]
    private bool m_Patience = false;

    private void Awake()
    {
        if (m_SelectionProgress == null)
        {
            m_SelectionProgress = GetComponent<SelectionProgress>();
        }
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
        if (m_Patience)
        {
            m_SelectionProgress.PlayOnSelectedClip();
        }
        else
        {
            m_SelectionProgress.PlayOnErrorClip();
        }
        Scribe.Side01 = m_Patience;
        m_sc01SClient.GoToSceneOnChoice();
    }
}
