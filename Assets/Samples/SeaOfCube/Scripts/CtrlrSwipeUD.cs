using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;

public class CtrlrSwipeUD : MonoBehaviour
{
    void OnEvent(params object[] args)
    {
        var _event = (WVR_EventType)args[0];
        Log.d("CtrlrSwipeUD", "OnEvent() _event = " + _event);

        switch (_event)
        {
            case WVR_EventType.WVR_EventType_TouchpadSwipe_DownToUp:
                transform.Rotate(30, 0, 0);
                break;
            case WVR_EventType.WVR_EventType_TouchpadSwipe_UpToDown:
                transform.Rotate(-30, 0, 0);
                break;
        }
    }

    void OnEnable()
    {
        WaveVR_Utils.Event.Listen("SWIPE_EVENT", OnEvent);
    }

    void OnDisable()
    {
        WaveVR_Utils.Event.Remove("SWIPE_EVENT", OnEvent);
    }
}

