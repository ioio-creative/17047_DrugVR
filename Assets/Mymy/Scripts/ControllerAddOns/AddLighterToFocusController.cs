using DrugVR_Scribe;
using UnityEngine;

public class AddLighterToFocusController : MonoBehaviour
{
    private const string ControllerPosTrkManObjectName = "/VIVEFocusWaveVR/FocusController";
    private const string OriginalControllerModelObjectName = "/VIVEFocusWaveVR/FocusController/MIA_Ctrl";

    [SerializeField]
    private GameObject lighterObject;
    private GameObject controllerPosTrkMan;
    private GameObject originalControllerModel;    
    private LaserPointer controllerLaser;
    private WaveVR_ControllerPoseTracker controllerPT;

    private bool isLighterOn = false;


    /* MonoBehaviour */

    private void Start ()
    {
        GameManager.OnSceneChange += HandleSceneChange;
        ReplaceControllerByLighter();
    }

    private void OnDisable()
    {
        ReplaceLighterByController();
    }

    private void OnDestroy()
    {
        GameManager.OnSceneChange -= HandleSceneChange;
    }

    private void LateUpdate()
    {
        //transform.localRotation = Quaternion.Euler(-transform.parent.eulerAngles.x, -transform.parent.eulerAngles.y, -transform.parent.eulerAngles.z);

        //Debug.Log(transform.position);
    }

    /* end of MonoBehaviour */


    /* event handlers */

    private void HandleSceneChange(DrugVR_SceneENUM nextScene)
    {
        enabled = false;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    /* end of event handlers */


    public void ReplaceControllerByLighter()
    {
        if (!isLighterOn)
        {
            controllerPosTrkMan = GameObject.Find(ControllerPosTrkManObjectName);
            originalControllerModel = GameObject.Find(OriginalControllerModelObjectName);
            controllerLaser = FindObjectOfType<LaserPointer>();

            controllerPT = controllerPosTrkMan.GetComponent<WaveVR_ControllerPoseTracker>();
            controllerPT.TrackRotation = false;

            transform.parent = controllerPosTrkMan.transform;

            controllerLaser.enabled = false;
            controllerLaser.IsEnableReticle = false;
            originalControllerModel.SetActive(false);
            originalControllerModel.transform.localScale = Vector3.zero;

            lighterObject.SetActive(true);
            isLighterOn = true;
        }
    }

    public void ReplaceLighterByController()
    {
        if (isLighterOn)
        {
            controllerPT.TrackRotation = true;
            originalControllerModel.SetActive(true);
            originalControllerModel.transform.localScale = Vector3.one;
            controllerLaser.enabled = true;
            controllerLaser.IsEnableReticle = true;

            lighterObject.SetActive(false);
            isLighterOn = false;
        }
    }
}
