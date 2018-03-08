using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PickableConfinedToPlane : MonoBehaviour
{
    [SerializeField]
    private bool m_IsShowDebugRay;
    [SerializeField]
    private GameObject m_DebugLaserPrefab;    
    [SerializeField]
    private BoxCollider m_ConfinedPlane;
    [SerializeField]
    private float m_MaxPointerDist = 100f;

    public GameObject m_PickableContainer;
    public GameObject m_PickupContainer;
    
    private Transform m_DebugLaserTransform;    
    private GameObject m_RaycastOriginObject;
    private Rigidbody m_ThisRigidBody;
    private LayerMask m_ConfinedPlaneLayer;

    private Vector3 m_GrabPosOffset;
    private Vector3 m_GrabRotOffset;
    /* MonoBehaviour */

    private void Awake()
    {
        m_ThisRigidBody = GetComponent<Rigidbody>();
        m_ConfinedPlaneLayer = 1 << m_ConfinedPlane.gameObject.layer;
    }

    private void Start()
    { 
        if (m_DebugLaserPrefab)
        {
            GameObject laser = Instantiate(m_DebugLaserPrefab);
            m_DebugLaserTransform = laser.transform;
        }
    }

    private void FixedUpdate()
    {
        bool isShowLaser = false;
       
        // m_RaycastOriginObject is assigned when OnObjectPicked()
        // i.e. if this gameObject is picked by LaserPointer
        if (m_RaycastOriginObject)
        {
            // !!! Important !!!
            // use RaycastAll
            RaycastHit[] hits = Physics.RaycastAll(m_RaycastOriginObject.transform.position,
                m_RaycastOriginObject.transform.forward,
                m_MaxPointerDist, m_ConfinedPlaneLayer);

            foreach (RaycastHit hit in hits)
            {
                // hit corresponding to confined plane
                if (hit.collider == m_ConfinedPlane)
                {
                    m_PickupContainer.transform.position = hit.point;
                    m_PickupContainer.transform.rotation = m_RaycastOriginObject.transform.rotation;


                    //gameObject.transform.position = hit.point + m_GrabPosOffset;                  

                    if (m_IsShowDebugRay)
                    {
                        ShowLaser(hit, m_RaycastOriginObject.transform);
                        isShowLaser = true;
                    }

                    break;
                }
            }
        }

        if (!isShowLaser)
        {
            HideLaser();
        }
    }

    /* end of MonoBehaviour */


    public void OnObjectPicked(GameObject raycastOriginObject)
    {        
        m_RaycastOriginObject = raycastOriginObject;

        // !!! Important !!!
        // use RaycastAll
        RaycastHit[] hits = Physics.RaycastAll(m_RaycastOriginObject.transform.position,
            m_RaycastOriginObject.transform.forward,
            m_MaxPointerDist, m_ConfinedPlaneLayer);

        foreach (RaycastHit hit in hits)
        {
            // hit corresponding to confined plane
            if (hit.collider == m_ConfinedPlane)
            {
                m_PickupContainer.transform.position = hit.point;
                m_PickupContainer.transform.rotation = m_RaycastOriginObject.transform.rotation;
                gameObject.transform.parent = m_PickupContainer.transform;

                //m_GrabPosOffset = gameObject.transform.position - hit.point;
                //gameObject.transform.position = hit.point + m_GrabPosOffset;

                //m_GrabRotOffset = gameObject.transform.rotation.eulerAngles - m_RaycastOriginObject.transform.rotation.eulerAngles;
                //gameObject.transform.rotation.eulerAngles = m_RaycastOriginObject.transform.rotation.eulerAngles + m_GrabRotOffset;

                if (m_IsShowDebugRay)
                {
                    ShowLaser(hit, m_RaycastOriginObject.transform);
                }

                break;
            }
        }

        // make the object not respond to physics
        m_ThisRigidBody.isKinematic = true;        
    }

    public void OnObjectReleased()
    {
        gameObject.transform.parent = m_PickableContainer.transform;
        
        // make the object respond to physics
        m_ThisRigidBody.isKinematic = false;        
        
        m_RaycastOriginObject = null;        
    }


    private void ShowLaser(RaycastHit hitTarget, Transform raycastOriginObjTransform)
    {
        // Show the laser
        m_DebugLaserTransform.gameObject.SetActive(true);

        // Move laser to the middle between the controller and the position the raycast hit
        m_DebugLaserTransform.position = Vector3.Lerp(raycastOriginObjTransform.position, hitTarget.point, .5f);

        // Rotate laser facing the hit point
        m_DebugLaserTransform.LookAt(hitTarget.point);

        // Scale laser so it fits exactly between the controller & the hit point
        m_DebugLaserTransform.localScale = new Vector3(
            m_DebugLaserTransform.localScale.x,
            m_DebugLaserTransform.localScale.y,
            hitTarget.distance);
    }

    private void HideLaser()
    {
        m_DebugLaserTransform.gameObject.SetActive(false);
    }
}
