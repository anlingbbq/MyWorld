using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerRotate : MonoBehaviour
{
    public Transform _player;

    private float _mouseX;
    private float _mouseY;

    public const float _minRotateY = -60;
    public const float _maxRotateY = 60;

    public float rotateSpeed = 10;
    public float mouseSpeedX = 4;
    public float mouseSpeedY = 4;

    private Quaternion screenMovementSpace;
    private Vector3 screenMovementForward, screenMovementRight;

    private void Update()
    {
        screenMovementSpace = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        screenMovementForward = screenMovementSpace * Vector3.forward;
        screenMovementRight = screenMovementSpace * Vector3.right;

        float h = Input.GetAxisRaw()

        if (Input.GetMouseButton(1))
        {
            _mouseX += Input.GetAxis("Mouse X") * mouseSpeedX;
            _mouseY += Input.GetAxis("Mouse Y") * -mouseSpeedY;

            _mouseY = ClampAngle(_minRotateY, _maxRotateY, _mouseY);
        }
        Quaternion rotation = Quaternion.Euler(_mouseY, _mouseX, 0.0f);
        _player.rotation = Quaternion.Lerp(_player.rotation, rotation, Time.deltaTime * rotateSpeed);
    }

    private static float ClampAngle(float min, float max, float angle)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
