using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrigger : MonoBehaviour
{
    [SerializeField]
    private float m_upwardMag = 85;

    Vector3 translateVector;

    private void Awake()
    {
        translateVector = new Vector3(0, m_upwardMag);
    }

    private void OnTriggerEnter(Collider plateCollider)
    {
        if (plateCollider.gameObject.layer == 10)
        {
            GameObject plateRigidBodyGO = plateCollider.GetComponentInParent<Rigidbody>().gameObject;
            plateRigidBodyGO.transform.rotation = new Quaternion(0, 0, 0, 0);
            plateRigidBodyGO.transform.Translate(translateVector);
            Rigidbody plateRB = plateRigidBodyGO.GetComponent<Rigidbody>();
            plateRB.velocity = Vector3.zero;
            plateRB.angularVelocity = Vector3.zero;
            plateRB.Sleep(); 
        }
    }



}
