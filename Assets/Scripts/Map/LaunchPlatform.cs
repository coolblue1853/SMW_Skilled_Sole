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
        // Y�� ���� ȸ��
        transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }

    // �Է½� �߻�� �۵�
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

    // ���� �������� �÷��̾� �߻� �Լ�
    void ActiveJumper()
    {
        if (_player == null)
            return;

        _player.transform.position = _firePoint.position;
        _player.transform.rotation = Quaternion.identity;

        Vector3 direction = _firePoint.forward; // �� ������ �θ� ȸ�� + ���� ���� �ݿ���

        Rigidbody rb = _player.GetComponent<Rigidbody>();
        rb.AddForce(direction * _shootForce, ForceMode.Impulse);
        if (_targetController != null)
            StartCoroutine(EnableMoveAfterDelay(_releasTime)); // �� ��: 0.5�� �� �̵� ����

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
