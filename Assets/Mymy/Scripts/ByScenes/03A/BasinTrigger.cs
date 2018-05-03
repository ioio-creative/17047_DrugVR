using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasinTrigger : MonoBehaviour
{
    private const int m_PlatesTotal = 15;

    private int m_PlateCounter = 0;

    private void OnTriggerEnter(Collider targetObj)
    {
        if (targetObj.gameObject.layer == 10)
        {
            //Destroy(targetObj.gameObject.GetComponentInParent<PickableConfinedToPlane>().ConfinedPlane.gameObject);
            Destroy(targetObj.gameObject.GetComponentInParent<Rigidbody>().gameObject);
            m_PlateCounter++;
        }

        if (m_PlateCounter >= m_PlatesTotal)
        {
            Scribe.Side03 = true;
            Sc03AClient.GoToSceneOnChoice();
        }
        
    }
    

}
