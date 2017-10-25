using UnityEngine;
using System.Collections;

public class FlyCamera : MonoBehaviour
{
    public float Speed = 10;
    public float xSpeed = 200;

    public float ySpeed = 200;
    public float yMinLimit = -50;
    public float yMaxLimit = 50;

    private float _rotateX;
    private float _rotateY;
    private float damping = 5.0f;

    // Update is called once per frame
    void Update()
    {
        float H = Input.GetAxis("Horizontal");
        float V = Input.GetAxis("Vertical");
        Vector3 cf = transform.forward;
        Vector3 cr = transform.right;
        transform.position += cf * Time.deltaTime * V * Speed;
        transform.position += cr * Time.deltaTime * H * Speed;

        if (Input.GetMouseButton(1))
        {
            _rotateX += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            _rotateY -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            _rotateY = ClampAngle(_rotateY, yMinLimit, yMaxLimit);
        }
        Quaternion rotation = Quaternion.Euler(_rotateY, _rotateX, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * damping * Speed);

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
