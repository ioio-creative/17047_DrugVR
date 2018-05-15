using UnityEngine;

public static class AngleCalculations
{
    // Note: azimuth is signed; zenith is unsigned
    public static void CalculateAzimuthAndZenithFromPointerDirection(Vector3 pointerDirection,
        Vector3 StaticUpDirection, Vector3 StaticForwardDirection,
        Vector3 debugRayOrigin,
        Color debugRayColorForPointer, Color debugRayColorForPointerProjectionOnFloor,
        ref float signedAzimuth, ref float unsignedZenith)
    {
        // The lines commented do the same thing.
        // But in the end, we chose to use Unity Vector3 API

        //Vector3 normedProjectionOnFloor = Vector3.Normalize(pointerDirection - new Vector3(0, pointerDirection.y, 0));
        Vector3 normedProjectionOnFloor = Vector3.Normalize(Vector3.ProjectOnPlane(pointerDirection, StaticUpDirection));
        //zenith = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(pointerDirection, StaticUpDirection));
        unsignedZenith = Vector3.Angle(pointerDirection, StaticUpDirection);

        Debug.DrawRay(debugRayOrigin, pointerDirection, debugRayColorForPointer);
        Debug.DrawRay(debugRayOrigin, normedProjectionOnFloor, debugRayColorForPointerProjectionOnFloor);

        //float signedMagnitudeOfSineAzimuth = Vector3.Dot(Vector3.Cross(StaticForwardDirection, normedProjectionOnFloor), StaticUpDirection);
        //azimuth = Mathf.Rad2Deg * Mathf.Asin(signedMagnitudeOfSineAzimuth);
        signedAzimuth = Vector3.SignedAngle(normedProjectionOnFloor, StaticForwardDirection, StaticUpDirection);
    }
}
