using DrugVR_Scribe;
using UnityEngine;

public class Sc06AClient : VideoSceneClientBase
{
    [SerializeField]
    private LighterTriggerProgressable lighterTriggerProgressable;


    /* MonoBehaviour */

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        lighterTriggerProgressable.OnSelectionComplete +=
            HandleLighterTriggerProgressableSelectionComplete;
    }

    private void OnDisable()
    {
        lighterTriggerProgressable.OnSelectionComplete -=
            HandleLighterTriggerProgressableSelectionComplete;
    }

    private void Start()
    {
        
    }

    /* end of MonoBehaviour */


    /* event handlers */

    private void HandleLighterTriggerProgressableSelectionComplete()
    {
        GameManager.Instance.GoToScene(DrugVR_SceneENUM.Sc06B);
    }

    /* end of event handlers */
}
