using UnityEngine;

public class PickableConfinedToPlane : MonoBehaviour
{
    [SerializeField]
    private BoxCollider m_ConfinedPlane;
    public BoxCollider ConfinedPlane
    {
        get { return m_ConfinedPlane; }
        set
        {
            m_ConfinedPlane = value;
            m_ConfinedPlaneLayer = 1 << m_ConfinedPlane.gameObject.layer;
        }
    }
    public bool OrbitLocked { get; private set; }
    [SerializeField]
    private Rigidbody m_ControlledRigidBody;
    [SerializeField]
    private bool m_UseGravityWhenPicked = true;
    [SerializeField]
    private bool m_IsKinematicWhenPicked = false;

      
    private GameObject m_RaycastOriginObject;    
    private LayerMask m_ConfinedPlaneLayer;
    private GameObject m_PickUpContainer;
    private bool m_OriginalUseGravity;
    private bool m_OriginalIsKinematic;
    private float m_MaxPointerDist;
    private float m_OriginalRigidbodyMass;


    /* MonoBehaviour */

    private void Awake()
    {        
        if (m_ConfinedPlane != null) m_ConfinedPlaneLayer = 1 << m_ConfinedPlane.gameObject.layer;
        m_OriginalRigidbodyMass = m_ControlledRigidBody.mass;
        OrbitLocked = false;
    }    

    private void Update()
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


    public void OnObjectPicked(GameObject raycastOriginObject, float raycastMaxDist)
    {
        OrbitLocked = true;

        m_RaycastOriginObject = raycastOriginObject;
        m_MaxPointerDist = raycastMaxDist;

        // instantiate empty m_PickUpContainer
        m_PickUpContainer = new GameObject("PickUpContainer");
        // set parent of m_PickUpContainer to that of this.gameObject
        m_PickUpContainer.transform.SetParent(gameObject.transform.parent);
        // set position of m_PickUpContainer to that of this.gameObject
        m_PickUpContainer.transform.position = gameObject.transform.position;

        bool isHitConfinedPlane =
            UpdatePickUpContainerOrientationByRaycastToConfinedPlane();

        // change parent of this.gameObject to be m_PickUpContainer
        // so that position & orientation offset between hit.point / raycast and this.gameObject can be preserved
        gameObject.transform.SetParent(m_PickUpContainer.transform);


        m_OriginalIsKinematic = m_ControlledRigidBody.isKinematic;
        m_ControlledRigidBody.isKinematic = m_IsKinematicWhenPicked;

        m_OriginalUseGravity = m_ControlledRigidBody.useGravity;
        m_ControlledRigidBody.useGravity = m_UseGravityWhenPicked;

        m_ControlledRigidBody.mass = 0f;
    }

    public void OnObjectReleased()
    {                
        m_ControlledRigidBody.useGravity = m_OriginalUseGravity;

        m_ControlledRigidBody.isKinematic = m_OriginalIsKinematic;

        m_ControlledRigidBody.mass = m_OriginalRigidbodyMass;

        // set parent back to the original one
        gameObject.transform.parent = m_PickUpContainer.transform.parent;

        Destroy(m_PickUpContainer);
        m_PickUpContainer = null;
        
        m_RaycastOriginObject = null;

        OrbitLocked = false;
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

                Debug.DrawLine(m_RaycastOriginObject.transform.position, hit.point, Color.green);

                isHitConfinedPlane = true;
                break;
            }
        }

        return isHitConfinedPlane;
    }
}
