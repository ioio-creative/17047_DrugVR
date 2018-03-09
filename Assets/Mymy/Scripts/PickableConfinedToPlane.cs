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

    
    private Transform m_DebugLaserTransform;    
    private GameObject m_RaycastOriginObject;
    private Rigidbody m_ThisRigidBody;
    private LayerMask m_ConfinedPlaneLayer;
    private GameObject m_PickUpContainer;


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
            bool isHitConfinedPlane = 
                UpdatePickUpContainerOrientationByRaycastToConfinedPlane();

            if (isHitConfinedPlane)
            {
                isShowLaser = true;
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

        // instantiate empty m_PickUpContainer
        m_PickUpContainer = new GameObject("PickUpContainer");

        // set parent of m_PickUpContainer to that of this.gameObject
        m_PickUpContainer.transform.parent = gameObject.transform.parent;
               
        bool isHitConfinedPlane = 
            UpdatePickUpContainerOrientationByRaycastToConfinedPlane();
                
        // change parent of this.gameObject to be m_PickUpContainer
        // so that position & orientation offset between hit.point / raycast and this.gameObject can be preserved
        gameObject.transform.parent = m_PickUpContainer.transform;        

        // make the object not respond to physics
        m_ThisRigidBody.isKinematic = true;
    }

    public void OnObjectReleased()
    {
        // set parent back to the original one
        gameObject.transform.parent = m_PickUpContainer.transform.parent;
        
        // make the object respond to physics
        m_ThisRigidBody.isKinematic = false;

        Destroy(m_PickUpContainer);
        m_PickUpContainer = null;
        
        m_RaycastOriginObject = null;
    }


    private bool UpdatePickUpContainerOrientationByRaycastToConfinedPlane()
    {
        bool isHitConfinedPlane = false;

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
                // change m_PickUpContainer.transform
                // so that its position is same as hit.point
                // and its orientation is same as m_RaycastOriginObject.
                // since m_PickUpContainer is parent of this.gameObject,
                // this.gameObject's orientation will be affected
                m_PickUpContainer.transform.position = hit.point;
                m_PickUpContainer.transform.rotation = m_RaycastOriginObject.transform.rotation;

                if (m_IsShowDebugRay)
                {
                    ShowLaser(hit, m_RaycastOriginObject.transform);
                }

                isHitConfinedPlane = true;
                break;
            }
        }

        return isHitConfinedPlane;
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
