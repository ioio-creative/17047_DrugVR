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

    protected override void OnEnable()
    {
        base.OnEnable();
        lighterTriggerProgressable.OnSelectionComplete +=
            HandleLighterTriggerProgressableSelectionComplete;
    }

    protected override void OnDisable()
    {
        base.OnEnable();
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
