using UnityEngine;

public class PrintOrientation : MonoBehaviour
{
    public float Zenith = 0f;
    public float Azimuth = 0f;


    private static Vector3 StaticUp = Vector3.up;
    private static Vector3 StaticForward = Vector3.forward;
    private static Vector3 StaticRight = Vector3.right;


    private void Start()
    {
		
	}
		
	private void Update()
    {        
        Vector3 forwardVec = transform.forward;
        Zenith = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(forwardVec, StaticUp));

        Vector3 normedProjectionOnFloor = Vector3.Normalize(transform.forward - new Vector3(0, transform.forward.y, 0));
        float signedMagnitudeOfSineAzimuth = Vector3.Dot(Vector3.Cross(StaticForward, normedProjectionOnFloor), StaticUp);
        Azimuth = Mathf.Rad2Deg * Mathf.Asin(signedMagnitudeOfSineAzimuth);

        Debug.DrawRay(transform.position, StaticUp, Color.green);
        Debug.DrawRay(transform.position, StaticForward, Color.yellow);
        Debug.DrawRay(transform.position, StaticRight, Color.red);

        Debug.DrawRay(transform.position, forwardVec, Color.blue);
        Debug.DrawRay(transform.position, normedProjectionOnFloor, Color.black);
	}
}
