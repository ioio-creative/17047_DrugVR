#pragma warning disable 0414 // private field assigned but not used.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WaveVR_Log;
using UnityEngine.UI;

public enum EGazeInputEvent
{
    PointerDown,
    PointerClick,
    PointerSubmit
}

public class GazeInputModule : PointerInputModule
{
    private static string LOG_TAG = "GazeInputModule";
    public bool progressRate = false;  // The switch to show how many percent to click by TimeToGaze
    public float RateTextZPosition = 0.5f;
    public bool progressCounter = false;  // The switch to show how long to click by TimeToGaze
    public float CounterTextZPosition = 0.5f;
    public float TimeToGaze = 3.0f;
    public EGazeInputEvent InputEvent = EGazeInputEvent.PointerDown;

    /**
    * @brief get intersection position in world space
    **/
    private Vector3 GetIntersectionPosition(Camera cam, RaycastResult raycastResult)
    {
        // Check for camera
        if (cam == null) {
            return Vector3.zero;
        }

        float intersectionDistance = raycastResult.distance + cam.nearClipPlane;
        Vector3 intersectionPosition = cam.transform.position + cam.transform.forward * intersectionDistance;
        return intersectionPosition;
    }

    private PointerEventData pointerData;
    private void CastToCenterOfScreen()
    {
        if (pointerData == null)
            pointerData = new PointerEventData (eventSystem);

        pointerData.Reset();

        // Cast ray
        pointerData.position = new Vector2 (0.5f * Screen.width, 0.5f * Screen.height);  // center of screen
        eventSystem.RaycastAll(pointerData, m_RaycastResultCache);

        // Get result of which ray casted
        RaycastResult raycastResult = FindFirstRaycast(m_RaycastResultCache);

        if (raycastResult.gameObject != null && raycastResult.worldPosition == Vector3.zero)
            raycastResult.worldPosition = GetIntersectionPosition(pointerData.enterEventCamera, raycastResult);

        // Update PointerEventData attributes
        pointerData.pointerCurrentRaycast = raycastResult;
        if (raycastResult.gameObject != null)
            pointerData.position = raycastResult.screenPosition;

        m_RaycastResultCache.Clear();
    }

    private float gazeTime = 0.0f;
    // { ------- Reticle --------
    private Text progressText = null;
    private Text counterText = null;
    private WaveVR_Reticle gazePointer = null;
    private bool progressflag = true;
    private float countingTime = 0f;

    private GameObject GetCurrentGameObject(PointerEventData pointerData) {
        if (pointerData != null && pointerData.enterEventCamera != null)
            return pointerData.pointerCurrentRaycast.gameObject;

        return null;
    }

    private Vector3 GetIntersectionPosition(PointerEventData pointerData) {
        if (null == pointerData.enterEventCamera)
            return Vector3.zero;

        float intersectionDistance = pointerData.pointerCurrentRaycast.distance + pointerData.enterEventCamera.nearClipPlane;
        Vector3 intersectionPosition = pointerData.enterEventCamera.transform.position + pointerData.enterEventCamera.transform.forward * intersectionDistance;
        return intersectionPosition;
    }

    private void UpdateProgressDistance(PointerEventData pointerEvent) {
        Vector3 intersectionPosition = GetIntersectionPosition(pointerEvent);
        if (gazePointer == null)
            return;

        GameObject pc = GameObject.Find ("PercentCanvas");
        if (pc != null) {
            GameObject percentCanvas = pc.gameObject;
            Vector3 tmpVec = new Vector3(percentCanvas.transform.localPosition.x, percentCanvas.transform.localPosition.y, intersectionPosition.z - (RateTextZPosition >= 0 ? RateTextZPosition : 0));
            percentCanvas.transform.localPosition = tmpVec;
        }
        GameObject cc = GameObject.Find ("CounterCanvas");
        if (cc != null) {
            GameObject counterCanvas = cc.gameObject;
            Vector3 tmpVec = new Vector3(counterCanvas.transform.localPosition.x, counterCanvas.transform.localPosition.y, intersectionPosition.z - (CounterTextZPosition >= 0 ? CounterTextZPosition : 0));
            counterCanvas.transform.localPosition = tmpVec;
        }
    }

    private void UpdateReticle (GameObject preGazedObject, PointerEventData pointerEvent) {
        if (gazePointer == null)
            return;

        GameObject curGazeObject = GetCurrentGameObject(pointerEvent);
        Vector3 intersectionPosition = GetIntersectionPosition(pointerEvent);
        bool isInteractive = pointerEvent.pointerPress != null || ExecuteEvents.GetEventHandler<IPointerClickHandler>(curGazeObject) != null;

        if (curGazeObject == preGazedObject) {
            if (curGazeObject != null) {
                gazePointer.OnGazeStay(pointerEvent.enterEventCamera, curGazeObject, intersectionPosition, isInteractive);
            } else {
                gazePointer.OnGazeExit(pointerEvent.enterEventCamera, preGazedObject);
                return;
            }
        } else {
            if (preGazedObject != null) {
                gazePointer.OnGazeExit(pointerEvent.enterEventCamera, preGazedObject);
            }
            if (curGazeObject != null) {
                gazePointer.OnGazeEnter(pointerEvent.enterEventCamera, curGazeObject, intersectionPosition, isInteractive);
            }
        }
        UpdateProgressDistance(pointerEvent);
    }
    // --------- Reticle -------- }

