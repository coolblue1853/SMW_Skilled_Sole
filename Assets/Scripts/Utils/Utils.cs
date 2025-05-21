using UnityEngine;

public class Utils
{
    public static void GetCameraFlatDirections(Transform _cameraTransform, out Vector3 cameraForward, out Vector3 cameraRight)
    {
        cameraForward = _cameraTransform.forward;
        cameraRight = _cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
    }
}

