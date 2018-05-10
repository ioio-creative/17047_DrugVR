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
    private LaserPointer controllerLaser;
    private WaveVR_ControllerPoseTracker controllerPT;

    private WaveVR_Controller.Device waveVrDevice;
    private bool isLighterOn = false;


    /* MonoBehaviour */

    private void OnDisable()
    {
        ReplaceLighterByController();
    }

    private void OnDestroy()
    {
        GameManager.OnSceneChange -= HandleSceneChange;
    }

    private void Start()
    {
        controllerPosTrkMan = GameObject.Find(ControllerPosTrkManObjectName);
        originalControllerModel = GameObject.Find(OriginalControllerModelObjectName);
        controllerLaser = FindObjectOfType<LaserPointer>();
        controllerPT = controllerPosTrkMan.GetComponent<WaveVR_ControllerPoseTracker>();

        GameManager.OnSceneChange += HandleSceneChange;

        lighterProgress.enabled = true;
        handWaveProgress.enabled = false;
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
            originalControllerModel.SetActive(false);
            originalControllerModel.transform.localScale = Vector3.zero;

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
            originalControllerModel.transform.localScale = Vector3.one;
            controllerLaser.enabled = true;
            controllerLaser.IsEnableReticle = true;

            isLighterOn = false;
        }
    }


    /* event handlers */

    private void HandleSceneChange(DrugVR_SceneENUM nextScene)
    {
        enabled = false;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    /* end of event handlers */
}
