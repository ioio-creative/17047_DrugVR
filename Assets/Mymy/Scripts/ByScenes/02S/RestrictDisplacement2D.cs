using UnityEngine;
using UnityEngine.EventSystems;

// Unity UI Scroll Rect and Mask
// https://www.youtube.com/watch?v=9B7ahj1kaYs
// https://www.youtube.com/watch?v=tcU8yzv_xEw
public class RestrictDisplacement2D : MonoBehaviour,
    IHandleUiButton
{
    public enum ScrollRectConstrainStates
    {
        Unlocked,
        Horizontal,
        Vertical
    }

    [SerializeField]
    private RectTransform rectToConstrain;
    [SerializeField]
    private float lerpSpeed = 10f;

    [SerializeField]
    private float axisConstrainThreshold = 0.1f;
    private ScrollRectConstrainStates scrollState = ScrollRectConstrainStates.Unlocked; 

    //Position pre contrain
    private Vector3 dragDisplacementFromOriginalPos;

    private Vector3 originalPos;
    private bool isDragging;


    /* MonoBehaviour */

    private void Start()
    {
        originalPos = rectToConstrain.position;        
    }

    private void Update()
    {

        //Debug.Log("Anchored position: " + rectToConstrain.anchoredPosition);
        //Debug.Log("Local position: " + rectToConstrain.localPosition);
        //Debug.Log("Position: " + rectToConstrain.position);

        //Debug.Log("Displacement: " + displacementFromOriginaldPos);

        dragDisplacementFromOriginalPos =
            rectToConstrain.position - originalPos;

        switch (scrollState)
        {
            case ScrollRectConstrainStates.Unlocked:                
                // if rectToConstrain is dragged more in the x-direction
                if (Mathf.Abs(dragDisplacementFromOriginalPos.x) > Mathf.Abs(dragDisplacementFromOriginalPos.y))
                {
                    // restrict y displacement
                    rectToConstrain.position = new Vector3
                    {
                        x = rectToConstrain.position.x,
                        y = originalPos.y,
                        z = originalPos.z
                    };

                }
                else  // if rectToConstrain is dragged more in the y-direction
                {
                    // restrict x displacement
                    rectToConstrain.position = new Vector3
                    {
                        x = originalPos.x,
                        y = rectToConstrain.position.y,
                        z = originalPos.z
                    };
                }
                if (Mathf.Abs(dragDisplacementFromOriginalPos.x) > axisConstrainThreshold)
                {
                    scrollState = ScrollRectConstrainStates.Horizontal;
                }
                else if (Mathf.Abs(dragDisplacementFromOriginalPos.y) > axisConstrainThreshold)
                {
                    scrollState = ScrollRectConstrainStates.Vertical;
                }

                break;
            case ScrollRectConstrainStates.Horizontal:
                // restrict y displacement
                rectToConstrain.position = new Vector3
                {
                    x = rectToConstrain.position.x,
                    y = originalPos.y,
                    z = originalPos.z
                };
                break;
            case ScrollRectConstrainStates.Vertical:
                // restrict x displacement
                rectToConstrain.position = new Vector3
                {
                    x = originalPos.x,
                    y = rectToConstrain.position.y,
                    z = originalPos.z
                };
                break;
            default:break;
        }

        if (!isDragging)
        {
            LerpToPosition(originalPos);
            switch (scrollState)
            {
                case ScrollRectConstrainStates.Horizontal:
                    if (Mathf.Abs(rectToConstrain.position.x) < axisConstrainThreshold)
                    {
                        scrollState = ScrollRectConstrainStates.Unlocked;
                    }
                    break;
                case ScrollRectConstrainStates.Vertical:
                    if (Mathf.Abs(rectToConstrain.position.y) < axisConstrainThreshold)
                    {
                        scrollState = ScrollRectConstrainStates.Unlocked;
                    }
                    break;
                default:break;
            }
        }
    }

    /* end of MonoBehaviour */


    private void LerpToPosition(Vector3 targetPosition)
    {
        Vector3 newPos = Vector3.Lerp(rectToConstrain.position, targetPosition, Time.deltaTime * lerpSpeed);
        rectToConstrain.position = newPos;
    }

    public void HandleEnter()
    {
        
    }

    public void HandleDown()
    {
        isDragging = true;
    }

    public void HandleExit()
    {
        
    }

    public void HandleUp()
    {
        isDragging = false;
    }
}
