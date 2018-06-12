using DrugVR_Scribe;
using UnityEngine;
using VRStandardAssets.Utils;

public class Sc06AClient : VideoSceneClientBase
{
    private const DrugVR_SceneENUM nextSceneToLoadWhenLighterTriggered = DrugVR_SceneENUM.Sc06B;
    private const DrugVR_SceneENUM nextSceneToLoadWhenHandWaveTriggered = DrugVR_SceneENUM.Sc07;

    [SerializeField]
    private LighterTriggerProgress lighterTriggerProgress;
    [SerializeField]
    private HandWaveProgressNew handWaveProgress;

    /* MonoBehaviour */

    protected override void Awake()
    {
        base.Awake();

        // set default next scene
        nextSceneToLoadBase = nextSceneToLoadWhenHandWaveTriggered;

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        lighterTriggerProgress.OnSelectionFadeOutComplete +=
            HandleLighterTriggerProgressSelectionFadeOutComplete;
        handWaveProgress.OnSelectionFadeOutComplete +=
            HandleHandWaveProgressSelectionFadeOutComplete;
        handWaveProgress.OnSelectionComplete += HandleHandWaveProgressSelectionComplete;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        lighterTriggerProgress.OnSelectionFadeOutComplete -=
            HandleLighterTriggerProgressSelectionFadeOutComplete;
        handWaveProgress.OnSelectionFadeOutComplete -=
            HandleHandWaveProgressSelectionFadeOutComplete;
    }

    /* end of MonoBehaviour */


    /* event handlers */

    private void HandleHandWaveProgressSelectionComplete()
    {
        lighterTriggerProgress.CheckAndFadeOutHandWaveInstruction();
        BackgroundAudioControl.Instance.MasterFadeBackgroundAudioToTargetVolume(0.3f, 1.5f);
    }

    private void HandleHandWaveProgressSelectionFadeOutComplete()
    {
        Scribe.Side05 = true;
        nextSceneToLoadBase = nextSceneToLoadWhenHandWaveTriggered;
        managerInst.PlayVideo();
        // Going to next scene will be done on play video end, by base class.
    }

    private void HandleLighterTriggerProgressSelectionFadeOutComplete()
    {
        Scribe.Side05 = false;
        nextSceneToLoadBase = nextSceneToLoadWhenLighterTriggered;
        GoToNextScene();
    }
    
    /* end of event handlers */
}
