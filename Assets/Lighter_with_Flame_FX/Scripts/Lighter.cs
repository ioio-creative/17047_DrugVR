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

    void Start ()
    {
        FlameFX.SetActive(false);
        FlameLight.SetActive(false);

        waveVrDevice = WaveVR_Controller.Input(m_DeviceToListen);
    }  
  
  
    void Update ()
    {
	    if (waveVrDevice.GetPressDown(m_InputToListen)) //check to see if the left mouse was pressed
        {
		    StartCoroutine(LighterOn());
        }

        if (waveVrDevice.GetPressUp(m_InputToListen))
        {
            LighterMesh.GetComponent<Animation>().Play("Release Button");
            FlameFX.SetActive(false);
            FlameLight.SetActive(false);
            LighterAudio.Stop();
        }         
    }
 
 
    IEnumerator LighterOn ()
    {

         LighterIgniteAudio.Play();

         LighterAudio.Play();

         LighterMesh.GetComponent<Animation>().Play("Push Button");

         FlameFX.SetActive(true);
    
         FlameLight.SetActive(true);

	     yield return new WaitForSeconds (2);

    }

}