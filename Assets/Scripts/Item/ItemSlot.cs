using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
     public UI_Inventory uI_Inventory;

    [Header("ItemData")]
    public ItemData Item = null;
    public int Stack = 0;
    public int Idx = -1;

    [Header("UI")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _stackTxt;
    [SerializeField] private GameObject _equiped;

    //����
    private Color _alpha0 = new Color(255,255,255,0);
    private Color _alpha255 = new Color(255,255,255,255);

    // ������ ������Ʈ
    public void UpdateIcon(Sprite icon)
    {
        _icon.sprite = icon;
        _icon.color = _alpha255;
    }

    // �ؽ�Ʈ ������Ʈ
    public void UpdateTMP()
    {
        if(Stack == 1)
        {
            _stackTxt.text = "";
        }
        else
        {
            _stackTxt.text = Stack.ToString();
        }
    }

    // �������� ������Ʈ
    public void UpdateEquiped(bool isEquiped)
    {
        if (isEquiped)
            _equiped.SetActive(true);
        else
            _equiped.SetActive(false);
    }

    // UI �ʱ�ȭ �Լ�
    public void ResetSlot()
    {
        Item = null;
        Stack = 0;
        _icon.sprite = null;
        _icon.color = _alpha0;
        _stackTxt.text = "";
        _equiped.SetActive(false);
    }

    // UI ��� �ռ�
    public void ShowUI()
    {
        if(Item!=null)
            uI_Inventory.ShowDetail(Idx);
    }
}
