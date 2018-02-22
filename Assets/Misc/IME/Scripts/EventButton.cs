using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityIME
{
    /// <summary>
    /// This class is used for Button to handle events from EventSystem.
    /// </summary>
    public class EventButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public void OnPointerEnter (PointerEventData eventData)
        {
            transform.localScale += new Vector3(0.2F, 0.2F, 0);
        }

        public void OnPointerExit (PointerEventData eventData)
        {
            transform.localScale -= new Vector3(0.2F, 0.2F, 0);
        }

        public void OnPointerClick (PointerEventData eventData)
        {
            if (gameObject.name.Equals("Change") || gameObject.name.Equals("Shift"))
                transform.localScale -= new Vector3(0.2F, 0.2F, 0);
        }
    }
}