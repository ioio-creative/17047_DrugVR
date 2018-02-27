using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyGoEvent : MonoBehaviour,
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
    [SerializeField]
    private string m_ObjectName; 


    /* MonoBehaviour */

    private void Start()
    {
		
	}
		
	private void Update()
    {
		
	}

    /* end of MonoBehaviour */


    /* UnityEngine.EventSystems pointer event handler interface */

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag: " + m_ObjectName);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag: " + m_ObjectName);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop: " + m_ObjectName);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag: " + m_ObjectName);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown: " + m_ObjectName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter: " + m_ObjectName);

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.SetColor("_Color", Color.yellow);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit: " + m_ObjectName);

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.SetColor("_Color", Color.red);
        }
    }

    public void OnPointerHover(PointerEventData eventData)
    {
        Debug.Log("OnPointerHover: " + m_ObjectName);

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.SetColor("_Color", Color.green);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp: " + m_ObjectName);
    }

    /* end of UnityEngine.EventSystems pointer event handler interface */
}
