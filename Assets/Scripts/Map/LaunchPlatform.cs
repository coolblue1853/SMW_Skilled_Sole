using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LaunchPlatform : MonoBehaviour
{
    private GameObject _player;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private float rotationSpeed = 90f;
    private PlayerController _targetController;

    void Update()
    {
        // Y�� ���� ȸ��
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
     
    }
    public void OnJump(InputValue input)
    {
        FireProjectile();
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


    void FireProjectile()
    {
        if (_player == null)
            return;

        _player.transform.position = firePoint.position;
        _player.transform.rotation = Quaternion.identity;

        Vector3 direction = firePoint.forward; // �� ������ �θ� ȸ�� + ���� ���� �ݿ���

        Rigidbody rb = _player.GetComponent<Rigidbody>();
        rb.AddForce(direction * shootForce, ForceMode.Impulse);
        if (_targetController != null)
            StartCoroutine(EnableMoveAfterDelay(0.5f)); // �� ��: 0.5�� �� �̵� ����

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
