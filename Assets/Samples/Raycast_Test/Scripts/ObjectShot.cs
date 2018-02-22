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

public class ObjectShot : MonoBehaviour
{

	private void TeleportRandomly ()
	{
		Vector3 direction = UnityEngine.Random.onUnitSphere;
		direction.y = Mathf.Clamp (direction.y, 0.5f, 1f);
		direction.z = Mathf.Clamp (direction.z, 3f, 7f);
		float distance = 2 * UnityEngine.Random.value + 1.5f;
		transform.localPosition = direction * distance;
	}

	public void HitBy()
	{
		TeleportRandomly ();
	}
}
