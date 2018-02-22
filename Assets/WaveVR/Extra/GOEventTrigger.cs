// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using System.Collections;
using WaveVR_Log;

public class GOEventTrigger : MonoBehaviour
{
    private static string LOG_TAG = "GOEventTrigger";
	private Vector3 startPosition;
	private Color defaultColor = Color.gray;
	private Color changedColor = Color.red;
	
    // --------------- Event Handling begins --------------
	public void OnEnter()
	{
        Log.d (LOG_TAG, "OnEnter");
		ChangeColor (true);
	}

	public void OnTrigger()
	{
        Log.d (LOG_TAG, "OnTrigger");
		TeleportRandomly ();
	}

	public void OnExit()
	{
        Log.d (LOG_TAG, "OnExit");
		ChangeColor (false);
	}

	public void OnGazeReset ()
	{
		transform.localPosition = startPosition;
		ChangeColor (false);
	}

    public void OnShowButton()
    {
        #if UNITY_EDITOR
        Debug.Log ("OnShowButton");
        #endif
        transform.gameObject.SetActive (true);
    }

    public void OnHideButton()
    {
        #if UNITY_EDITOR
        Debug.Log ("OnHideButton");
        #endif
        transform.gameObject.SetActive (false);
    }
    // --------------- Event Handling ends --------------

	public void ChangeColor(string color)
	{
		if (color.Equals("blue"))
			GetComponent<Renderer>().material.color = Color.blue;
		else if (color.Equals("cyan"))
			GetComponent<Renderer>().material.color = Color.cyan;
	}

	private void ChangeColor(bool change)
	{
		GetComponent<Renderer>().material.color = change ? changedColor : defaultColor;
	}

	private void TeleportRandomly () {
		Vector3 direction = UnityEngine.Random.onUnitSphere;
		direction.y = Mathf.Clamp (direction.y, 0.5f, 1f);
		direction.z = Mathf.Clamp (direction.z, 3f, 10f);
		float distance = 2 * UnityEngine.Random.value + 1.5f;
		transform.localPosition = direction * distance;
	}

	void Start ()
	{
		startPosition = transform.localPosition;
	}
}
