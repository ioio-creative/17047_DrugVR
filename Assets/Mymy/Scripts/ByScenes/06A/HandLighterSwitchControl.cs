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
    private Transform lighterTransform;
    //private readonly Quaternion LighterFixedQuaternion = Quaternion.Euler(0, 0, 0);

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

    private void Awake()
    {
        controllerPosTrkMan = GameObject.Find(ControllerPosTrkManObjectName);
        originalControllerModel = GameObject.Find(OriginalControllerModelObjectName);
        originalControllerModelTransform = originalControllerModel.transform;
        originalControllerModelInitialScale = originalControllerModelTransform.localScale;
        controllerLaser = FindObjectOfType<LaserPointer>();
        controllerPT = controllerPosTrkMan.GetComponent<WaveVR_ControllerPoseTracker>();

        lighterTransform = lighter.transform;
    }

    private void Start()
    {
        GameManager.OnSceneChange += HandleSceneChange;

        lighterProgress.enabled = true;
        handWaveProgress.enabled = false;

        ReplaceControllerByLighter();

        //lighterTransform.parent = controllerPosTrkMan.transform;

        waveVrDevice = WaveVR_Controller.Input(m_DeviceToListen);        
    }

    private void Update()
    {        
        //bool isPress = waveVrDevice.GetPress(m_InputToListen);     

        //if (isPress)
        //{
        //    lighterProgress.enabled = false;
        //    handWaveProgress.enabled = true;

        //    ReplaceLighterByController();
        //}
        //else
        //{
        //    lighterProgress.enabled = true;
        //    handWaveProgress.enabled = false;

        //    ReplaceControllerByLighter();
        //}

        if (isLighterOn)
        {
            lighterTransform.position = originalControllerModelTransform.position;
        }
    }

    /* end of MonoBehaviour */


    private void ReplaceControllerByLighter()
    {
        if (!isLighterOn)
        {
            lighter.SetActive(true);

            //controllerPT.TrackRotation = false;
            controllerPT.TrackRotation = true;

            controllerLaser.IsEnableReticle = false;
            controllerLaser.IsEnableBeam = false;
            controllerLaser.enabled = false;
            
            originalControllerModel.SetActive(false);
            originalControllerModelTransform.localScale = Vector3.zero;

            //lighterTransform.rotation = LighterFixedQuaternion;

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
