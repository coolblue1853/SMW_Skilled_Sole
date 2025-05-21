using UnityEngine;

public class ResourcesController : MonoBehaviour
{
    [Header("Stemina")]
    private PlayerStatHandler _statHandler;
    private PlayerController _playerController;
    [SerializeField] private float _restoreDelay = 2f; // 멈춘 뒤 회복까지 대기 시간
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

    // 달릴때의 스테미나 사용 함수
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

    // 스테미너 회복 함수
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
