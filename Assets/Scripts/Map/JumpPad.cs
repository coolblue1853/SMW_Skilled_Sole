using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float _jumpForce;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Player"))
            return;

        // 충돌 지점이 점프패드보다 위쪽이면 작동
        ContactPoint contact = collision.contacts[0];
        if (contact.point.y > transform.position.y + 0.1f)
        {
            var rb = collision.transform.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }
}
