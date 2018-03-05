﻿/*
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

// https://www.raywenderlich.com/149239/htc-vive-tutorial-unity
// https://unity3d.com/learn/tutorials/topics/virtual-reality/interaction-vr
public class LaserPointer : MonoBehaviour
{
    [SerializeField]
    private GameObject trackedObj;
    [SerializeField]
    private GameObject laserPrefab;    
    [SerializeField]
    private GameObject reticlePrefab;
    [SerializeField]
    private Vector3 reticleOffset;
    [SerializeField]
    private float maxPointerDist = 100f;


    private GameObject laser;
    private Transform laserTransform;
    private Transform trackedObjTransform;
    private GameObject reticle;
    private Transform reticleTransform;
    // used when ray cast doesn't hit any object
    private RaycastHit defaultHit;
    private Vector3 originalReticleScale;
    private Quaternion originalReticleRotation;


    private void Awake()
    {
        originalReticleScale = reticleTransform.localScale;        
    }

    private void Start()
    {
        trackedObjTransform = trackedObj.transform;

        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

        reticle = Instantiate(reticlePrefab);
        reticleTransform = reticle.transform;

        defaultHit = new RaycastHit()
        {
            point = trackedObjTransform.position + trackedObjTransform.forward * maxPointerDist,
            normal = trackedObjTransform.forward,
            distance = maxPointerDist
        };
    }

    private void Update()
    {
        // set default values, used when ray cast doesn't hit any object
        RaycastHit hit = defaultHit;

        // Send out a raycast from the controller
        bool isHit = Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, maxPointerDist);

        ShowReticle(hit);      
    }

    // // https://www.raywenderlich.com/149239/htc-vive-tutorial-unity
    private void ShowLaser(RaycastHit hitTarget)
    {
        // Show the laser
        laser.SetActive(true);

        // Move laser to the middle between the controller and the position the raycast hit
        laserTransform.position = Vector3.Lerp(trackedObjTransform.position, hitTarget.point, .5f);

        // Rotate laser facing the hit point
        laserTransform.LookAt(hitTarget.point);

        // Scale laser so it fits exactly between the controller & the hit point
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hitTarget.distance);
    }

    // // https://unity3d.com/learn/tutorials/topics/virtual-reality/interaction-vr
    private void ShowReticle(RaycastHit hitTarget)
    {
        // set visible
        reticle.SetActive(true);

        // set position
        reticleTransform.position = hitTarget.point;

        // set scale
        reticleTransform.localScale = 

        // set rotation
        reticleTransform.forward = hitTarget.normal;
    }   
}
