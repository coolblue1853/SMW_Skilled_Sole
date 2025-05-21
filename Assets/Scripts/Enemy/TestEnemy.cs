using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemy :MonoBehaviour
{
    [Header("AI")]
    private NavMeshAgent _agent;
    private Transform _player;
    private PlayerStatHandler _statHandler;
    [SerializeField] private float _updateRate = 0.2f; // 네비메쉬 갱신

    [Header("Attack")]
    [SerializeField] private float _detectionRadius = 5f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _damge = 10f;
    [SerializeField] private LayerMask _targetLayer;     

    private float attackTimer = 0f;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = PlayerManager.Instance.transform;
        _statHandler = _player.GetComponent<PlayerStatHandler>();
    }
    private void Start()
    {
        StartCoroutine(UpdateDestination());
    }
    void Update()
    {
        // 공격 코드
        attackTimer -= Time.deltaTime;

        Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRadius, _targetLayer);

        if (hits.Length > 0)
        {

            if (attackTimer <= 0f)
            {
                _statHandler.Health -= _damge;
                attackTimer = _attackCooldown;
            }
        }
    }

    // 경로 재탐색 코루틴
    IEnumerator UpdateDestination()
    {
        while (true)
        {
            if (_player != null)
                _agent.SetDestination(_player.position);

            yield return new WaitForSeconds(_updateRate);
        }
    }

    void OnDrawGizmosSelected()
    {
        // 에디터에서 감지 범위 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
