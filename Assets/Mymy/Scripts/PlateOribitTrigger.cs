using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateOribitTrigger : MonoBehaviour
{

    void OnTriggerEnter(Collider plateInOrbit)
    {
        if (plateInOrbit.gameObject.layer == 10)
        {
            PickableConfinedToPlane plate = plateInOrbit.gameObject.GetComponentInParent<PickableConfinedToPlane>();
            if (!plate.OrbitLocked)
            {
                plate.ConfinedPlane = gameObject.GetComponent<BoxCollider>();
            }
        }

    }
}
