using DrugVR_Scribe;
using UnityEngine;
using VRStandardAssets.Utils;

public class Sc06AClient : VideoSceneClientBase
{
    [SerializeField]
    private const DrugVR_SceneENUM nextSceneToLoadWhenLighterTriggered = DrugVR_SceneENUM.Sc06B;
    [SerializeField]
    private const DrugVR_SceneENUM nextSceneToLoadWhenHandWaveTriggered = DrugVR_SceneENUM.Sc07;

    [SerializeField]
    private LighterTriggerProgress lighterTriggerProgress;
    [SerializeField]
    private HandWaveProgressNew handWaveProgress;

    [SerializeField]
    private UIFader[] fadersToStart;


    /* MonoBehaviour */

    protected override void Awake()
    {
        base.Awake();

        // set default next scene
        nextSceneToLoadBase = nextSceneToLoadWhenHandWaveTriggered;

        foreach (UIFader fader in fadersToStart)
        {
            StartCoroutine(fader.InterruptAndFadeIn());
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        lighterTriggerProgress.OnSelectionFadeOutComplete +=
            HandleLighterTriggerProgressSelectionFadeOutComplete;
        handWaveProgress.OnSelectionFadeOutComplete +=
            HandlerHandWaveProgressSelectionFadeOutComplete;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        lighterTriggerProgress.OnSelectionFadeOutComplete -=
            HandleLighterTriggerProgressSelectionFadeOutComplete;
        handWaveProgress.OnSelectionFadeOutComplete -=
            HandlerHandWaveProgressSelectionFadeOutComplete;
    }

    /* end of MonoBehaviour */


    /* event handlers */

    private void HandlerHandWaveProgressSelectionFadeOutComplete()
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
