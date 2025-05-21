using System.Collections;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    private GameObject _platform;

    [SerializeField] private float _waitTime = 1.0f;
    [SerializeField] private float _moveSpeed = 2.0f;

    private float _yPivot;
    private float _extra = 0.1f;

    private void Start()
    {
        _platform = transform.gameObject;
        _platform.transform.position = _startPoint.position;
        _yPivot = transform.position.y + _platform.transform.localScale.y / 2;


        StartCoroutine(MoveRoutine());
    }
    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            // ���ۿ��� �� ��������
            yield return StartCoroutine(MoveTo(_endPoint.position));
            yield return new WaitForSeconds(_waitTime);

            // ������ ���� ��������
            yield return StartCoroutine(MoveTo(_startPoint.position));
            yield return new WaitForSeconds(_waitTime);
        }
    }

    // ���ϴ� �������� �̵�
    private IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(_platform.transform.position, target) > 0.01f)
        {
            _platform.transform.position =
                Vector3.MoveTowards(_platform.transform.position,target,_moveSpeed * Time.deltaTime);
            yield return null;
        }

        _platform.transform.position = target;
    }

    // �÷��̾ �ö�Ż��, ���� �����̰� �ϱ� ���� �θ� ����
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            float playerPivot = collision.transform.position.y + collision.transform.localScale.y / 2;
            if (playerPivot + _extra > _yPivot)
            {
                collision.transform.SetParent(_platform.transform, true);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

}
