using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerRotate : MonoBehaviour
{
    public Transform _player;

    private float _mouseX;
    private float _mouseY;

    public const float minRotateY = -120;
    public const float maxRotateY = 60;

    public float moveSpeed = 10;
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

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        _player.transform.position += screenMovementForward * v * Time.deltaTime * rotateSpeed;
        _player.transform.position += screenMovementRight * h * Time.deltaTime * rotateSpeed;

        if (Input.GetMouseButton(1))
        {
            _mouseX += Input.GetAxis("Mouse X") * mouseSpeedX;
            _mouseY += Input.GetAxis("Mouse Y") * -mouseSpeedY;

            _mouseY = ClampAngle(minRotateY, maxRotateY, _mouseY);
        }
        Quaternion rotation = Quaternion.Euler(_mouseY, _mouseX, 0.0f);
        _player.rotation = Quaternion.Lerp(_player.rotation, rotation, Time.deltaTime * rotateSpeed);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 pos = _player.transform.position;
            pos.y += Time.deltaTime * moveSpeed;
            _player.transform.position = pos;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            Vector3 pos = _player.transform.position;
            pos.y -= Time.deltaTime * moveSpeed;
            _player.transform.position = pos;
        }
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
