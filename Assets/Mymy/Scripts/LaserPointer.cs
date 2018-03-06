/*
 * Copyright (c) 2016 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using wvr;

// https://www.raywenderlich.com/149239/htc-vive-tutorial-unity
// https://unity3d.com/learn/tutorials/topics/virtual-reality/interaction-vr
public class LaserPointer : MonoBehaviour
{
    [SerializeField]
    private WVR_DeviceType device = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    [SerializeField]
    private WVR_InputId inputToListen = WVR_InputId.WVR_InputId_16;
    [SerializeField]
    private Transform trackedObjTransform;
    [SerializeField]
    private GameObject laserPrefab;    
    [SerializeField]
    private GameObject reticlePrefab;
    [SerializeField]
    private Vector3 reticleOffset;
    [SerializeField]
    private float maxPointerDist = 100f;
    [SerializeField]
    private bool isUseRaycastNormalForReticleOrientation;
    [SerializeField]
    private LayerMask layersForRaycast;
    private int pickableObjLayer = 10;

    private Transform laserTransform;
    private Transform reticleTransform;
    // used when ray cast doesn't hit any object
    private RaycastHit defaultHit;
    private Vector3 originalReticleScale;
    private Quaternion originalReticleRotation;

    // Serves as a reference to the GameObject that the player is currently grabbing
    private GameObject objectInHand;
    private float objectInHandOriginalDistance;
    

    /* MonoBehaviour */

    private void Start()
    {
        GameObject laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

        GameObject reticle = Instantiate(reticlePrefab);
        reticleTransform = reticle.transform;

        originalReticleScale = reticleTransform.localScale;
        originalReticleRotation = reticleTransform.localRotation;

        defaultHit = new RaycastHit()
        {
            point = trackedObjTransform.position + trackedObjTransform.forward * maxPointerDist,
            normal = trackedObjTransform.forward,
            distance = maxPointerDist
        };
    }

    private void Update()
    {
        defaultHit.point = trackedObjTransform.position + trackedObjTransform.forward * maxPointerDist;
        defaultHit.normal = trackedObjTransform.forward;

        // set default values, used when ray cast doesn't hit any object
        RaycastHit hit = defaultHit;

        // Send out a raycast from the controller
        bool isHit = Physics.Raycast(trackedObjTransform.position, trackedObjTransform.forward, out hit, maxPointerDist, layersForRaycast);

        bool isBtnPressed = WaveVR_Controller.Input(device).GetPress(inputToListen);

        ShowReticle(hit);

        if (isBtnPressed)
        {
            ShowLaser(hit);
        }
        else
        {
            HideLaser();
        }

        if (isHit && isBtnPressed)
        {
            GrabObject(hit.collider);            
        }
        else
        {            
            ReleaseObject();
        }

        // set objectInHand position
        if (objectInHand)
        {
            objectInHand.transform.position = trackedObjTransform.position + 
                trackedObjTransform.forward * objectInHandOriginalDistance;
        }
    }

    /* end of MonoBehaviour */


    /* show laser & pointer */

    private void HideLaser()
    {
        laserTransform.gameObject.SetActive(false);
    }

    // https://www.raywenderlich.com/149239/htc-vive-tutorial-unity
    private void ShowLaser(RaycastHit hitTarget)
    {
        // Show the laser
        laserTransform.gameObject.SetActive(true);

        // Move laser to the middle between the controller and the position the raycast hit
        laserTransform.position = Vector3.Lerp(trackedObjTransform.position, hitTarget.point, .5f);

        // Rotate laser facing the hit point
        laserTransform.LookAt(hitTarget.point);

        // Scale laser so it fits exactly between the controller & the hit point
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hitTarget.distance);
    }

    private void HideReticle()
    {
        reticleTransform.gameObject.SetActive(false);
    }

    // // https://unity3d.com/learn/tutorials/topics/virtual-reality/interaction-vr
    private void ShowReticle(RaycastHit hitTarget)
    {
        // set visible
        reticleTransform.gameObject.SetActive(true);

        // set position
        reticleTransform.position = hitTarget.point;

        // set scale
        reticleTransform.localScale = originalReticleScale * hitTarget.distance;

        // set rotation
        if (isUseRaycastNormalForReticleOrientation)
        {
            reticleTransform.forward = hitTarget.normal;
        }
        else
        {
            reticleTransform.localRotation = originalReticleRotation;
        }
    }

    /* end of show laser & pointer */


    /* grab object */
    /* https://www.raywenderlich.com/149239/htc-vive-tutorial-unity */

    private void GrabObject(Collider col)
    {
        if (objectInHand == null && col.gameObject.layer == pickableObjLayer)
        {
            objectInHand = col.gameObject;
            objectInHandOriginalDistance = 
                (objectInHand.transform.position - trackedObjTransform.position).magnitude;
        }
    }

    private void ReleaseObject()
    {
        // Remove the reference to the formerly attached object
        objectInHand = null;
    }
  
    /* end of grab object */
}
