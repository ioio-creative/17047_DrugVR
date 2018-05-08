using UnityEngine;

public class RotateGameObject : MonoBehaviour
{
    [SerializeField]
    private float m_EulerYOffset = -52.5f;

    private void Start()
    {
        Quaternion rotationAlongY = Quaternion.Euler(0, m_EulerYOffset, 0);
        transform.rotation = rotationAlongY * transform.rotation;        
	}	
}
