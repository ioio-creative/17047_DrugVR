using UnityEngine;

public class FadeSceneMenuOnSelected : MonoBehaviour
{
    public SelectionStandard Selection
    {
        set
        {
            m_Selection = value;
        }
    }
    [SerializeField]
    private SelectionStandard m_Selection;

    public IShowAndHideSceneMenu SceneMenuControl
    {
        set
        {
            m_SceneMenuControl = value;
        }
    }
    [SerializeField]
    private IShowAndHideSceneMenu m_SceneMenuControl;


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
		
	}

    /* end of MonoBehaviour */


    /* event handlers */

    private void HandleSelectionComplete()
    {
        m_SceneMenuControl.HideSceneMenu();
    }

    /* end of event handlers */
}
