using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrigger : MonoBehaviour
{
    [SerializeField]
    private float m_RespawnYOffset = 5;


    private void OnTriggerEnter(Collider plateCollider)
    {
        if (plateCollider.gameObject.layer == 10)
        {
            //RigidBody is in the same level as collider while Pickable class is 1 level higher
            PickableConfinedToPlane platePickable = plateCollider.GetComponentInParent<PickableConfinedToPlane>();
            Vector3 originalRBPosition = platePickable.OriginalRBPosition;
            Rigidbody plateRB = plateCollider.GetComponent<Rigidbody>();
            plateRB.gameObject.transform.position = new Vector3(originalRBPosition.x, originalRBPosition.y + m_RespawnYOffset, originalRBPosition.z);
            plateRB.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);        
            
            plateRB.velocity = Vector3.zero;
            plateRB.angularVelocity = Vector3.zero;
            plateRB.Sleep();
        }
    }



}
