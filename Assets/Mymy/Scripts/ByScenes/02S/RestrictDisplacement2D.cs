using UnityEngine;

// Unity UI Scroll Rect and Mask
// https://www.youtube.com/watch?v=9B7ahj1kaYs
// https://www.youtube.com/watch?v=tcU8yzv_xEw
public class RestrictDisplacement2D : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectToConstrain;
    [SerializeField]
    private float lerpSpeed = 10f;


    private Vector3 originalPos;
    private bool isDragging;


    /* MonoBehaviour */

    private void Start()
    {
        originalPos = rectToConstrain.position;        
    }

    private void Update()
    {
        Vector3 displacementFromOriginaldPos =
            rectToConstrain.position - originalPos;

        //Debug.Log("Anchored position: " + rectToConstrain.anchoredPosition);
        //Debug.Log("Local position: " + rectToConstrain.localPosition);
        //Debug.Log("Position: " + rectToConstrain.position);

        //Debug.Log("Displacement: " + displacementFromOriginaldPos);

        // if rectToConstrain is dragged more in the x-direction
        if (Mathf.Abs(displacementFromOriginaldPos.x) > Mathf.Abs(displacementFromOriginaldPos.y))
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

        if (!isDragging)
        {
            LerpToPosition(originalPos);
        }
    }

    /* end of MonoBehaviour */


    private void LerpToPosition(Vector3 newPosition)
    {
        Vector3 newPos = Vector3.Lerp(rectToConstrain.position, newPosition, Time.deltaTime * lerpSpeed);
        rectToConstrain.position = newPos;
    }

    public void HandleBeginDrag()
    {
        isDragging = true;
    }

    public void HandleEndDrag()
    {
        isDragging = false;
    }
}
