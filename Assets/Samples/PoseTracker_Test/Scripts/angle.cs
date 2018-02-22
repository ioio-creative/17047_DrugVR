using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class angle : MonoBehaviour {
    public GameObject target;
    // private float fAngle = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (target != null)
        {
            /*
            Vector3 _targetForward = target.transform.rotation * Vector3.forward;
            fAngle = Vector3.Angle (_targetForward, Vector3.right);
            Debug.Log (fAngle);
            */
            Debug.Log ("rotation " + target.transform.rotation.eulerAngles.x + ", " + target.transform.rotation.eulerAngles.y + ", " + target.transform.rotation.eulerAngles.z);
        }
	}
}
