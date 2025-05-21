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
            // 시작에서 끝 지점으로
            yield return StartCoroutine(MoveTo(_endPoint.position));
            yield return new WaitForSeconds(_waitTime);

            // 끝에서 시작 지점으로
            yield return StartCoroutine(MoveTo(_startPoint.position));
            yield return new WaitForSeconds(_waitTime);
        }
    }

    // 원하는 방향으로 이동
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

    // 플레이어가 올라탈시, 같이 움직이게 하기 위한 부모 설정
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
