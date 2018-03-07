using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasinTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider targetObj)
    {
        Destroy(targetObj.gameObject);
    }

}
