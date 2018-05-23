using DrugVR_Scribe;
using UnityEngine;

public class GoToSceneOnSelected : MonoBehaviour
{
    public DrugVR_SceneENUM SceneToGoNext
    {
        set
        {
            m_SceneToGoNext = value;
        }
    }
    [SerializeField]
    private DrugVR_SceneENUM m_SceneToGoNext;

    public SelectionStandard Selection
    {
        set
        {
            m_Selection = value;
        }
    }
    [SerializeField]
    private SelectionStandard m_Selection;

    private GameManager m_ManagerInstance;


    /* MonoBehaviour */

    private void OnEnable()
    {
        m_Selection.OnSelectionComplete += HandleSelectionComplete;
    }

    private void OnDisable()
    {
        m_Selection.OnSelectionComplete -= HandleSelectionComplete;
    }

    private void Start()
    {
        m_ManagerInstance = GameManager.Instance;
    }

    /* end of MonoBehaviour */


    /* event handlers */

    private void HandleSelectionComplete()
    {
        m_ManagerInstance.GoToScene(m_SceneToGoNext);
    }

    /* end of event handlers */
}
