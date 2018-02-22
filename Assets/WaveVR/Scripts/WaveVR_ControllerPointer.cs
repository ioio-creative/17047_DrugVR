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
using wvr;
using System;
using WaveVR_Log;

/// <summary>
/// Draws a pointer in front of any object that the controller point at.
/// The circle dilates if the object is clickable.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class WaveVR_ControllerPointer : MonoBehaviour {
    private string LOG_TAG = "WaveVR_ControllerPointer";

    public bool ListenToDevice = false;
    public WVR_DeviceType device;
    /// <summary>
    /// Number of segments making the pointer circle.
    /// </summary>
    private int pointerSegments = 20;
    /// <summary>
    /// Growth speed multiplier for the pointer.
    /// </summary>
    private float pointerGrowthSpeed = 8.0f;
    /// <summary>
    /// Color of the pointer.
    /// </summary>
    private Color pointerColor = Color.white;
    /// <summary>
    /// The color flicker flag of the pointer
    /// </summary>
    private bool colorFlickerPerSecond = false;
    /// <summary>
    /// The color deepening flag of the pointer during rotation status
    /// </summary>
    // public bool deepeningColorRotation = false;
    /// <summary>
    /// The rotation of the pointer
    /// </summary>
    // public int rotationSpeed = 6;  // 1 the highest speed

    private Material materialComp;
    private Renderer rend;
    private float pointerDistanceInMeters = 10.0f;         // Current distance of the pointer (in meters).
    private const float kpointerDistanceMin = 1.0f;        // Minimum distance of the pointer (in meters).
    private const float kpointerDistanceMax = 10.0f;       // Maximum distance of the pointer (in meters).
    private float pointerOuterAngle = 1.2f;                // Current outer angle of the pointer (in degrees).
    private const float kpointerMinOuterAngle = 1.2f;      // Minimum outer angle of the pointer (in degrees).
    private const float kpointerGrowthAngle = 0.15f;        // Angle at which to expand the pointer when intersecting with an object (in degrees).
    private float pointerOuterDiameter = 0.0f;             // Current outer diameters of the pointer, before distance multiplication.
    private Color colorFactor = Color.black;               // The color variable of the pointer
    private float colorFlickerTime = 0.0f;                 // The color flicker time
    private bool enabledpointer = true;                    // true: show pointer, false: remove pointer
    private bool meshIsCreated = false;                    // true: the mesh of reticle is created, false: the mesh of reticle is not ready
    private bool stay = false;

    void Start () {
         if (enabledpointer) {
              if (!meshIsCreated) {
                   initialPointer();
              }
         } else {
              if (meshIsCreated) {
                   removePointer();
              }
         }
    }

    void Update() {
       if (ListenToDevice)
            enabledpointer = WaveVR_Controller.Input (device).connected ? true : false;

        if (enabledpointer) {
            if (!meshIsCreated) {
                initialPointer();
            }
        } else {
            if (meshIsCreated) {
                removePointer();
            }
            return;
        }

        pointerDistanceInMeters = Mathf.Clamp(pointerDistanceInMeters, kpointerDistanceMin, kpointerDistanceMax);

        //if (pointerInnerAngle < kpointerMinInnerAngle)
        //     pointerInnerAngle = kpointerMinInnerAngle;
        if (pointerOuterAngle < kpointerMinOuterAngle)
            pointerOuterAngle = kpointerMinOuterAngle;
        // float innerHalfAngelRadians = Mathf.Deg2Rad * pointerInnerAngle * 0.5f;
        float outerHalfAngelRadians = Mathf.Deg2Rad * pointerOuterAngle * 0.5f;
        // float innerDiameter = 2.0f * Mathf.Tan(innerHalfAngelRadians);
        float outerDiameter = 2.0f * Mathf.Tan(outerHalfAngelRadians);

        //   pointerInnerDiameter = Mathf.Lerp(pointerInnerDiameter, innerDiameter, Time.deltaTime * pointerGrowthSpeed);
        pointerOuterDiameter = Mathf.Lerp(pointerOuterDiameter, outerDiameter, Time.deltaTime * pointerGrowthSpeed);

        //   materialComp.SetFloat("_InnerDiameter", pointerInnerDiameter * pointerDistanceInMeters);
        materialComp.SetFloat("_OuterDiameter", pointerOuterDiameter * pointerDistanceInMeters);
        materialComp.SetFloat("_DistanceInMeters", pointerDistanceInMeters);
    }

    private void initialPointer() {
        colorFlickerTime = Time.unscaledTime;
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        materialComp = gameObject.GetComponent<Renderer>().material;
        meshIsCreated = true;
    }

    public void removePointer() {
        rend = GetComponent<Renderer>();
        rend.enabled = false;
        meshIsCreated = false;
    }

    public void OnPointerEnter (Camera camera, GameObject target, Vector3 intersectionPosition, bool isInteractive) {
        stay = true;
        SetPointerTarget(intersectionPosition, isInteractive);
    }
    
    public void OnPointerStay (Camera camera, GameObject target, Vector3 intersectionPosition, bool isInteractive) {
        stay = true;
        SetPointerTarget(intersectionPosition, isInteractive);
    }

    public void OnPointerExit (Camera camera, GameObject target) {
        stay = false;
        pointerDistanceInMeters = kpointerDistanceMax;
        pointerOuterAngle = kpointerMinOuterAngle;
    }

    public float getPointerCurrentDistance() {
         return pointerDistanceInMeters;
    }

    private void SetPointerTarget (Vector3 target, bool interactive) {
        Vector3 targetLocalPosition = transform.InverseTransformPoint(target);
        pointerDistanceInMeters = Mathf.Clamp(targetLocalPosition.z, kpointerDistanceMin, kpointerDistanceMax);
        if (stay) {
            pointerOuterAngle = kpointerMinOuterAngle + Mathf.Clamp(kpointerDistanceMax / (Mathf.Abs(targetLocalPosition.z)+0.001f), 0.0f, 30.0f) * kpointerGrowthAngle;
        } else {

            pointerOuterAngle = kpointerMinOuterAngle;
        }
    }

    public void SetPointerColor(Color pointer_color)
    {
        pointerColor = pointer_color;
    }
}
