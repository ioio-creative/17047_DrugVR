using DrugVR_Scribe;
using UnityEngine;
using wvr;

public class HandLighterSwitchControl : MonoBehaviour
{    
    [SerializeField]
    private WVR_DeviceType deviceToListen = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    [SerializeField]
    private WVR_InputId inputToListen = WVR_InputId.WVR_InputId_Alias1_Touchpad;

    [SerializeField]
    private LighterTriggerProgress lighterProgress;
    [SerializeField]
    private HandWaveProgressNew handWaveProgress;
    private bool modelSwitch = true;

    [SerializeField]
    private GameObject lighterObject;
    private Transform lighterTransform;
    //private readonly Quaternion LighterFixedQuaternion = Quaternion.Euler(0, 0, 0);

    [SerializeField]
    private float lighterHeadDistanceMultiplier;

    private GameObject controllerPosTrkMan;
    private GameObject originalControllerModel;
    private Transform originalControllerModelTransform;
    private Vector3 originalControllerModelInitialScale;
    private LaserPointer controllerLaser;
    private WaveVR_ControllerPoseTracker controllerPT;

    private WaveVR_Controller.Device waveVrDevice;
    private bool isLighterOn = false;

    private Transform headTransform;


    /* MonoBehaviour */

    private void OnDestroy()
    {
        GameManager.OnSceneChange -= HandleSceneChange;
    }

    private void Awake()
    {
        controllerPosTrkMan = GameManager.Instance.FocusControllerObject;
        originalControllerModel = GameManager.Instance.ControllerModelObject;
        originalControllerModelTransform = originalControllerModel.transform;
        originalControllerModelInitialScale = originalControllerModelTransform.localScale;
        controllerLaser = controllerPosTrkMan.GetComponentInChildren<LaserPointer>();
        controllerPT = controllerPosTrkMan.GetComponent<WaveVR_ControllerPoseTracker>();

        lighterTransform = lighterObject.transform;

        headTransform = GameManager.Instance.HeadObject.transform;
    }

    private void OnEnable()
    {
        handWaveProgress.OnSelectionComplete += HandleHandWaveSelectionComplete;
        lighterProgress.OnSelectionComplete += HandleLighterSelectionComplete;
    }

    private void OnDisable()
    {
        handWaveProgress.OnSelectionComplete -= HandleHandWaveSelectionComplete;
        lighterProgress.OnSelectionComplete -= HandleLighterSelectionComplete;
    }

    private void Start()
    {
        GameManager.OnSceneChange += HandleSceneChange;

        lighterProgress.enabled = true;
        handWaveProgress.enabled = false;

        ReplaceControllerByLighter();        

        // The following statement is no longer needed
        // as update lighterTransform.position in the Update() method.
        //lighterTransform.parent = controllerPosTrkMan.transform;

        waveVrDevice = WaveVR_Controller.Input(deviceToListen);        
    }

    private void Update()
    {
        if (modelSwitch)
        {
            bool isPress = waveVrDevice.GetPress(inputToListen);

            // switch mode       
            if (isPress)
            {
                lighterProgress.enabled = false;
                handWaveProgress.enabled = true;
                lighterProgress.InterruptAndFadeOut();

                ReplaceLighterByController();
            }
            else
            {
                lighterProgress.enabled = true;
                handWaveProgress.enabled = false;
                handWaveProgress.InterruptAndFadeOutAndReset();

                ReplaceControllerByLighter();
            }
        }
        if (isLighterOn)
        {
            // update lighter transform
            lighterTransform.position = (originalControllerModelTransform.position - headTransform.position) * lighterHeadDistanceMultiplier;
            lighterTransform.rotation =
                Quaternion.LookRotation(lighterTransform.position - headTransform.position);
            // fix x-, z-axis rotation
            lighterTransform.rotation = Quaternion.Euler(0, lighterTransform.eulerAngles.y, 0);
        }
    }

    /* end of MonoBehaviour */


    private void ReplaceControllerByLighter()
    {
        if (!isLighterOn)
        {
            lighterObject.SetActive(true);

            // The following two statements are longer needed
            // as we no longer set the parent of lighterTransform to
            // the transform of FocusController GameObject.
            //controllerPT.TrackRotation = false;            
            //lighterTransform.rotation = LighterFixedQuaternion;

            controllerLaser.IsEnableReticle = false;
            controllerLaser.IsEnableBeam = false;
            controllerLaser.enabled = false;
            
            originalControllerModel.SetActive(false);
            originalControllerModelTransform.localScale = Vector3.zero;
            
            isLighterOn = true;         
        }
    }

    private void ReplaceLighterByController()
    {
        if (isLighterOn)
        {
            lighterObject.SetActive(false);

            // The following two statements are longer needed
            // as we no longer set the parent of lighterTransform to
            // the transform of FocusController GameObject.
            //controllerPT.TrackRotation = true;

            originalControllerModel.SetActive(true);
            originalControllerModelTransform.localScale = originalControllerModelInitialScale;

            //controllerLaser.enabled = true;
            //controllerLaser.IsEnableReticle = true;
            //controllerLaser.IsEnableBeam = true;

            isLighterOn = false;
        }
    }


    /* event handlers */

    private void HandleSceneChange(DrugVR_SceneENUM nextScene)
    {
        ReplaceLighterByController();
        enabled = false;        
        Destroy(lighterObject);
    }

    private void HandleHandWaveSelectionComplete()
    {
        handWaveProgress.enabled = false;
        modelSwitch = false;
    }

    private void HandleLighterSelectionComplete()
    {
        lighterProgress.enabled = false;
        modelSwitch = false;
    }

    /* end of event handlers */
}
