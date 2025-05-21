using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerStatHandler _statHandler;
    private ResourcesController _resourcesController;
    private Transform _cameraTransform;

    [Header("Walk_run")]
    [SerializeField] private float _runToggleCooldown = 1f; // ���������� �޸��� -> �ȱ� ��ȯ�ð�.
    [SerializeField] private Vector2 _moveInput;
    [SerializeField] private float _runToggleTimer = 0f;
    public bool IsRunning = false;
    [SerializeField] private bool _availableMove = true;

    [Header("Jump")]
    [SerializeField] private float _initialJumpForce = 10f;
    [SerializeField] private float _jumpHoldGravity = 0.5f; // �����̽� ������ �߷°�
    [SerializeField] private float _fallGravity = 2f;  // �������� �߷°�
    [SerializeField] private float _maxJumpHoldTime = 0.2f;

    private GroundChecker _groundChecker = new GroundChecker();
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private Transform _groundPivot;

    [SerializeField] private LayerMask _ladderLayerMask;
    [SerializeField] private LayerMask _wallJumpLayerMask;

    private Rigidbody _rigidbody;
    private Vector3 _inputDir;
    private float _jumpTimer;
    private bool _isJumping;
    private bool _isMoving = false;

    // ��ٸ� ���� ����
    [Header("Ladder")]
    [SerializeField] private Transform _ladderRayPivot;
    private bool _isOnLadder = false;
    private bool _isClimbing = false;
    private Vector3 _ladderForward; // ��ٸ� ���� ĳ��

    [Header("WallJump")]
    // ������ ���ú���
    [SerializeField] private float wallJumpForce = 8f;
    [SerializeField] private float ladderGravity = -2f; // ���ϴ� �߷� ����
    private bool _isWallJumpalbe = false;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _statHandler = GetComponent<PlayerStatHandler>();
        _resourcesController = GetComponent<ResourcesController>();
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

        if (!_isMoving && IsRunning && _runToggleTimer > 0f)
            _runToggleTimer -= Time.deltaTime;

        // ���͹� Ÿ���� ������ �ڵ����� �޸��� -> �ȱ���
        if (_isMoving == false && IsRunning)
        {
            if (_runToggleTimer <= 0f)
            {
                IsRunning = false;
                _runToggleTimer = _runToggleCooldown;
            }
        }
        // ������ ���� �߷°� ����
        AdjustGravity();
    }

    // ������ �Է�
    public void OnMove(InputValue input)
    {
        if (!_availableMove)
            return;

        if (IsRunning)
            _runToggleTimer = _runToggleCooldown;
        _moveInput = input.Get<Vector2>();
    }

    // ���� �Է�
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
            if (_groundChecker.CheckGrounded(_groundLayerMask,_groundPivot))
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

    // �޸��� ��ǲ
    public void OnRun(InputValue input)
    {
        if (_isMoving && _statHandler.Stemina >= _resourcesController.RunUseStemina)
        {
            if (_runToggleTimer >= 0f)
            {
                IsRunning = !IsRunning; // ���
                _runToggleTimer = _runToggleCooldown;
            }
        }
    }

    // ������ �Լ�
    private void Move()
    {
        if (!_availableMove) return;

        _inputDir = new Vector3(_moveInput.x, 0, _moveInput.y);
        _isMoving = _inputDir.sqrMagnitude > 0.01f;

        // ī�޶� ���� ���� ���
      Utils. GetCameraFlatDirections(_cameraTransform,out Vector3 cameraForward, out Vector3 cameraRight);
        Vector3 moveDir;

        if (_isOnLadder)
            moveDir = _ladderForward;
        else
            moveDir = cameraForward * _inputDir.z + cameraRight * _inputDir.x;

        float speed = IsRunning ? _statHandler.RunSpeed : _statHandler.WalkSpeed;

        if (_statHandler.Stemina < _resourcesController.RunUseStemina)
            IsRunning = false;

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
    // ��ٸ�, ���� Ȯ���ϴ� �Լ�
    public void CheckLadder()
    {
        RaycastHit hit;
        Vector3 origin = _ladderRayPivot.position;

        _inputDir = new Vector3(_moveInput.x, 0, _moveInput.y);
        Utils.GetCameraFlatDirections(_cameraTransform, out Vector3 cameraForward, out Vector3 cameraRight);
        Vector3 direction = (cameraForward * _inputDir.z + cameraRight * _inputDir.x).normalized;

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
            if (_inputDir != Vector3.zero)
            {
                _isOnLadder = false;
                _isClimbing = false;
                _isWallJumpalbe = false;
                _rigidbody.useGravity = true;
            }
        }
    }

    // ��ٸ��� ������� ������ �Լ�
    private void ClimbLadder()
    {
        float climbSpeed = _statHandler.WalkSpeed * 0.75f; // ��ٸ� �ӵ��� �ȴ� �ӵ��� 75%
        Vector3 climbVelocity = new Vector3(0, _moveInput.y * climbSpeed, 0);
        _rigidbody.velocity = climbVelocity;
    }

    // ������, �������� ���� �߷� ���� �Լ�
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

   
    // �̵� ������ �������� �����ϴ� �Լ�
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
