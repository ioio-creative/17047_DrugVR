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
    private float secondsToWaitBeforeStartingRotation;
    [SerializeField]
    private float secondsForEachRotation;

    private Transform thisTransform;
    private int numOfRotationsMade;


    private IEnumerator Start()
    {
        thisTransform = transform;
        numOfRotationsMade = 0;

        yield return new WaitForSeconds(secondsToWaitBeforeStartingRotation);
        StartCoroutine(RotateIntermittenly());
    }

    private IEnumerator RotateIntermittenly()
    {
        Quaternion intialRotation = thisTransform.rotation;
        Quaternion fromRotation = intialRotation;
        Quaternion toRotation = Quaternion.AngleAxis(360 / numOfPartsToDivideTheCircle, rotationAxis);

        float startTime = Time.time;

        while (true)
        {
            if (thisTransform.rotation == toRotation)
            {
                numOfRotationsMade++;

                fromRotation = thisTransform.rotation;
                // even though numOfPartsToDivideTheCircle may not divide 360, this won't result in any error
                toRotation = Quaternion.AngleAxis(numOfRotationsMade * 360 / numOfPartsToDivideTheCircle, rotationAxis);

                // didn't use modulus here because Quaternion.AngleAxis(0, rotationAxis) would result in no rotation

                // also didn't use the following, somehow won't work after one complete rotation
                //if (numOfRotationsMade == numOfPartsToDivideTheCircle)
                //{
                //    numOfRotationsMade = 0;
                //}
                //Debug.Log("num of rotations: " + numOfRotationsMade + ", current: " + thisTransform.rotation + ", to: " + toRotation);

                // so I just allow numOfRotationsMade continue to increase, it's OK.

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
