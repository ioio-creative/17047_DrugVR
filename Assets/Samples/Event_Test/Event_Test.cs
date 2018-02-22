using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;

public class Event_Test : MonoBehaviour
{
    void OnEvent(params object[] args)
    {
        var _event = (WVR_EventType)args [0];
        Log.d ("Event_Test", "OnEvent() _event = " + _event);

        switch (_event)
        {
        case WVR_EventType.WVR_EventType_TouchpadSwipe_LeftToRight:
            transform.Rotate (0, 180 * (10 * Time.deltaTime), 0);
            break;
        case WVR_EventType.WVR_EventType_TouchpadSwipe_RightToLeft:
            transform.Rotate (0, -180 * (10 * Time.deltaTime), 0);
            break;
        case WVR_EventType.WVR_EventType_TouchpadSwipe_DownToUp:
            transform.Rotate (0, 0, 180 * (10 * Time.deltaTime));
            break;
        case WVR_EventType.WVR_EventType_TouchpadSwipe_UpToDown:
            transform.Rotate (0, 0, -180 * (10 * Time.deltaTime));
            break;
        }
    }

    void OnEnable()
    {
        WaveVR_Utils.Event.Listen ("SWIPE_EVENT", OnEvent);
    }

    void OnDisable()
    {
        WaveVR_Utils.Event.Remove ("SWIPE_EVENT", OnEvent);
    }
}
