using UnityEngine;

public class ResourcesController : MonoBehaviour
{
    [Header("Stemina")]
    private PlayerStatHandler _statHandler;
    private PlayerController _playerController;
    [SerializeField] private float _restoreDelay = 2f; // ���� �� ȸ������ ��� �ð�
    [SerializeField] private float _useSteminaInterval = 0.1f;
    public float RunUseStemina = 0.1f;
    [SerializeField] private float _restoreStemina = 0.3f;

    private float _steminaTimer = 0f;
    private float _restoreDelayTimer = 0f;
    private float _restoreIntervalTimer = 0f;

    private void Awake()
    {
        _statHandler = GetComponent<PlayerStatHandler>();
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        HandleSteminaDrain();
        HandleSteminaRestore();
    }

    // �޸����� ���׹̳� ��� �Լ�
    private void HandleSteminaDrain()
    {
        if (_playerController.IsRunning)
        {
            _steminaTimer += Time.deltaTime;

            if (_steminaTimer >= _useSteminaInterval)
            {
                _steminaTimer = 0f;
                _statHandler.Stemina -= RunUseStemina;
            }
        }
        else
        {
            _steminaTimer = 0f;
        }
    }

    // ���׹̳� ȸ�� �Լ�
    private void HandleSteminaRestore()
    {
        if (_playerController.IsRunning)
        {
            _restoreDelayTimer = 0f;
            _restoreIntervalTimer = 0f;
            return;
        }

        if (_statHandler.MaxStemina > _statHandler.Stemina)
        {
            _restoreDelayTimer += Time.deltaTime;

            if (_restoreDelayTimer >= _restoreDelay)
            {
                _restoreIntervalTimer += Time.deltaTime;

                if (_restoreIntervalTimer >= _useSteminaInterval)
                {
                    _restoreIntervalTimer = 0f;
                    _statHandler.Stemina += _restoreStemina;
                }
            }
        }

    }
}
