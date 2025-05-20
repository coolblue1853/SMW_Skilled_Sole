using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class PlayerChecker : MonoBehaviour
{
    [SerializeField] private Transform _pivot;
    [SerializeField] private Behaviour _activateComponent;
    [SerializeField] private GameObject[] _objects;

    [SerializeField] private Vector3 _direction = Vector3.forward;
    [SerializeField] private float _length = 5f;
    private Color _rayColor = Color.red;

    private void Start()
    {
        StartCoroutine(DrawRayRepeatedly());
    }

    private IEnumerator DrawRayRepeatedly()
    {
        while (true)
        {
            DrawRay(_pivot.position, _direction, _length, _rayColor);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void DrawRay(Vector3 origin, Vector3 direction, float length, Color color)
    {
        RaycastHit hit;
        Vector3 normalizedDir = direction.normalized;
        Vector3 endPoint = origin + normalizedDir * length;

        if (Physics.Raycast(origin, normalizedDir, out hit, length))
        {
            if (hit.transform.CompareTag("Player"))
            {
                _activateComponent.enabled = true;
                foreach(var value in _objects)
                {
                    value.SetActive(true);
                }
                Destroy(this);
            }
        }
        else
        {
            Debug.DrawLine(origin, origin + normalizedDir * length, _rayColor, 0.5f);
        }
    }
}
