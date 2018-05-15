using DrugVR_Scribe;
using UnityEngine;

public class Sc06AClient : VideoSceneClientBase
{
    [SerializeField]
    private DrugVR_SceneENUM sceneToGoWhenLighterTriggered = DrugVR_SceneENUM.Sc06B;
    private DrugVR_SceneENUM sceneToGoWhenHandWaveTriggered = DrugVR_SceneENUM.Sc07;

    [SerializeField]
    private LighterTriggerProgress lighterTriggerProgress;
    [SerializeField]
    private HandWaveProgressNew handWaveProgress;


    /* MonoBehaviour */

    protected override void Awake()
    {
        base.Awake();

        // set default next scene
        nextSceneToLoadBase = sceneToGoWhenHandWaveTriggered;
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

    /* end of MonoBehaviour */


    /* event handlers */

    private void HandlerHandWaveProgressSelectionComplete()
    {
        Scribe.Side05 = true;
        nextSceneToLoadBase = sceneToGoWhenHandWaveTriggered;
        GameManager.PlayVideo();
        // Going to next scene will be done on play video end, by base class.
    }

    private void HandleLighterTriggerProgressSelectionComplete()
    {
        Scribe.Side05 = false;
        nextSceneToLoadBase = sceneToGoWhenLighterTriggered;
        GoToNextScene();
    }
    
    /* end of event handlers */
}
