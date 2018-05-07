using DrugVR_Scribe;
using UnityEngine;

public class Sc06AClient : VideoSceneClientBase
{
    [SerializeField]
    private LighterTriggerProgress lighterTriggerProgress;


    /* MonoBehaviour */

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        lighterTriggerProgress.OnSelectionComplete +=
            HandleLighterTriggerProgressSelectionComplete;
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        lighterTriggerProgress.OnSelectionComplete -=
            HandleLighterTriggerProgressSelectionComplete;
    }

    private void Start()
    {
        
    }

    /* end of MonoBehaviour */


    /* event handlers */

    private void HandleLighterTriggerProgressSelectionComplete()
    {
        //GameManager.Instance.GoToScene(DrugVR_SceneENUM.Sc06B);
    }

    /* end of event handlers */
}
