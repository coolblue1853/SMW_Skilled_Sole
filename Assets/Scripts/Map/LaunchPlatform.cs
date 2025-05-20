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
        // Y축 고정 회전
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

        Vector3 direction = firePoint.forward; // 이 방향이 부모 회전 + 포신 기울기 반영됨

        Rigidbody rb = _player.GetComponent<Rigidbody>();
        rb.AddForce(direction * shootForce, ForceMode.Impulse);
        if (_targetController != null)
            StartCoroutine(EnableMoveAfterDelay(0.5f)); // ← 예: 0.5초 후 이동 가능

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
