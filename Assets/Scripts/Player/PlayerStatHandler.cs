using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatHandler : MonoBehaviour
{
    // �ɷ�ġ ���� �̺�Ʈ
    public event Action<float> OnHealthChanged;
    public event Action<float> OnMaxHealthChanged;

    [SerializeField] private float _invincibleDuration = 1.0f; // ���� �ð� (��)
    private bool _isInvincible = false;
    [Header("<Health>")]
    // ü��, ���� ResuorceManager�� ����?
    public float limitHealth = 10;
    [SerializeField] private float _health;
    public float Health
    {
        get => _health;
        set
        {
            if (value < 0 && _isInvincible) return; // ���� ���¸� ������ ����

            _health = Mathf.Clamp(value, 0, MaxHealth);
            OnHealthChanged?.Invoke(_health);

            // �������� ���� ��� ���� �ð� ����
            if (value < 0)
            {
                StartCoroutine(InvincibleCoroutine());
            }

            if (_health <= 0)
            {
                // ���
                Debug.Log("���!");
            }
        }
    }

    private IEnumerator InvincibleCoroutine()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_invincibleDuration);
        _isInvincible = false;
    }
    [SerializeField] private float _maxHealth;
    public float MaxHealth
    {
        get => _maxHealth;
        set
        {
            float newHealth = Mathf.Min(value, limitHealth);

            if (_maxHealth != newHealth)
            {
                _maxHealth = newHealth;
                OnMaxHealthChanged?.Invoke(_maxHealth);
            }
        }
    }

}
