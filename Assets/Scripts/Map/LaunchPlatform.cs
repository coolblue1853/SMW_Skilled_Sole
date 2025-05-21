using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchPlatform : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _shootForce = 10f;
    [SerializeField] private float _rotationSpeed = 90f;
    [SerializeField] private float _releasTime = 1f;

    private GameObject _player;
    private PlayerController _targetController;

    void Update()
    {
        // Y축 고정 회전
        transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }

    // 입력시 발사대 작동
    public void OnJump(InputValue input)
    {
        ActiveJumper();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            _player = other.gameObject;
            _targetController = other.GetComponent<PlayerController>();
            _targetController.SetAvailableMove(false);
            other.transform.position = transform.position;
        }
    }

    // 포신 방향으로 플레이어 발사 함수
    void ActiveJumper()
    {
        if (_player == null)
            return;

        _player.transform.position = _firePoint.position;
        _player.transform.rotation = Quaternion.identity;

        Vector3 direction = _firePoint.forward; // 이 방향이 부모 회전 + 포신 기울기 반영됨

        Rigidbody rb = _player.GetComponent<Rigidbody>();
        rb.AddForce(direction * _shootForce, ForceMode.Impulse);
        if (_targetController != null)
            StartCoroutine(EnableMoveAfterDelay(_releasTime)); // ← 예: 0.5초 후 이동 가능

        _player = null;
    }

    IEnumerator EnableMoveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_targetController != null)
            _targetController.SetAvailableMove(true);
        _targetController = null;
    }

}
