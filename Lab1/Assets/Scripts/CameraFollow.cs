using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform carTransform;
    private Vector3 _offset = new Vector3(0, 5, -18);

    private void LateUpdate()
    {
       
        // Calculate the camera's position behind the car, relative to its rotation
        Vector3 desiredPosition = carTransform.TransformPoint(_offset);
        transform.position = desiredPosition;

        // Make the camera look at the car
        transform.LookAt(carTransform);
    }
}
