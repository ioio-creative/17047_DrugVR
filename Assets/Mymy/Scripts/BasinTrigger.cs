using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasinTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider targetObj)
    {
        if (targetObj.gameObject.layer == 10)
        {
            Destroy(targetObj.gameObject.GetComponentInParent<PickableConfinedToPlane>().ConfinedPlane.gameObject);
            Destroy(targetObj.gameObject.GetComponentInParent<Rigidbody>().gameObject);
        }
        
    }

}
