using UnityEngine;

public class PickableConfinedToPlane : MonoBehaviour
{
    [SerializeField]
    private bool m_IsShowDebugRay;
    [SerializeField]
    private GameObject m_DebugLaserPrefab;    
    [SerializeField]
    private LayerMask m_PlaneLayer;
    [SerializeField]
    private float m_MaxPointerDist = 100f;


    private Transform m_DebugLaserTransform;
    private GameObject m_ObjectPicked;
    private GameObject m_RaycastOriginObject;


    /* MonoBehaviour */

    private void Start()
    {
        if (m_DebugLaserPrefab)
        {
            GameObject laser = Instantiate(m_DebugLaserPrefab);
            m_DebugLaserTransform = laser.transform;
        }
    }

    private void Update()
    {
        bool isShowLaser = false;

        if (m_ObjectPicked && m_RaycastOriginObject)
        {
            RaycastHit hit;

            if (Physics.Raycast(m_RaycastOriginObject.transform.position,
                    m_RaycastOriginObject.transform.forward, 
                    out hit, m_MaxPointerDist, m_PlaneLayer))
            {
                m_ObjectPicked.transform.position = hit.point;

                if (m_IsShowDebugRay)
                {
                    ShowLaser(hit, m_RaycastOriginObject.transform);
                    isShowLaser = true;
                }
            }
            else
            {
                //float distance = (m_ObjectPicked.transform.position - m_RaycastOriginObject.transform.position).magnitude;
                //m_ObjectPicked.transform.position = m_RaycastOriginObject.transform.position +
                //    m_RaycastOriginObject.transform.forward * distance;                
            }
        }

        if (!isShowLaser)
        {
            HideLaser();
        }
    }

    /* end of MonoBehaviour */


    public void OnObjectPicked(GameObject objectPicked,
        GameObject raycastOriginObject)
    {
        m_ObjectPicked = objectPicked;
        m_RaycastOriginObject = raycastOriginObject;
    }

    public void OnObjectReleased()
    {
        m_ObjectPicked = null;
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
