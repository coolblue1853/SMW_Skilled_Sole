using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class UI_Inventory : UI_Scene
{
    private PlayerInteractController _interaction;
    private PlayerStatHandler _statHandler;
    private BuffController _buffs;

    [SerializeField] private GameObject _inventory;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private int _slotCount;
    private List<ItemSlot> itemSlots = new List<ItemSlot>();

    private GameObject _slots;

    //Detail 파트
    private ItemData _curItemData;
    private int _curIndex;

    private TextMeshProUGUI _detailNameTxt;
    private TextMeshProUGUI _detailDescritionTxt;
    private Button _consumBtn;
    private Button _equipBtn;
    private Button _unequipBtn;


    enum Grid
    {
        ItemSlots,
    }
    enum TMPs
    {
        Name,
        Description
    }
    enum Buttons
    {
        UseConsumable,
        Equip,
        Unequip,
    }

    public override void Init()
    {
        _inventory.SetActive(true); // 초기화 시작
        base.Init();
        var player = PlayerManager.Instance;
        _interaction = player.InteractController;
        _statHandler = player.StatHandler;
        _buffs = player.BuffController;
        _interaction.OnAddItem += AddItem;

        Bind<GridLayoutGroup>(typeof(Grid));
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Button>(typeof(Buttons));

        _slots = Get<GridLayoutGroup>((int)Grid.ItemSlots).gameObject;
        _detailNameTxt = Get<TextMeshProUGUI>((int)TMPs.Name);
        _detailDescritionTxt = Get<TextMeshProUGUI>((int)TMPs.Description);
        _consumBtn = Get<Button>((int)Buttons.UseConsumable);
        _equipBtn = Get<Button>((int)Buttons.Equip);
        _unequipBtn = Get<Button>((int)Buttons.Unequip);


        //함수 연결
        _consumBtn.onClick.AddListener(ConsumItem);
        _equipBtn.onClick.AddListener(EquipItem);
        _unequipBtn.onClick.AddListener(UnequipItem);

        //초기화 작업
        CloseAllBtn();

        for (int i = 0; i < _slotCount; i++)
        {
            var go = Instantiate(_slotPrefab);
            itemSlots.Add(go.GetComponent<ItemSlot>());
            itemSlots[i].uI_Inventory = this;
            itemSlots[i].Idx = i;
            go.transform.SetParent(_slots.transform,false);
        }
        _inventory.SetActive(false); // 초기화 종료
    }
    private void OnDisable()
    {
        _interaction.OnAddItem -= AddItem;
    }

    // stack과 빈칸을 확인해서 생성 가능한지 확인하는 함수.
    void AddItem(ItemData data)
    {
        for(int i = 0; i < _slotCount; i++)
        {
            var curSlot = itemSlots[i];
            //스택부터 체크, null 이 아니고, 이름이 같고, stack 이 채울수 있을때.
            if (curSlot.Item != null && curSlot.Item.DisplayName == data.DisplayName && curSlot.Stack < data.MaxStackAmount)
            {
                curSlot.Stack += 1;
                curSlot.UpdateTMP();
                return;
            }
        }
        for (int i = 0; i < _slotCount; i++)
        {
            var curSlot = itemSlots[i];
            // 빈칸체크
            if (curSlot.Item == null)
            {
                curSlot.Item = data;
                curSlot.Stack = 1;
                curSlot.UpdateIcon(data.Icon);
                curSlot.UpdateTMP();
                return;
            }
        }
    }

    public void ShowDetail(int index)
    {
        _curIndex = index;
        _curItemData = itemSlots[_curIndex].Item;
        _detailNameTxt.text = _curItemData.DisplayName; 
        _detailDescritionTxt.text = _curItemData.Descrition;

        CloseAllBtn();

        switch (_curItemData.Type)
        {
            case ItemType.Consumable:
                _consumBtn.gameObject.SetActive(true);
                break;
            case ItemType.Equipable:
                if (_curItemData.isEquiped)
                {
                    _unequipBtn.gameObject.SetActive(true);
                    _equipBtn.gameObject.SetActive(false);
                }
                else
                {
                    _unequipBtn.gameObject.SetActive(false);
                    _equipBtn.gameObject.SetActive(true);
                }
                break;
        }
    }



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

    // 장착 해제의 중복 구조 해결할 필요성 있음
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
            _curItemData.isEquiped = !_curItemData.isEquiped;
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
            _curItemData.isEquiped = !_curItemData.isEquiped;
            _unequipBtn.gameObject.SetActive(false);
            _equipBtn.gameObject.SetActive(true);
        }
    }


    void ResetDetail()
    {
        _detailNameTxt.text = "";
        _detailDescritionTxt.text = "";
        CloseAllBtn();
    }
    void CloseAllBtn()
    {
        _consumBtn.gameObject.SetActive(false);
        _equipBtn.gameObject.SetActive(false);
        _unequipBtn.gameObject.SetActive(false);
    }

    public void OnInventory()
    {
       if(_inventory.activeSelf == true)
            _inventory.SetActive(false);
       else
            _inventory.SetActive(true);
    }
}
