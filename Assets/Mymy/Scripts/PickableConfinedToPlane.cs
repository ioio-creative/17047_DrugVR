using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PickableConfinedToPlane : MonoBehaviour
{   
    [SerializeField]
    private BoxCollider m_ConfinedPlane;
    [SerializeField]
    private float m_MaxPointerDist = 100f;
    [SerializeField]
    private bool m_UseGravityWhenPicked = true;
    [SerializeField]
    private bool m_IsKinematicWhenPicked = false;

    
    private Transform m_DebugLaserTransform;    
    private GameObject m_RaycastOriginObject;
    private Rigidbody m_ThisRigidBody;
    private LayerMask m_ConfinedPlaneLayer;
    private GameObject m_PickUpContainer;
    private bool m_OriginalUseGravity;
    private bool m_OriginalIsKinematic;


    /* MonoBehaviour */

    private void Awake()
    {
        m_ThisRigidBody = GetComponent<Rigidbody>();
        m_ConfinedPlaneLayer = 1 << m_ConfinedPlane.gameObject.layer;
    }    

    private void FixedUpdate()
    {
        // m_RaycastOriginObject is assigned when OnObjectPicked()
        // i.e. if this gameObject is picked by LaserPointer
        if (m_RaycastOriginObject)
        {
            bool isHitConfinedPlane = 
                UpdatePickUpContainerOrientationByRaycastToConfinedPlane();            
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

        m_OriginalIsKinematic = m_ThisRigidBody.isKinematic;
        m_ThisRigidBody.isKinematic = m_IsKinematicWhenPicked;

        m_OriginalUseGravity = m_ThisRigidBody.useGravity;
        m_ThisRigidBody.useGravity = m_UseGravityWhenPicked;
    }

    public void OnObjectReleased()
    {
        m_ThisRigidBody.useGravity = m_OriginalUseGravity;

        m_ThisRigidBody.isKinematic = m_OriginalIsKinematic;

        // set parent back to the original one
        gameObject.transform.parent = m_PickUpContainer.transform.parent;

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

                Debug.DrawRay(m_RaycastOriginObject.transform.position, hit.point, Color.green);

                isHitConfinedPlane = true;
                break;
            }
        }

        return isHitConfinedPlane;
    }
}
