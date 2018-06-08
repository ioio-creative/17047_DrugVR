using DrugVR_Scribe;
using UnityEngine;
using wvr;

public class HandLighterSwitchControl : MonoBehaviour
{
    private const DrugVR_SceneENUM currentScene = DrugVR_SceneENUM.Sc06A;


    [SerializeField]
    private WVR_DeviceType deviceToListen = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    [SerializeField]
    private WVR_InputId inputToListen = WVR_InputId.WVR_InputId_Alias1_Touchpad;

    [SerializeField]
    private LighterTriggerProgress lighterProgress;
    [SerializeField]
    private HandWaveProgressNew handWaveProgress;
    private bool controllerModelReplaced = true;

    [SerializeField]
    private GameObject lighterObject;
    private Transform lighterTransform;
    //private readonly Quaternion LighterFixedQuaternion = Quaternion.Euler(0, 0, 0);

    [SerializeField]    
    private float lighterHeadDistanceMultiplier;

    private MyControllerSwtich controllerSwitch;
    private LaserPointer controllerLaser;
    private Transform controllerTransform;

    private WaveVR_Controller.Device waveVrDevice;
    private bool isLighterOn = false;

    private Transform headTransform;


    /* MonoBehaviour */

    

    private void Awake()
    {
        controllerSwitch = GameManager.Instance.ControllerSwitch;
        controllerLaser = controllerSwitch.LaserPointerRef;
        controllerTransform = controllerSwitch.ControllerTransform;

        lighterTransform = lighterObject.transform;        

        headTransform = GameManager.Instance.HeadObject.transform;

        GameManager.Instance.MenuControl.OnShowMenu += HandleShowMenu;
        GameManager.Instance.MenuControl.OnHideMenu += HandleHideMenu;
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

    private void OnDestroy()
    {
        GameManager.OnSceneChange -= HandleSceneChange;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.MenuControl.OnShowMenu -= HandleShowMenu;
            GameManager.Instance.MenuControl.OnHideMenu -= HandleHideMenu; 
        }
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
        if (controllerModelReplaced)
        {
            bool isPress = waveVrDevice.GetPress(inputToListen);

            // switch mode       
            if (isPress)
            {
                lighterProgress.enabled = false;
                handWaveProgress.enabled = true;
                lighterProgress.CheckAndFadeOut();

                ReplaceLighterByController();
            }
            else
            {
                lighterProgress.enabled = true;
                handWaveProgress.enabled = false;
                handWaveProgress.CheckAndFadeOutAndResetAndFadeInLighterInstruction();

                ReplaceControllerByLighter();
            }
        }

        if (isLighterOn)
        {
            //Debug.Log(controllerTransform.position.y);
            //RemapFloat(controllerTransform.position.y, -0.85637f, 0.0646f, -0.12f, 0.0646f)

            float oldControllerLowestY = -0.85637f;
            float oldControllerHighestY = 0.0646f;

            /* !!! Important !!! 
             * percentageToRaiseControllerY may be the only param that needs to be tuned
             * 0 <= percentageToRaiseControllerY <= 1
             */
            //float percentageToRaiseControllerY = 0;
            float percentageToRaiseControllerY = 0.387f;

            float newControllerLowestY =
                oldControllerLowestY + percentageToRaiseControllerY * (oldControllerHighestY - oldControllerLowestY);
            float newControllerHighestY = oldControllerHighestY;

            Vector3 remappedControllerPosCopy = new Vector3
            (
               controllerTransform.position.x,
               RemapFloat(controllerTransform.position.y, oldControllerLowestY, oldControllerHighestY, newControllerLowestY, newControllerHighestY),
               controllerTransform.position.z
            );
            
            // update lighter transform
            // calculate 'contrained' controller position wrt HMD position, then multiply with distance multiplier
            lighterTransform.position = (remappedControllerPosCopy - headTransform.position) * lighterHeadDistanceMultiplier + headTransform.position;

            /* !!! Important !!!
             * this is to make the lighter size looking the same, though it may not be needed
             */            
            //lighterTransform.localScale = lighterHeadDistanceMultiplier * lighterTransform.localScale;

            Quaternion headToLighterLook = Quaternion.LookRotation(lighterTransform.position - headTransform.position);        
            // fix x-, z-axis rotation
            lighterTransform.rotation = Quaternion.Euler(0, headToLighterLook.eulerAngles.y, 0);
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

            controllerSwitch.HideController();
            
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

            controllerSwitch.ShowController(false, false);

            isLighterOn = false;
        }
    }


    /* event handlers */

    private void HandleSceneChange(DrugVR_SceneENUM nextScene)
    {
        if (currentScene != nextScene)
        {
            if (lighterObject)
            {
                lighterObject.SetActive(false);
            }

            controllerSwitch.ShowController(true, true);

            enabled = false;
            Destroy(lighterObject);
        }
    }

    private void HandleHandWaveSelectionComplete()
    {
        handWaveProgress.enabled = false;
        controllerModelReplaced = false;
    }

    private void HandleLighterSelectionComplete()
    {
        lighterProgress.enabled = false;
        controllerModelReplaced = false;
    }

    private void HandleShowMenu()
    {
        handWaveProgress.gameObject.SetActive(false);
        lighterProgress.gameObject.SetActive(false);
        enabled = false;
    }

    private void HandleHideMenu()
    {
        handWaveProgress.gameObject.SetActive(true);
        lighterProgress.gameObject.SetActive(true);
        enabled = true;
    }

    /* end of event handlers */

    private float RemapFloat(float input, float from1, float to1, float from2, float to2)
    {
        input = Mathf.Clamp(input, from1, to1);
        return (input - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
