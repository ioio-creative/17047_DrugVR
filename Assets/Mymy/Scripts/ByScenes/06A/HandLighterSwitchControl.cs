using DrugVR_Scribe;
using UnityEngine;
using wvr;

public class HandLighterSwitchControl : MonoBehaviour
{
    private const string ControllerPosTrkManObjectName = "/VIVEFocusWaveVR/FocusController";
    private const string OriginalControllerModelObjectName = "/VIVEFocusWaveVR/FocusController/MIA_Ctrl";

    [SerializeField]
    private WVR_DeviceType m_DeviceToListen = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    [SerializeField]
    private WVR_InputId m_InputToListen = WVR_InputId.WVR_InputId_Alias1_Touchpad;

    [SerializeField]
    private LighterTriggerProgress lighterProgress;
    [SerializeField]
    private HandWaveProgressNew handWaveProgress;

    [SerializeField]
    private GameObject lighter;

    private GameObject controllerPosTrkMan;
    private GameObject originalControllerModel;
    private Transform originalControllerModelTransform;
    private Vector3 originalControllerModelInitialScale;
    private LaserPointer controllerLaser;
    private WaveVR_ControllerPoseTracker controllerPT;

    private WaveVR_Controller.Device waveVrDevice;
    private bool isLighterOn = false;


    /* MonoBehaviour */

    private void OnDestroy()
    {
        GameManager.OnSceneChange -= HandleSceneChange;
    }

    private void Start()
    {
        controllerPosTrkMan = GameObject.Find(ControllerPosTrkManObjectName);
        originalControllerModel = GameObject.Find(OriginalControllerModelObjectName);
        originalControllerModelTransform = originalControllerModel.transform;
        originalControllerModelInitialScale = originalControllerModelTransform.localScale;
        controllerLaser = FindObjectOfType<LaserPointer>();
        controllerPT = controllerPosTrkMan.GetComponent<WaveVR_ControllerPoseTracker>();

        GameManager.OnSceneChange += HandleSceneChange;

        lighterProgress.enabled = true;
        handWaveProgress.enabled = false;

        // !!! Important !!!
        // Make sure this is run before LaserPointer's Start()
        // by setting script execution order
        ReplaceControllerByLighter();

        lighter.transform.parent = controllerPosTrkMan.transform;

        waveVrDevice = WaveVR_Controller.Input(m_DeviceToListen);        
    }

    private void Update()
    {        
        bool isPress = waveVrDevice.GetPress(m_InputToListen);     

        if (isPress)
        {
            lighterProgress.enabled = false;
            handWaveProgress.enabled = true;

            ReplaceLighterByController();
        }
        else
        {
            lighterProgress.enabled = true;
            handWaveProgress.enabled = false;

            ReplaceControllerByLighter();
        }
    }

    /* end of MonoBehaviour */


    private void ReplaceControllerByLighter()
    {
        if (!isLighterOn)
        {
            lighter.SetActive(true);

            controllerPT.TrackRotation = false;            

            controllerLaser.enabled = false;
            controllerLaser.IsEnableReticle = false;
            controllerLaser.IsEnableBeam = false;
            originalControllerModel.SetActive(false);
            originalControllerModelTransform.localScale = Vector3.zero;

            isLighterOn = true;
        }
    }

    private void ReplaceLighterByController()
    {
        if (isLighterOn)
        {
            lighter.SetActive(false);

            controllerPT.TrackRotation = true;
            originalControllerModel.SetActive(true);
            originalControllerModelTransform.localScale = originalControllerModelInitialScale;
            controllerLaser.enabled = true;
            controllerLaser.IsEnableReticle = true;
            controllerLaser.IsEnableBeam = true;

            isLighterOn = false;
        }
    }


    /* event handlers */

    private void HandleSceneChange(DrugVR_SceneENUM nextScene)
    {
        enabled = false;
        lighter.SetActive(false);
        Destroy(lighter);
    }

    /* end of event handlers */
}
