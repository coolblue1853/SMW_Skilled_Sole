using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemy :MonoBehaviour
{
    private NavMeshAgent _agent;
    private Transform _player;


    public float updateRate = 0.5f; // 경로 갱신 주기 (초 단위)

    private void Update()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = PlayerManager.Instance.transform;

        _agent.SetDestination(_player.position);
    }
}
