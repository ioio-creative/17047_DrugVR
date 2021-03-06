using UnityEngine;
using System.Collections;
using wvr;

public class Lighter : MonoBehaviour
{
    public WVR_DeviceType DeviceToListen { get { return m_DeviceToListen; } }
    public WVR_InputId InputToListen { get { return m_InputToListen; } }
    public int EditorUseMouseButton { get { return m_EditorUseMouseButton; } }
        
    public GameObject LighterMesh;
    public GameObject FlameFX;
    public GameObject FlameLight;
    public AudioSource LighterIgniteAudio;
    public AudioSource LighterAudio;
    
    [SerializeField]
    private WVR_DeviceType m_DeviceToListen;
    [SerializeField]
    private WVR_InputId m_InputToListen;
#if !UNITY_EDITOR
    private WaveVR_Controller.Device m_WaveVrDevice;
#endif
    /*
        https://docs.unity3d.com/ScriptReference/Input.GetMouseButtonDown.html
        button values are 0 for the primary button (often the left button),
        1 for secondary button,
        and 2 for the middle button
    */
    [SerializeField]
    private int m_EditorUseMouseButton = 0;

    private bool m_IsInputPressDown;
    private bool m_IsInputPressUp;

    private void Awake()
    {        
        if (DeviceToListen == WVR_DeviceType.WVR_DeviceType_Controller_Right)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (DeviceToListen == WVR_DeviceType.WVR_DeviceType_Controller_Left)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void Start()
    {
        FlameFX.SetActive(false);
        FlameLight.SetActive(false);

#if !UNITY_EDITOR
        m_WaveVrDevice = WaveVR_Controller.Input(m_DeviceToListen); 
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        m_IsInputPressDown = Input.GetMouseButtonDown(m_EditorUseMouseButton);
        m_IsInputPressUp = Input.GetMouseButtonUp(m_EditorUseMouseButton);
#else
        m_IsInputPressDown = m_WaveVrDevice.GetPressDown(m_InputToListen);
        m_IsInputPressUp = m_WaveVrDevice.GetPressUp(m_InputToListen);
#endif

        if (m_IsInputPressDown)
        {
		    StartCoroutine(LighterOn());
        }

        if (m_IsInputPressUp)
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