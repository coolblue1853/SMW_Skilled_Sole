using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [SerializeField] private float _jumpForce;
    private float _yPivot;
    private float _extra = 0.1f;

    private void Start()
    {
        _yPivot = transform.position.y + transform.localScale.y / 2;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            float playerPivot = collision.transform.position.y + collision.transform.localScale.y / 2;
            if (playerPivot + _extra > _yPivot)
            {
                var rb = collision.transform.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }

        }
    }

}
