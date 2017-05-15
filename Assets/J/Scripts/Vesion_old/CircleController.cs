using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleController : MonoBehaviour {

    public Transform target;
    private float normalDistance = 3;

    private float xSpeed = 250.0f;
    private float ySpeed = 120.0f;

    private int yMinLimit = -20;
    private int yMaxLimit = 80;

    private float x = 0.0f;
    private float y = 0.0f;

    private Vector3 screenPoint;
    private Vector3 offset;

    private Quaternion rotation = Quaternion.Euler(new Vector3(30f, 0f, 0f));
    private Vector3 CameraTarget;
    void Start()
    {

        CameraTarget = target.position;

        float z = target.transform.position.z - normalDistance;
        transform.position = rotation * new Vector3(transform.position.x, transform.position.y, z);

        transform.LookAt(target);

        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            var rotation = Quaternion.Euler(y, x, 0);
            var position = rotation * new Vector3(0.0f, 0.0f, -normalDistance) + CameraTarget;

            transform.rotation = rotation;
            transform.position = position;

        }  
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
