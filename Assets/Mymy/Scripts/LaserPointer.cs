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

public class LaserPointer : MonoBehaviour
{
    public GameObject laserPrefab;
    public GameObject trackedObj;


    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
        
    
    private void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;        
    }

    private void Update()
    {        
        RaycastHit hit;

        // Send out a raycast from the controller
        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
        {
            hitPoint = hit.point;
            ShowLaser(hit.point, hit.distance);
        }
        else
        {
            //RaycastHit2D btnHit = Physics2D.Raycast(trackedObj.transform.position, transform.forward, 100);
            //if (btnHit)
            //{
            //    ShowLaser(btnHit.point, btnHit.distance);
            //}
            //else
            //{
                ShowLaser(trackedObj.transform.position + trackedObj.transform.forward * 100, 100);
            //}
        }
    }

    private void ShowLaser(Vector3 hitTarget, float hitDistance)
    {
        // Show the laser
        laser.SetActive(true);

        // Move laser to the middle between the controller and the position the raycast hit
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitTarget, .5f);

        // Rotate laser facing the hit point
        laserTransform.LookAt(hitTarget);

        // Scale laser so it fits exactly between the controller & the hit point
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hitDistance);
    }

    //private void ShowLaser(RaycastHit hit)
    //{
    //    // Show the laser
    //    laser.SetActive(true);

    //    // Move laser to the middle between the controller and the position the raycast hit
    //    laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);

    //    // Rotate laser facing the hit point
    //    laserTransform.LookAt(hitPoint);

    //    // Scale laser so it fits exactly between the controller & the hit point
    //    laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
    //        hit.distance);
    //}    
}
