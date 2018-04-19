// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WaveVR_Log;
using wvr;
public class Go_Event: MonoBehaviour,
IPointerUpHandler,
IPointerEnterHandler,
IPointerExitHandler,
IPointerDownHandler,
IBeginDragHandler,
IDragHandler,
IEndDragHandler,
IDropHandler,
IPointerHoverHandler
{
    private const string LOG_TAG = "WaveVR_HelloVR";
    Vector3 originalPos;
    WVR_DeviceType curFocusControllerType=WVR_DeviceType.WVR_DeviceType_HMD;
    public  bool isControllerFocus_R;
    public bool  isControllerFocus_L;
    private GameObject m_RightController;
    private GameObject m_LeftController;
    void Start(){
       originalPos=transform.position;
    }

    void Update()
    {
        if (isControllerFocus_R || isControllerFocus_L)
        {
            // Log.d(LOG_TAG,"Bumber Down :"+WaveVR_Controller.Input(curFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Bumper));
            // Log.d(LOG_TAG,"Bumber Up :"+WaveVR_Controller.Input(curFocusControllerType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Bumper));
            if (WaveVR_Controller.Input(curFocusControllerType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Touchpad) ||
                             WaveVR_Controller.Input(curFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Bumper))
            {
                moveSphere();
            }
        }
            
        if(Input.GetKey(KeyCode.Escape)){
            Application.Quit();
        }
    }

    void moveSphere()
    {
        if (originalPos != transform.position)
        {
            transform.position = originalPos;
            return;
        }
        if (curFocusControllerType == WVR_DeviceType.WVR_DeviceType_Controller_Right)
        {
            transform.position=new Vector3(1, originalPos.y, originalPos.z);
        }
        else
        {
            transform.position = new Vector3(-1, originalPos.y, originalPos.z);

        }
        //GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
    }


    public void OnPointerUp(PointerEventData eventData){
        //Debug.Log("OnPointerUp");
    }

    public void OnPointerEnter (PointerEventData eventData)
    {
        WVR_DeviceType type = eventData.enterEventCamera.gameObject.GetComponent<WaveVR_PoseTrackerManager>().Type;
        GameObject target=eventData.enterEventCamera.gameObject;
        if (target.GetComponent<WaveVR_PoseTrackerManager>())
        {
            if (type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
            {
                m_RightController = target;
                isControllerFocus_R = true;
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            }
            else if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
            {
                m_LeftController = target;
                isControllerFocus_L = true;
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
            }
        }
        curFocusControllerType =type;
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        RaycastHit hit;
        if(m_RightController && isControllerFocus_R){ //R_Controller Leave
            Vector3 fwd_R = m_RightController.transform.TransformDirection(Vector3.forward);
            if (!Physics.Raycast(m_RightController.transform.position, fwd_R, out hit))
            {

                isControllerFocus_R = false;
                if (isControllerFocus_L)
                {
                    curFocusControllerType = WVR_DeviceType.WVR_DeviceType_Controller_Left;
                    GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
                    return;
                }
            }
        }

        if(m_LeftController && isControllerFocus_L){ //L_Controller Leave
            Vector3 fwd_L = m_LeftController.transform.TransformDirection(Vector3.forward);
            if (!Physics.Raycast(m_LeftController.transform.position, fwd_L, out hit))
            {
                isControllerFocus_L = false;
                if (isControllerFocus_R)
                {
                    curFocusControllerType = WVR_DeviceType.WVR_DeviceType_Controller_Right;
                    GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                    return;
                }
            }
        }
        curFocusControllerType = WVR_DeviceType.WVR_DeviceType_HMD;

        GetComponent<MeshRenderer> ().material.SetColor ("_Color", Color.green);
     }

     public void OnPointerDown (PointerEventData eventData)
     {
        //Debug.Log("OnPointerDown");

     }

     public void OnBeginDrag(PointerEventData eventData)
     {
        //Debug.Log("OnBeginDrag");
     }

     public void OnDrag(PointerEventData eventData)
     {
        //Debug.Log("OnDrag");
     }


     public void OnEndDrag(PointerEventData eventData)
     {
         //Debug.Log("OnEndDrag");
     }

     public void OnDrop(PointerEventData eventData)
     {
         //Debug.Log("OnDrop");
     }

     public void OnPointerHover (PointerEventData eventData)
     {
         // Debug.Log("OnPointerHover: "+eventData.enterEventCamera.gameObject);
     }
}