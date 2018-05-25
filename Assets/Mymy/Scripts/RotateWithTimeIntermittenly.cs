using System.Collections;
using UnityEngine;

public class RotateWithTimeIntermittenly : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotationOrigin;
    [SerializeField]
    private Vector3 rotationAxis;
    [SerializeField]
    private int numOfPartsToDivideTheCircle;
    [SerializeField]
    private float secondsToWaitBetweenRotations;
    [SerializeField]
    private float secondsForEachRotation;

    private Transform thisTransform;


    private void Start()
    {
        thisTransform = transform;
        StartCoroutine(RotateIntermittenly());
    }

    private IEnumerator RotateIntermittenly()
    {
        Quaternion fromRotation = Quaternion.identity;
        Quaternion toRotation = fromRotation;
        Quaternion amountToRotateEachTime = 
            Quaternion.AngleAxis(360 / numOfPartsToDivideTheCircle, rotationAxis);

        float startTime = Time.time;

        while (true)
        {
            if (thisTransform.rotation == toRotation)
            {
                fromRotation = thisTransform.rotation;
                // https://docs.unity3d.com/ScriptReference/Quaternion-operator_multiply.html
                toRotation = thisTransform.rotation * amountToRotateEachTime;

                yield return new WaitForSeconds(secondsToWaitBetweenRotations);

                startTime = Time.time;
            }
            else
            {
                thisTransform.rotation = Quaternion.Slerp(
                    fromRotation, toRotation, (Time.time - startTime) / secondsForEachRotation);
            }

            yield return null;
        }                
    }
}
