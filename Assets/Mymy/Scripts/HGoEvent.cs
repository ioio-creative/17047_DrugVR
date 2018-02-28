using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HGoEvent : MonoBehaviour,    
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerUpHandler,
    IPointerDownHandler,
    IPointerHoverHandler,
    IDragHandler,
    IBeginDragHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerHover(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    

    void Start ()
    {
		
	}

    void Update ()
    {
		
	}

    public void OnDrag(PointerEventData eventData)
    {
        Transform transform = GetComponent<Transform>();
        Vector3 position = transform.localPosition;
        position.x += eventData.delta.x;
        position.y += eventData.delta.y;
        transform.localPosition = position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Transform transform = GetComponent<Transform>();
        Vector3 position = transform.localPosition;
        position.x += eventData.delta.x;
        position.y += eventData.delta.y;
        transform.localPosition = position;
    }
}
