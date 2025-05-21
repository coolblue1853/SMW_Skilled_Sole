using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public string Id { get; }
    public float Duration { get; }
    public float Value { get; }
    public Action OnExpire { get; }
    private float _timeRemaining;

    public Buff(float duration, float value, Action onExpire)
    {
        Id = Guid.NewGuid().ToString();
        Duration = duration;
        Value = value;
        _timeRemaining = duration;
        OnExpire = onExpire;
    }
    public bool CheckEndBuff(float time)
    {
        _timeRemaining -= time;
        if (_timeRemaining <= 0f)
        {
            OnExpire?.Invoke();
            return true;
        }
        return false;
    }

}

public class BuffController : MonoBehaviour
{
    private List<Buff> _activeBuffs = new List<Buff>();
    private PlayerStatHandler _statHandler;
    private void Awake()
    {
        _statHandler = GetComponent<PlayerStatHandler>();
    }

    // 버프 시간 체크
    private void Update()
    {
        float deltaTime = Time.deltaTime;
        for (int i = 0; i < _activeBuffs.Count; i++)
        {
            if (_activeBuffs[i].CheckEndBuff(deltaTime))
            {
                _activeBuffs.RemoveAt(i);
            }
        }
    }

    // 새로운 버프 적용
    public  void ApplyBuff(BuffType type, float value, float duration)
    {
        switch (type)
        {
            case BuffType.Speed:
                _statHandler.AddSpeedModifier(value);
                Buff buff = new Buff(duration, value,
                    () => { _statHandler.RemoveSpeedModifier(value); });
                _activeBuffs.Add(buff);
                break;
        }
    }
}
