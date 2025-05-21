# 스파르타코딩클럽 10기_7 설민우 스파르타 던전 탐험 개인 프로젝트 입니다

# 스파르타 던전 탐험

스파르타 코딩클럽 10기, 유니티 숙련 개인 프로젝트 스파르타 던전 탐험 과제작업물입니다.

## 📷 스크린샷

![SoloMain](https://github.com/user-attachments/assets/6d2dde4d-55b8-4b9d-9919-0e4ea65fd58c)

## 빌드 파일 주소
https://drive.google.com/file/d/1aGOkOX911GhEabwecfvCFYxrNr2ctFqE/view?usp=sharing

## 🕹️ 기능
<details>
<summary><input type="checkbox" checked disabled> 1. (필수) 기본 이동 및 점프 </summary>

![Moving](https://github.com/user-attachments/assets/a1779a11-3bff-49aa-ba34-5017e79f2263)

![image](https://github.com/user-attachments/assets/57f05127-dcec-4096-963a-26396cfc654f)

- 유니티의 인풋시스템을 이용하여 기본적인 이동과 점프를 구현했습니다.
- 마리오에서의 점프처럼 스페이스바를 누르고 있는 시간을 통해 약점프, 강 점프를 구현했습니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 2. (필수) 체력바 UI </summary>

![Hp](https://github.com/user-attachments/assets/b245d1f3-16f6-47c0-95a5-30a88c347823)

```
using UnityEngine.UI;

public class UI_HpBar : UI_Scene
{
    private PlayerStatHandler _stathandler;

    enum Images
    {
        EmptyBar,
        HpBar,
    }
 
    public override void Init()
    {
        base.Init();
        _stathandler = PlayerManager.Instance.StatHandler;
        _stathandler.OnHealthChanged += UpdateHealthBar;
        _stathandler.OnMaxHealthChanged += UpdateMaxHealthBar;

        Bind<Image>(typeof(Images));
        UpdateHealthBar(_stathandler.Health);
        UpdateMaxHealthBar(_stathandler.MaxHealth);
    }


    void OnDisable()
    {
        _stathandler.OnHealthChanged -= UpdateHealthBar;
        _stathandler.OnMaxHealthChanged -= UpdateMaxHealthBar;
    }

    void UpdateHealthBar(float current)
    {
        var image = Get<Image>((int)Images.HpBar);
        if (image != null)
            image.fillAmount = (float)current / _stathandler.limitHealth;
    }
    void UpdateMaxHealthBar(float current)
    {
        var image = Get<Image>((int)Images.EmptyBar);
        if (image != null)
            image.fillAmount = (float)current / _stathandler.limitHealth;
    }
}

```
- 체력바는 이전처럼 StatHandler를 만들고 이를 옵버패턴을 이용해서 연결하여 UI에 자동으로 반영되도록 했습니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 3. (필수) 동적 환경 조사 </summary>

![dp](https://github.com/user-attachments/assets/72ae136f-c4ae-437a-9cf2-e18926245f2f)

```
using System;
using UnityEngine;
public class PlayerInteractController : MonoBehaviour
{
    public event Action<ItemObject> OnInteractionChanged;
    public event Action<ItemData> OnAddItem;

    [Header("Info")]
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

```
- 동적 환경조사의 경우, 강의와는 다르게 3인칭 시점에서 움직이기 때문에 플레이어가 바라보는 카메라의 방향을 전면으로 했습니다.
- 이를 기준으로 BoxCast를 통해 앞에 존재하는 충돌체를 감지해 UI에 보여주도록 옵저버 패턴을 통해 작업했습니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 4. (필수) 점프대 </summary>

![jumper](https://github.com/user-attachments/assets/cfbd42f6-8615-4c06-8ce8-13eae5dd8944)


```
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private float _jumpForce;
    private float _yPivot;
    private float _extra = 0.1f;

    private void Start()
    {
        _yPivot = transform.position.y + transform.localScale.y / 2;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            float playerPivot = collision.transform.position.y + collision.transform.localScale.y / 2;
            if (playerPivot + _extra > _yPivot)
            {
                var rb = collision.transform.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
        }
    }
}

```
- 점프대는 점프의 기능을 OnCollison했을때 강제로 부여하는 형식으로 구현했습니다.
- 대신 점프대의 옆면에 부딪혔을 때를 예외로 해주기 위해서 점프대 윗면의 높이보다 충돌체(플레이어)의 위치가 높았을때에만 점프하도록 했습니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 5. (필수) 아이템 데이터 </summary>

![image](https://github.com/user-attachments/assets/47592f0d-2f85-4b47-a3c8-cb2b549d5bef)

```
using UnityEngine;

public enum ItemType
{
    Resource,
    Equipable,
    Consumable
}
public enum ConsumableType
{
    Health,
}
public enum BuffType
{
    Speed,
}
[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType Type;
    public float Value;
}
[System.Serializable]
public class ItemDataBuff
{
    public BuffType Type;
    public float Time;
    public float Value;
}
[System.Serializable]
public class ItemDataEquip
{
    public BuffType Type;
    public float Value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string DisplayName;
    public string Descrition;
    public ItemType Type;
    public Sprite Icon;
    public GameObject DropPrefab;

    [Header("Stacking")]
    public int MaxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header("Buff")]
    public ItemDataBuff[] buffs;

    [Header("Equip")]
    public ItemDataEquip[] equips;
}

```

- 강의 내용과 거의 일치하게 스크립터블 오브젝트를 이용해서 아이템 데이터를 구성했습니다.
  

</details>

<details>
<summary><input type="checkbox" checked disabled> 6. (필수) 아이템 사용 </summary>

![Item](https://github.com/user-attachments/assets/831eb3be-1d4e-4d44-b64b-1ddcfc0749f3)

```
void ConsumItem()
 {
     if(_curItemData != null && _curItemData.Type == ItemType.Consumable)
     {
         var slot = itemSlots[_curIndex];
         var cunsumData = slot.Item.consumables;
         var buffData = slot.Item.buffs;

         foreach(var value in cunsumData)
         {
             switch (value.Type)
             {
                 case ConsumableType.Health:
                     _statHandler.Health += value.Value;
                     break;
             }
         }
         foreach (var value in buffData)
         {
             switch (value.Type)
             {
                 case BuffType.Speed:
                     _buffs.ApplyBuff(value.Type, value.Value, value.Time);
                     break;
             }
         }

         if (slot.Stack == 1)
         {
             slot.ResetSlot();
             ResetDetail();
         }
         else
         {
             slot.Stack -= 1;
             slot.UpdateTMP();
         }
     }
 }
```
- 인벤토리의 경우 나름 중요하다고 생각해 강의를 참고하지 않고 직접 제작했습니다.
- 최대한 의존성을 줄이기 위해서 노력하고 예외처리에 신경을 썼습니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 1. (도전) 추가 UI </summary>

![Stemina](https://github.com/user-attachments/assets/facb2ea6-5274-42aa-aa41-ab53597ccd07)

```
 using UnityEngine.UI;
public class UI_Stemina : UI_Scene
{
    private PlayerStatHandler _stathandler;
    enum Images
    {
        EmptyBar,
        SteminaBar,
    }

    public override void Init()
    {
        base.Init();
        _stathandler = PlayerManager.Instance.StatHandler;
        _stathandler.OnSteminaChanged += UpdateSteminaBar;
        _stathandler.OnMaxSteminaChanged += UpdateMaxSteminaBar;

        Bind<Image>(typeof(Images));
        UpdateSteminaBar(_stathandler.Stemina);
        UpdateMaxSteminaBar(_stathandler.MaxStemina);
    }

    void OnDisable()
    {
        _stathandler.OnSteminaChanged -= UpdateSteminaBar;
        _stathandler.OnMaxSteminaChanged -= UpdateMaxSteminaBar;
    }

    void UpdateSteminaBar(float current)
    {
        var image = Get<Image>((int)Images.SteminaBar);
        if (image != null)
            image.fillAmount = (float)current / _stathandler.limitStemina;
    }
    void UpdateMaxSteminaBar(float current)
    {
        var image = Get<Image>((int)Images.EmptyBar);
        if (image != null)
            image.fillAmount = (float)current / _stathandler.limitStemina;
    }

}

```

- 달리기를 사용하면 스태미너가 감소하고, 스태미너가 0이되면 달리기가 자동으로 멈추도록 설정했습니다.
- 이 반영은 옵저버 패턴을 이용하여 구현했습니다.

</details>

</details>

<details>
<summary><input type="checkbox" checked disabled> 2. (도전) 3인칭 시점 </summary>

![Camera](https://github.com/user-attachments/assets/75cbdc0c-3b8b-4b0e-9f42-b37a0a900e06)

```
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _player;

    [Header("Settings")]
    [SerializeField] private float _zoomSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private float _minZoom = 2f;
    [SerializeField] private float _maxZoom = 2f;

    private float _zoomDistance = 10f;
    private float _yaw = 0f; // 좌우 회전각
    private float _pitch = 20f; // 상하 회전각

    // 카메라 각 입력
    private Vector2 _lookInput = Vector2.zero;
    private bool _isRightMousePressed = false;

    // 3인칭 각도 조절
    public void OnLook(InputValue input)
    {
        if (_isRightMousePressed)
        {
            _lookInput = input.Get<Vector2>();

            _yaw += _lookInput.x * _rotationSpeed * Time.deltaTime;
            _pitch -= _lookInput.y * _rotationSpeed * Time.deltaTime;
            _pitch = Mathf.Clamp(_pitch, 5f, 80f); // 각도 제한
        }
    }

    // 3인칭 줌 기능
    public void OnZoom(InputValue input)
    {
        float scroll = input.Get<float>();
        scroll /= 120f; // 축 값 Nomalize
        _zoomDistance -= scroll * _zoomSpeed; // 마우스 입력이 반대이기 때문에 - 로
        _zoomDistance = Mathf.Clamp(_zoomDistance, _minZoom, _maxZoom);

    }   

    // 3인칭 각도 조절을 위한 인풋
    public void OnRightClick(InputValue input)
    {
        _isRightMousePressed = input.isPressed;
    }
        
    private void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
        Vector3 cameraOffset = rotation * new Vector3(0, 0, -_zoomDistance);

        transform.position = _player.position + cameraOffset;
        transform.LookAt(_player.position);
    }

}

```
- WoW 카메라 기능을 구현해보기 위해 휠을 통해 앞,뒤로 땡겨오고, 마우스 오른쪽 버튼을 누르고 시점을 조정하는 기능을 추가했습니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 3. (도전) 움직이는 플랫폼 구현 </summary>

![MovingPlatform](https://github.com/user-attachments/assets/37589a03-738f-46cd-9688-a3e774d9d505)

```
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

```
- 움직이는 플랫폼을 만들되, 위에 그냥 플레이어가 올라가면 떨어져버리는 문제가 있었습니다
- 플랫폼 위에 올라서면 플레이어가 플랫폼의 자식 오브젝트로 들어가도록 했습니다
- 문제!! << 플랫폼 자체에 올리니, 플랫폼의 스케일에 따라 플레이어의 스케일이 영향을 받는 문제가 있었습니다.
- 콜라이더와 메쉬를 분리하여 해당 문제를 해결했습니다(부모가 될 녀석은 무조건 1,1,1 스케일로)

</details>

<details>
<summary><input type="checkbox" checked disabled> 4. (도전) 벽 타기 및 매달리기 </summary>

![ezgif-63d1543f15a4ff](https://github.com/user-attachments/assets/27f94e9e-39d0-4e94-bc49-d1433f7cd631)

```
  public void OnJump(InputValue input)
  {
      if (_isClimbing && input.isPressed)
      {
          _isOnLadder = false;
          _isClimbing = false;
          _rigidbody.useGravity = true;

          //벽점프
          if (_isWallJumpalbe)
          {
              _isWallJumpalbe = false;
              SetAvailableMove(false);
              Vector3 jumpDir = (-_ladderForward.normalized + Vector3.up).normalized;
              _rigidbody.velocity = Vector3.zero; // 기존 속도 초기화
              _rigidbody.AddForce(jumpDir * wallJumpForce, ForceMode.Impulse);
              StartCoroutine(EnableMoveAfterDelay(0.5f));
          }
 
          return;
      }

      if (input.isPressed && _availableMove)
      {
          if (_groundChecker.CheckGrounded(_groundLayerMask,_groundPivot))
          {
              _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z); // Y속도 초기화
              _rigidbody.AddForce(Vector3.up * _initialJumpForce, ForceMode.Impulse);
              _isJumping = true;
              _jumpTimer = 0f;
          }
      }
      else
      {
          _isJumping = false;
      }
  }
```
- 사다리 타기 기능 및 마리오식 벽점프 기능을 추가했습니다.
- 사다리 타기는 사다리에 닿으면 입력값을 y축으로 넣도록 했습니다.
- 벽 점프는 벽의 반대방향으로 대각선 윗 방향으로 리지드바디 임펄스로 힘을 가합니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 5. (도전) 다양한 아이템 구현 </summary>

![image](https://github.com/user-attachments/assets/368eb7ec-4904-450d-b112-51971c65a842)

![image](https://github.com/user-attachments/assets/b308750b-d2a8-4c36-9861-c21ca23ebf04)


- 섭취 가능 아이템을 생성하고, 채력 증가, 이동속도 증가등의 아이템을 추가했습니다.
- 아이템 데이터는 스크립터블 오브젝트로 구현했습니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 6. (도전) 장비 장착 </summary>

![ezgif-6a69d376d81fe5](https://github.com/user-attachments/assets/03a7abeb-1f3b-48e7-8b33-690833a54377)

```
    void EquipItem()
    {
        if (_curItemData != null && _curItemData.Type == ItemType.Equipable)
        {
            var slot = itemSlots[_curIndex];
            var equipData = slot.Item.equips;

            foreach (var value in equipData)
            {
                switch (value.Type)
                {
                    case BuffType.Speed:
                        _statHandler.AddSpeedModifier(value.Value);
                        break;
                }
            }
            slot.UpdateEquiped(true);
            slot.isEquiped = !slot.isEquiped;
            _unequipBtn.gameObject.SetActive(true);
            _equipBtn.gameObject.SetActive(false);
        }
    }
    void UnequipItem()
    {
        if (_curItemData != null && _curItemData.Type == ItemType.Equipable)
        {
            var slot = itemSlots[_curIndex];
            var equipData = slot.Item.equips;

            foreach (var value in equipData)
            {
                switch (value.Type)
                {
                    case BuffType.Speed:
                        _statHandler.RemoveSpeedModifier(value.Value);
                        break;
                }
            }
            slot.UpdateEquiped(false);
            slot.isEquiped = !slot.isEquiped;
            _unequipBtn.gameObject.SetActive(false);
            _equipBtn.gameObject.SetActive(true);
        }
    }
```
- 섭취 아이템과 마찬가지로 장착 할 수 있는 아이템도 생성했습니다.
- 장착시 E 표시가 나옵니다.
- 지금은 이동속도 증가만 가능합니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 7. (도전) 레이저 트랩 </summary>

![Lazor](https://github.com/user-attachments/assets/43055838-4bcc-4210-857b-0cf95ca0c08f)

```
using UnityEngine;

public class Obstruction : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] float _damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {

           var _statHandler = other.GetComponent<PlayerStatHandler>();
            _statHandler.Health -= _damage;
    
        }
    }
}

```
- 평소에는 가만히 있다가 Ray에 닿으면 이동하는 레이저 트랩을 생성했습니다.
- 닿게 되면 체력이 감소합니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 8. (도전) 상호작용 가능한 오브젝트 표시 </summary>

![Interact](https://github.com/user-attachments/assets/383fada0-5edb-4afb-bac1-77ad4f1e3374)


```
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hover : UI_Scene
{
    private Camera mainCamera;
    [SerializeField] private GameObject hoverUI;
    private TextMeshProUGUI hoverTxt;
    [SerializeField] private LayerMask interactableLayer;
    enum Texts
    {
        Txt,
    }

    public override void Init()
    {
        hoverUI.SetActive(true);

        Bind<TextMeshProUGUI>(typeof(Texts));
        hoverTxt = Get<TextMeshProUGUI>((int)Texts.Txt);
        mainCamera = Camera.main;

        hoverUI.SetActive(false);
    }

    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
        {
            // 마우스 근처에 UI 표시
            hoverUI.SetActive(true);
            hit.transform.TryGetComponent<ItemObject>(out ItemObject item);
            hoverTxt.text = item.Data.DisplayName;

            Vector2 screenPos = Input.mousePosition;
            hoverUI.transform.position = screenPos + new Vector2(20f, -20f); // 마우스 옆에 띄움
        }
        else
        {
            hoverUI.SetActive(false);
        }
    }
}
```
- 상호작용 가능한 물체에 마우스가 가까이 가면 UI가 출력됩니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 9. (도전) 플랫폼 발사기 </summary>

![Shotter](https://github.com/user-attachments/assets/19ed1011-c2a7-439b-85a2-46dad21739fc)


```
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private float _jumpForce;
    private float _yPivot;
    private float _extra = 0.1f;

    private void Start()
    {
        _yPivot = transform.position.y + transform.localScale.y / 2;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            float playerPivot = collision.transform.position.y + collision.transform.localScale.y / 2;
            if (playerPivot + _extra > _yPivot)
            {
                var rb = collision.transform.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
        }
    }
}

```
- 360 도 돌아가며 그 방향으로 플레이어를 발사시켜주는 마리오 대포식 장치입니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 10. (도전) 발전된 AI </summary>

![image](https://github.com/user-attachments/assets/f81ea347-f9aa-4359-8e51-73caf822e21e)

![Ai](https://github.com/user-attachments/assets/4156a49f-108f-49db-94b4-4936ec5f1b55)

```
using System.Collections;
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

```
- 변경된 네비메쉬를 사용했습니다.
- 가중치를 설정해 아랫쪽 다리를 통했을때는 윗쪽 다리를 이용하는 것이 더 빠르다고 판단합니다.
- 네비링크를 이용해서 절벽에서도 떨어져서 추격합니다.

</details>

## 🛠️ 기술 스택

- C#
- .NET Core 3.1
- Unity 22.3.17f1

## 🧙 사용법

1. 이 저장소를 클론하거나 다운로드합니다.
2. 빌드를 진행하여 실행합니다.
3. 방향키 혹은 WASD 로 이동하고, 마우스로 와우처럼 시점 조절이 가능합니다.
4. 쉬프트키로 달릴 수 있고 마우스 오른쪽 키를 누른 상태로 마우스를 돌리면 시점 조절이 가능합니다.
5. I 키로 인벤토리 창을 열고 닫을 수 있습니다.
   
## 🗂️ 프로젝트 구조
<details>
<summary><input type="checkbox" checked disabled> 펼쳐보기 </summary>

```
├── Camera
│ ├── CameraAspectFixer.cs
│ └── CameraController.cs
│
├── Enemy
│ └── TestEnemy.cs
│
├── Item
│ ├── ItemData.cs
│ ├── ItemObject.cs
│ └── ItemSlot.cs
│
├── Manager
│ └── UIManager.cs
│
├── Map
│ ├── JumpPlatform.cs
│ ├── LaunchPlatform.cs
│ ├── MovePlatform.cs
│ ├── Obstruction.cs
│ └── PlayerChecker.cs
│
├── Player
│ ├── BuffController.cs
│ ├── GroundChecker.cs
│ ├── PlayerController.cs
│ ├── PlayerInteractController.cs
│ ├── PlayerManager.cs
│ ├── PlayerStatHandler.cs
│ └── ResourcesController.cs
│
├── UI
│ ├── Popup
│ │ ├── UI_Inventory.cs
│ │ └── UI_Popup.cs
│ │
│ ├── Scene
│ │ ├── UI_Hover.cs
│ │ ├── UI_HpBar.cs
│ │ ├── UI_Interaction.cs
│ │ ├── UI_Scene.cs
│ │ └── UI_Stamina.cs
│ │
│ └── UI_Base.cs
│
├── Utils
│ ├── Define.cs
│ └── Utils.cs
```
</details>


## 🙋 개발자 정보

- 이름: SulMinWoo
- 연락처 : sataka1853@naver.com
