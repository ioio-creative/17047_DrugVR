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

public class ShotRaycast : MonoBehaviour
{
    private static string LOG_TAG = "ShotRaycast";
    //public GameObject raycastObject;

    private bool ListenToDevice = false;

    private float distance;
    private LineRenderer lr;
    private float rayLength = 10.0f;

    // Use this for initialization
    void Start ()
    {
        initLineRenderer ();
    }

    private void initLineRenderer()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        #if UNITY_5_5_OR_NEWER
        lr.startWidth = 0.02f;
        lr.endWidth = 0.01f;
        #else
        lr.SetWidth (0.02f, 0.01f);
        #endif

        lr.material = new Material (Shader.Find("Particles/Additive"));    // in "Always Included Shaders"

        lr.useWorldSpace = true;
        lr.enabled = !ListenToDevice;    // if not listen to device, default enable ray.
    }

    // Update is called once per frame
    void Update ()
    {
        shotRaycast ();
    }

    private void shotRaycast()
    {
        if (lr.enabled == true)
        {
            Vector3 pos = transform.position;
            Vector3 forward = transform.TransformDirection(new Vector3(0, 0, 1));

            Vector3 vertex0 = pos;
            Vector3 vertex1 = forward * rayLength;

            lr.SetPosition(0, vertex0);
            lr.SetPosition(1, vertex1);

            #if UNITY_5_5_OR_NEWER
            lr.startColor = Color.yellow;
            lr.endColor = Color.yellow;
            #else
            lr.SetColors(Color.yellow, Color.yellow);
            #endif

            RaycastHit hit;
            if (Physics.Raycast(pos, forward, out hit))
            {
                distance = hit.distance;
                #if UNITY_EDITOR
                Debug.Log("ShotRaycast, raycast hits: " + distance + " " + hit.collider.gameObject.name);
                #endif
                Log.d (LOG_TAG, "raycast hits: " + distance + " " + hit.collider.gameObject.name);
                hit.transform.SendMessage ("HitBy");
            }
        }
    }

    void onDestroy() {
        Destroy(this.GetComponent<Renderer>().material);
    }
}
