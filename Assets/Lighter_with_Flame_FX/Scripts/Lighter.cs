using UnityEngine;
using System.Collections;
using wvr;

public class Lighter : MonoBehaviour
{
    public GameObject LighterMesh;
    public GameObject FlameFX;
    public GameObject FlameLight;
    public AudioSource LighterIgniteAudio;
    public AudioSource LighterAudio;

    [SerializeField]
    private WVR_DeviceType m_DeviceToListen = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    [SerializeField]
    private WVR_InputId m_InputToListen;
    private WaveVR_Controller.Device waveVrDevice;

    private bool m_IsTriggerPressDown;
    private bool m_IsTriggerPressUp;


    private void Start()
    {
        FlameFX.SetActive(false);
        FlameLight.SetActive(false);

        waveVrDevice = WaveVR_Controller.Input(m_DeviceToListen);
    }

    private void Update()
    {
#if UNITY_EDITOR
        /*
            https://docs.unity3d.com/ScriptReference/Input.GetMouseButtonDown.html
            button values are 0 for the primary button (often the left button),
            1 for secondary button,
            and 2 for the middle button
        */
        m_IsTriggerPressDown = Input.GetMouseButtonDown(0);
        m_IsTriggerPressUp = Input.GetMouseButtonUp(0);
#else
        m_IsTriggerPressDown = waveVrDevice.GetPressDown(m_InputToListen);
        m_IsTriggerPressUp = waveVrDevice.GetPressUp(m_InputToListen);
#endif

        if (m_IsTriggerPressDown)
        {
		    StartCoroutine(LighterOn());
        }

        if (m_IsTriggerPressUp)
        {
            LighterMesh.GetComponent<Animation>().Play("Release Button");
            FlameFX.SetActive(false);
            FlameLight.SetActive(false);
            LighterAudio.Stop();
        }         
    }
 
 
    private IEnumerator LighterOn()
    {
         LighterIgniteAudio.Play();
         LighterAudio.Play();
         LighterMesh.GetComponent<Animation>().Play("Push Button");
         FlameFX.SetActive(true);   
         FlameLight.SetActive(true);
	     yield return new WaitForSeconds (2);
    }
}