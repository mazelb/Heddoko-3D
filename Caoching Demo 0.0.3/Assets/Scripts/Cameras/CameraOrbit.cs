using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour {

    public Transform target;
    public float distance = 10.0f;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;
    public float zSpeed = 120.0f;

    public float yMinLimit = -20;
    public float yMaxLimit = 80;

    private double x = 0.0;
    private double y = 0.0;
    private double z = 0.0;


    // Use this for initialization
    void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        //z = Distance;
        z = Vector3.Distance(transform.position, target.position);

        // Make the rigid body not change rotation
        if (GetComponent< Rigidbody > ())
            GetComponent< Rigidbody > ().freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (target && Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;
            if(Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                z -= Input.GetAxis("Mouse ScrollWheel") * zSpeed * 0.02;
            }
            else
            {
                z += Input.GetAxis("Mouse ScrollWheel") * zSpeed * 0.02;
            }
            
            y = ClampAngle((float)y, yMinLimit, yMaxLimit);
          
            var rotation = Quaternion.Euler((float)y, (float)x, 0);
            Vector3 vec = new Vector3(0.0f, 0.0f, (float)-z);
            var position = rotation * vec + target.position;

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
