using UnityEngine;

public class RotateWithTime : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private Vector3 rotationOrigin;
    [SerializeField]
    private Vector3 rotationAxis;


    private void Update()
    {
        transform.RotateAround(rotationOrigin, rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