    private void OnTriggeGaze()
    {
        bool sendEvent = false;
        // The gameobject to which raycast positions
        var currentOverGO = pointerData.pointerCurrentRaycast.gameObject;

        // { ------- Reticle --------
        if (progressText == null) {
            GameObject pt = GameObject.Find("ProgressText");
            if (pt != null) {
                progressText = pt.GetComponent<Text>();
            }
        }
        if (counterText == null) {
            GameObject ct = GameObject.Find("CounterText");
            if (ct != null) {
                counterText = ct.GetComponent<Text>();
            }
        }
        if (gazePointer == null)
        {
            GameObject head = WaveVR_Render.Instance.gameObject;
            if (head != null)
                gazePointer = head.GetComponentInChildren<WaveVR_Reticle> ();
        }

        if (pointerData.pointerEnter == null && currentOverGO == null) {
            UpdateReticle(currentOverGO, pointerData);
            progressflag = true;
            if (progressText != null) {
                progressText.text = "";
            }
            if (counterText != null) {
                counterText.text = "";
            }
            if (gazePointer != null) {
                gazePointer.triggerProgressBar(false);
            }
            return;
        }

        if (!progressRate && !progressCounter) {  //  if no counting, reset trigger flag
            progressflag = true;
        }

        if (!progressRate || !progressCounter) {  //  clear counting content
            if (progressText != null && progressText.text != "") {
                progressText.text = "";
            }
            if (counterText != null && counterText.text != "") {
                counterText.text = "";
            }
        }
        // --------- Reticle -------- }

        if (pointerData.pointerEnter != currentOverGO)
        {
            #if UNITY_EDITOR
            Debug.Log ("pointerEnter: " + pointerData.pointerEnter + ", currentOverGO: " + currentOverGO);
            #endif
            //HandlePointerExitAndEnter (pointerData, currentOverGO);
            if (pointerData.pointerEnter != null)
            {
                ExecuteEvents.ExecuteHierarchy (pointerData.pointerEnter, pointerData, ExecuteEvents.pointerExitHandler);
                pointerData.pointerEnter = null;
            }
            ExecuteEvents.ExecuteHierarchy (currentOverGO, pointerData, ExecuteEvents.pointerEnterHandler);
            pointerData.pointerEnter = currentOverGO;

            gazeTime = Time.unscaledTime;

            // { ------- Reticle --------
            countingTime = Time.unscaledTime;
            UpdateReticle(currentOverGO, pointerData);
            // --------- Reticle -------- }
        }
        else
        {
            // { ------- Reticle --------
            if (progressflag) {   // begin to count, do initialization
                if (gazePointer != null) {
                    gazePointer.triggerProgressBar(true);
                }
                if (progressRate && progressText != null) {
                    progressText.text = "0%";
                }
                if (progressCounter && counterText != null) {
                    counterText.text = TimeToGaze.ToString();
                }
                countingTime = Time.unscaledTime;
                progressflag = false;  // counting the rate of waiting for clicking event
            }
            // --------- Reticle -------- }

            float elapsedTime = Time.unscaledTime;
            if (elapsedTime - gazeTime > TimeToGaze)
            {
                #if UNITY_EDITOR
                //Debug.Log ("Selected: {" + currentOverGO.name + "} over " + TimeToGaze + " seconds.");
                #endif
                sendEvent = true;
                gazeTime = Time.unscaledTime;

                // { ------- Reticle --------
                if (progressRate) {
                    if (progressText != null) {
                        progressText.text = "";
                    }
                }
                if (progressCounter) {
                    if (counterText != null) {
                        counterText.text = "";
                    }
                }
                if (gazePointer != null) {
                    gazePointer.triggerProgressBar(false);
                }
                progressflag = true;   // reset trigger flag after each counting is done
            } else {
                float rate = ((Time.unscaledTime - gazeTime) / TimeToGaze) * 100;
                if (gazePointer != null) {
                    gazePointer.setProgressBarTime(rate);
                }
                if (progressRate) {
                    if (progressText != null) {
                        progressText.text = Mathf.Floor(rate) + "%";
                    }
                }
                if (progressCounter) {
                    if (counterText != null) {
                        counterText.text = System.Math.Round(TimeToGaze - (Time.unscaledTime - countingTime), 2).ToString();
                    }
                }
                // --------- Reticle -------- }
            }
        }

        // Standalone Input Module information
        pointerData.delta = Vector2.zero;
        pointerData.dragging = false;

        DeselectIfSelectionChanged (currentOverGO, pointerData);

        if (sendEvent)
        {
            if (InputEvent == EGazeInputEvent.PointerClick)
            {
                ExecuteEvents.ExecuteHierarchy (currentOverGO, pointerData, ExecuteEvents.pointerClickHandler);
                pointerData.clickTime = Time.unscaledTime;
            } else if (InputEvent == EGazeInputEvent.PointerDown)
            {
                // like "mouse" action, press->release soon, do NOT keep the pointerPressRaycast cause do NOT need to controll "down" object while not gazing.
                pointerData.pressPosition = pointerData.position;
                pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;

                var _pointerDownGO = ExecuteEvents.ExecuteHierarchy (currentOverGO, pointerData, ExecuteEvents.pointerDownHandler);
                ExecuteEvents.ExecuteHierarchy (_pointerDownGO, pointerData, ExecuteEvents.pointerUpHandler);
            } else if (InputEvent == EGazeInputEvent.PointerSubmit)
            {
                ExecuteEvents.ExecuteHierarchy (currentOverGO, pointerData, ExecuteEvents.submitHandler);
            }
        }
    }

    private void GazeControl()
    {
        CastToCenterOfScreen ();
        OnTriggeGaze();
    }

    public override void Process()
    {
        GazeControl ();
    }
}
