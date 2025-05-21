using System;
using System.Collections;
using UnityEngine;

public class PlayerStatHandler : MonoBehaviour
{
    // �ɷ�ġ ���� �̺�Ʈ
    public event Action<float> OnHealthChanged;
    public event Action<float> OnMaxHealthChanged;
    public event Action<float> OnSteminaChanged;
    public event Action<float> OnMaxSteminaChanged;


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

     // ü��, ���� ResuorceManager�� ����?
    public float limitStemina = 10;
    [SerializeField] private float _stemina;
    public float Stemina
    {
        get => _stemina;
        set
        {
            _stemina = Mathf.Clamp(value, 0, MaxStemina);
            OnSteminaChanged?.Invoke(_stemina);
        }
    }
    [SerializeField] private float _maxStemina;
    public float MaxStemina
    {
        get => _maxStemina;
        set
        {
            float newStemina = Mathf.Min(value, limitStemina);

            if (_maxStemina != newStemina)
            {
                _maxStemina = newStemina;
                OnMaxSteminaChanged?.Invoke(_maxStemina);
            }
        }
    }

    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    private float _addSpeed;
    public float WalkSpeed
    {
        get => _walkSpeed + _addSpeed;
        set => _walkSpeed = value;
    }
    public float RunSpeed
    {
        get => _runSpeed + _addSpeed;
        set => _runSpeed = value;
    }
    public void AddSpeedModifier(float value) => _addSpeed += value;    
    public void RemoveSpeedModifier(float value) => _addSpeed -= value;    
}
