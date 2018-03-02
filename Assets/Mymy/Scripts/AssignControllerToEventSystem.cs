using UnityEngine;

public class AssignControllerToEventSystem : MonoBehaviour
{
    private void Start()
    {
        WaveVR_ControllerInputModule evntSys = GameObject.Find("/System/EventSystem").GetComponent<WaveVR_ControllerInputModule>();
        evntSys.RightController = gameObject;     
    }
}
