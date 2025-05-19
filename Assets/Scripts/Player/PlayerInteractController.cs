using System;
using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    public event Action<ItemObject> OnInteractionChanged;
    public event Action<ItemData> OnAddItem;

    [SerializeField] private float _checkRate = 0.05f;
    private float _lastCheckTime;
    [SerializeField] private float _maxCheckDistance;
    [SerializeField] private LayerMask _layerMask;

    public GameObject curInteractGameObject;
    private ItemObject curItem;
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        Interaction();
    }

    void Interaction()
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
                    curItem = hit.collider.GetComponent<ItemObject>();
                    OnInteractionChanged?.Invoke(curItem);
                }
            }
            else
            {
                if (curInteractGameObject != null)
                {
                    curInteractGameObject = null;
                    curItem = null;
                    OnInteractionChanged?.Invoke(null);
                }
            }
        }
    }

    public void OnInteract()
    {
        if(curItem != null)
            OnAddItem.Invoke(curItem.Data);
    }
}
