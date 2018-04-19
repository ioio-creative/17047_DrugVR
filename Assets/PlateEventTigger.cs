using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WaveVR_Log;

public class PlateEventTigger : MonoBehaviour,
    IDragHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    #region override event handling function
    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PlateEventTrigger.OnPointerDown");
        gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    } 
    #endregion
}
