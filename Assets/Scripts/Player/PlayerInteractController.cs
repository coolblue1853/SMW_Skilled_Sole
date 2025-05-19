using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerInteractController : MonoBehaviour
{
    public event Action<IInteractable> OnInteractionChanged;

    [SerializeField] private float _checkRate = 0.05f;
    private float _lastCheckTime;
    [SerializeField] private float _maxCheckDistance;
    [SerializeField] private LayerMask _layerMask;

    public GameObject curInteractGameObject;
    private IInteractable curInteractable;
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Time.time - _lastCheckTime > _checkRate)
        {
            _lastCheckTime = Time.time;

            if (_camera == null)
                _camera = Camera.main;

            Vector3 flatForward = _camera.transform.forward;
            flatForward.y = 0f;
            flatForward.Normalize();

            Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, _maxCheckDistance / 2f);
            Quaternion rotation = Quaternion.LookRotation(flatForward);

            Vector3 origin = transform.position - flatForward * (_maxCheckDistance / 2f);

            if (Physics.BoxCast(origin, boxHalfExtents, flatForward,
                out RaycastHit hit, rotation, _maxCheckDistance, _layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    // OnInteractionChanged?.Invoke(curInteractable);
                }
            }
            else
            {
                curInteractGameObject = null;
                curInteractable = null;
                // OnInteractionChanged?.Invoke(null);
            }
        }
    }



    // 씬창에서의 확인 하기 위한 Gizmo
    void OnDrawGizmos()
    {
        Vector3 flatForward = _camera.transform.forward;
        flatForward.y = 0f;
        flatForward.Normalize();

        Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, _maxCheckDistance / 2f);
        Vector3 center = transform.position + flatForward * _maxCheckDistance * 0.5f;
        Quaternion rotation = Quaternion.LookRotation(flatForward);

        Gizmos.color = Color.cyan;
        Matrix4x4 matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.matrix = matrix;
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }

}
