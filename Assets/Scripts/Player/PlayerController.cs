using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerStatHandler _statHandler;
    private Transform _cameraTransform;

    [Header("Walk_run")]
    [SerializeField] private float _runUseStemina = 0.1f;
    [SerializeField] private float _restoreStemina = 0.3f;
    [SerializeField] private float _restoreDelay = 2f; // ���� �� ȸ������ ��� �ð�
    [SerializeField] private float _useSteminaInterval = 0.1f;
    [SerializeField] private float _runToggleCooldown = 1f; // ���������� �޸��� -> �ȱ� ��ȯ�ð�.
    [SerializeField] private Vector2 _moveInput;

    [Header("Jump")]
    [SerializeField] private float _initialJumpForce = 10f;
    [SerializeField] private float _jumpHoldGravity = 0.5f; // �����̽� ������ �߷°�
    [SerializeField] private float _fallGravity = 2f;  // �������� �߷°�
    [SerializeField] private float _maxJumpHoldTime = 0.2f;

    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private Transform _groundPivot;
    [SerializeField] private LayerMask _ladderLayerMask;
    [SerializeField] private LayerMask _wallJumpLayerMask;

    private Rigidbody _rigidbody;
    private Vector3 _inputDir;
    private float _jumpTimer;
    [SerializeField] private float _runToggleTimer = 0f;
    private float _steminaTimer = 0f;
    private float _restoreDelayTimer = 0f;
    private float _restoreIntervalTimer = 0f;
    private bool _isJumping;
    [SerializeField] private bool _isRunning = false;
    private bool _isMoving = false;
    [SerializeField]  private bool _availableMove = true;

    // ��ٸ� ���� ����
    [SerializeField] private Transform _ladderRayPivot;
    private bool _isOnLadder = false;
    private bool _isClimbing = false;
    private Vector3 _ladderForward; // ��ٸ� ���� ĳ��

    // ������ ���ú���
    [SerializeField] private float wallJumpForce = 8f;
    [SerializeField] private float ladderGravity = -2f; // ���ϴ� �߷� ����
    private bool _isWallJumpalbe = false;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _statHandler = GetComponent<PlayerStatHandler>();
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (_availableMove)
        {
            Move();
        }

    }
    private void Update()
    {
        CheckLadder();


        if (_isClimbing && _isOnLadder)
        {
            ClimbLadder();
            return;
        }

        if (_isJumping)
        {
            _jumpTimer += Time.deltaTime;

            if (_jumpTimer > _maxJumpHoldTime)
            {
                _isJumping = false;
            }
        }


        if (!_isMoving && _isRunning && _runToggleTimer > 0f)
            _runToggleTimer -= Time.deltaTime;

        // ���͹� Ÿ���� ������ �ڵ����� �޸��� -> �ȱ���
        if (_isMoving == false && _isRunning)
        {
            if (_runToggleTimer <= 0f)
            {
                _isRunning = false;
                _runToggleTimer = _runToggleCooldown;
            }
        }

        HandleSteminaDrain();
        HandleSteminaRestore();
        // ������ ���� �߷°� ����
        AdjustGravity();
    }

    public void OnMove(InputValue input)
    {
        if (!_availableMove)
            return;

        if (_isRunning)
            _runToggleTimer = _runToggleCooldown;
        _moveInput = input.Get<Vector2>();
    }
    public void OnJump(InputValue input)
    {
        if (_isClimbing && input.isPressed)
        {
            _isOnLadder = false;
            _isClimbing = false;
            _rigidbody.useGravity = true;

            //������
            if (_isWallJumpalbe)
            {
                _isWallJumpalbe = false;
                SetAvailableMove(false);
                Vector3 jumpDir = (-_ladderForward.normalized + Vector3.up).normalized;
                _rigidbody.velocity = Vector3.zero; // ���� �ӵ� �ʱ�ȭ
                _rigidbody.AddForce(jumpDir * wallJumpForce, ForceMode.Impulse);
                StartCoroutine(EnableMoveAfterDelay(0.5f));
            }
   
            return;
        }

        if (input.isPressed && _availableMove)
        {
            if (CheckGrounded())
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
    public void OnRun(InputValue input)
    {
        if (_isMoving && _statHandler.Stemina >= _runUseStemina)
        {
            if (_runToggleTimer >= 0f)
            {
                _isRunning = !_isRunning; // ���
                _runToggleTimer = _runToggleCooldown;
            }
        }
    }

    private void Move()
    {
        if (!_availableMove) return;

        _inputDir = new Vector3(_moveInput.x, 0, _moveInput.y);
        _isMoving = _inputDir.sqrMagnitude > 0.01f;

        // ī�޶� ���� ���� ���
        Vector3 cameraForward = _cameraTransform.forward;
        Vector3 cameraRight = _cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDir;

        if (_isOnLadder) 
        {
            // ��ٸ��� ������ ��ٸ� ������ ������
            moveDir = _ladderForward;
        }
        else
        {
            moveDir = cameraForward * _inputDir.z + cameraRight * _inputDir.x;
        }

        float speed = _isRunning ? _statHandler.RunSpeed : _statHandler.WalkSpeed;

        if (_statHandler.Stemina < _runUseStemina)
            _isRunning = false;

        Vector3 horizontalVelocity = moveDir * speed;

        if (Physics.Raycast(_ladderRayPivot.position, horizontalVelocity.normalized, out RaycastHit hit, 0.5f))
        {
            Vector3 slideDir = Vector3.ProjectOnPlane(horizontalVelocity, hit.normal);
            horizontalVelocity = slideDir;
        }

        _rigidbody.velocity = new Vector3(horizontalVelocity.x, _rigidbody.velocity.y, horizontalVelocity.z);

        if (_isMoving && _availableMove)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }



    private void ClimbLadder()
    {
        float climbSpeed = _statHandler.WalkSpeed * 0.75f; // ��ٸ� �ӵ��� �ȴ� �ӵ��� 75%
        Vector3 climbVelocity = new Vector3(0, _moveInput.y * climbSpeed, 0);
        _rigidbody.velocity = climbVelocity;
    }

    bool CheckGrounded()
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
    private void CheckLadder()
    {
        RaycastHit hit;
        Vector3 origin = _ladderRayPivot.position;

        _inputDir = new Vector3(_moveInput.x, 0, _moveInput.y);

        Vector3 cameraForward = _cameraTransform.forward;
        Vector3 cameraRight = _cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = (cameraForward * _inputDir.z + cameraRight * _inputDir.x).normalized;

        Debug.DrawRay(origin, direction * 0.6f, Color.red, 0.1f);

        if (Physics.Raycast(origin, direction, out hit, 0.6f, _ladderLayerMask))
        {
            _isOnLadder = true;

            _ladderForward = -hit.normal;
            if (Mathf.Abs(_moveInput.y) > 0.1f)
            {
                _isClimbing = true;
                _rigidbody.useGravity = false;
            }
        }
        else if (Physics.Raycast(origin, direction, out hit, 0.6f, _wallJumpLayerMask))
        {
            _ladderForward = -hit.normal;
            if (Mathf.Abs(_moveInput.y) > 0.1f)
            {
                _isClimbing = true;
                _isWallJumpalbe = true;
            }
        }
        else 
        {
            if(_inputDir != Vector3.zero)
            {
                _isOnLadder = false;
                _isClimbing = false;
                _isWallJumpalbe = false;
                _rigidbody.useGravity = true;
            }

        }
    }




    private void AdjustGravity()
    {
        float gravityForce = Mathf.Abs(Physics.gravity.y);

        if (_rigidbody.velocity.y > 0)
        {
            // ���� �ö󰡴� ��, �����̽��ٸ� �������� �������������� �߷°��� �ٸ��� �ؼ�, ������, ������ ����
            _rigidbody.AddForce(Vector3.down * gravityForce * (_isJumping ? _jumpHoldGravity : _fallGravity), ForceMode.Acceleration);
        }
        else if (_rigidbody.velocity.y < 0)
        {
            // �������� �� 
            _rigidbody.AddForce(Vector3.down * gravityForce * _fallGravity, ForceMode.Acceleration);
        }
    }

    private void HandleSteminaDrain()
    {
        if (_isRunning)
        {
            _steminaTimer += Time.deltaTime;

            if (_steminaTimer >= _useSteminaInterval)
            {
                _steminaTimer = 0f;
                _statHandler.Stemina -= _runUseStemina;
            }
        }
        else
        {
            _steminaTimer = 0f;
        }
    }
    private void HandleSteminaRestore()
    {
        if (_isRunning)
        {
            _restoreDelayTimer = 0f;
            _restoreIntervalTimer = 0f;
            return;
        }

        if (_statHandler.MaxStemina > _statHandler.Stemina)
        {
            _restoreDelayTimer += Time.deltaTime;

            if (_restoreDelayTimer >= _restoreDelay)
            {
                _restoreIntervalTimer += Time.deltaTime;

                if (_restoreIntervalTimer >= _useSteminaInterval)
                {
                    _restoreIntervalTimer = 0f;
                    _statHandler.Stemina += _restoreStemina;
                }
            }
        }

    }

    public void SetAvailableMove(bool value)
    {
        if (_inputDir == Vector3.zero)
            _moveInput = Vector3.zero;

        _availableMove = value;
        _rigidbody.velocity = Vector3.zero;
        _isMoving = false;
    }
    IEnumerator EnableMoveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetAvailableMove(true);
    }
}
