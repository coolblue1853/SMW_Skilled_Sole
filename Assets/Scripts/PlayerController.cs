using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector2 _moveInput;
    [SerializeField] private float _initialJumpForce = 10f; 
    [SerializeField] private float _jumpHoldGravityScale = 0.5f; // �����̽� ������ �߷°�
    [SerializeField] private float _fallGravityScale = 2f;  // �������� �߷°�
    [SerializeField] private float _maxJumpHoldTime = 0.2f; 

    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private Transform _groundPivot;

    private Rigidbody _rigidbody;
    private bool _isJumping;
    private float _jumpTimer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void Update()
    {
        if (_isJumping)
        {
            _jumpTimer += Time.deltaTime;

            if (_jumpTimer > _maxJumpHoldTime)
            {
                _isJumping = false;
            }
        }

        // ������ ���� �߷°� ����
        AdjustGravity();
    }

    public void OnMove(InputValue input)
    {
        _moveInput = input.Get<Vector2>();
    }
    public void OnJump(InputValue input)
    {
        if (input.isPressed)
        {
            if (IsGrounded())
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z); // Y�ӵ� �ʱ�ȭ
                _rigidbody.AddForce(Vector3.up * _initialJumpForce, ForceMode.Impulse);
                _isJumping = true;
                _jumpTimer = 0f;
            }
        }
        else
        {
            _isJumping = false;
        }
    }


    private void Move()
    {
        Vector3 dir = new Vector3(_moveInput.x, 0, _moveInput.y);
        _rigidbody.velocity = dir * _moveSpeed + new Vector3(0, _rigidbody.velocity.y, 0);
    }
    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(_groundPivot.position + (_groundPivot.forward * 0.2f) + (_groundPivot.up * 0.1f), Vector3.down),
            new Ray(_groundPivot.position + (-_groundPivot.forward * 0.2f) + (_groundPivot.up * 0.1f), Vector3.down),
            new Ray(_groundPivot.position + (_groundPivot.right * 0.2f) + (_groundPivot.up * 0.1f), Vector3.down),
            new Ray(_groundPivot.position + (-_groundPivot.right * 0.2f) +(_groundPivot.up * 0.1f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            Debug.DrawRay(rays[i].origin, rays[i].direction * 0.2f, Color.red, 0.1f);

            if (Physics.Raycast(rays[i], 0.2f, _groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    private void AdjustGravity()
    {
        float gravityForce = Mathf.Abs(Physics.gravity.y);

        if (_rigidbody.velocity.y > 0)
        {
            // ���� �ö󰡴� ��, �����̽��ٸ� �������� �������������� �߷°��� �ٸ��� �ؼ�, ������, ������ ����
            _rigidbody.AddForce(Vector3.down * gravityForce * (_isJumping ? _jumpHoldGravityScale : _fallGravityScale), ForceMode.Acceleration);
        }
        else if (_rigidbody.velocity.y < 0)
        {
            // �������� �� 
            _rigidbody.AddForce(Vector3.down * gravityForce * _fallGravityScale, ForceMode.Acceleration);
        }
    }
}
