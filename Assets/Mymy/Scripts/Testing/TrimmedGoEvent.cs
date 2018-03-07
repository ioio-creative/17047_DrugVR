using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrimmedGoEvent : MonoBehaviour, 
    IHandlePointer
{

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown: TrimmedGoEvent");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter: TrimmedGoEvent");

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.SetColor("_Color", Color.yellow);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit: TrimmedGoEvent");

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.SetColor("_Color", Color.red);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown: TrimmedGoEvent");
    }
}
