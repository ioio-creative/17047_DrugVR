using DrugVR_Scribe;
using UnityEngine;

public class Sc06AClient : VideoSceneClientBase
{
    [SerializeField]
    private LighterTriggerProgress lighterTriggerProgress;
    [SerializeField]
    private HandWaveProgressNew handWaveProgress;


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
        handWaveProgress.OnSelectionComplete +=
            HandlerHandWaveProgressSelectionComplete;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        lighterTriggerProgress.OnSelectionComplete -=
            HandleLighterTriggerProgressSelectionComplete;
        handWaveProgress.OnSelectionComplete -=
            HandlerHandWaveProgressSelectionComplete;
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

    private void HandlerHandWaveProgressSelectionComplete()
    {
        //GameManager.Instance.GoToScene(DrugVR_SceneENUM.Sc06B);
    }

    /* end of event handlers */
}
