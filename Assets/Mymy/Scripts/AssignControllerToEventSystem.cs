using UnityEngine;

public class AssignControllerToEventSystem : MonoBehaviour
{
    private void Awake()
    {
        WaveVR_InputModuleManager imManager = GameObject.Find("/System/InputModuleManager").GetComponent<WaveVR_InputModuleManager>();
        imManager.Controller.RightController = gameObject;     
    }

}
