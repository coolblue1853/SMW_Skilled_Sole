using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _player;

    [Header("Settings")]
    [SerializeField] private float _zoomSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private float _minZoom = 2f;
    [SerializeField] private float _maxZoom = 2f;

    private float _zoomDistance = 10f;
    private float _yaw = 0f; // �¿� ȸ����
    private float _pitch = 20f; // ���� ȸ����

    // ī�޶� �� �Է�
    private Vector2 _lookInput = Vector2.zero;
    [SerializeField]  private bool _isRightMousePressed = false;

    public void OnLook(InputValue input)
    {
        if (_isRightMousePressed)
        {
            _lookInput = input.Get<Vector2>();

            _yaw += _lookInput.x * _rotationSpeed * Time.deltaTime;
            _pitch -= _lookInput.y * _rotationSpeed * Time.deltaTime;
            _pitch = Mathf.Clamp(_pitch, 5f, 80f); // ���� ����
        }
    }

    public void OnZoom(InputValue input)
    {
        float scroll = input.Get<float>();
        scroll /= 120f; // �� �� Nomalize
        _zoomDistance -= scroll * _zoomSpeed; // ���콺 �Է��� �ݴ��̱� ������ - ��
        _zoomDistance = Mathf.Clamp(_zoomDistance, _minZoom, _maxZoom);

    }   

    public void OnRightClick(InputValue input)
    {
        _isRightMousePressed = input.isPressed;
    }
        
    private void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
        Vector3 cameraOffset = rotation * new Vector3(0, 0, -_zoomDistance);

        transform.position = _player.position + cameraOffset;
        transform.LookAt(_player.position);
    }

}
