using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public UI_Inventory uI_Inventory;
    public ItemData Item = null;
    public int Stack = 0;
    public int Idx = -1;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _tmp;
    private Color _alpha0 = new Color(255,255,255,0);
    private Color _alpha255 = new Color(255,255,255,255);

    public void UpdateIcon(Sprite icon)
    {
        _icon.sprite = icon;
        _icon.color = _alpha255;
    }
    public void UpdateTMP()
    {
        if(Stack == 1)
        {
            _tmp.text = "";
        }
        else
        {
            _tmp.text = Stack.ToString();
        }
    }

    public void ResetSlot()
    {
        Item = null;
        Stack = 0;
        _icon.sprite = null;
        _icon.color = _alpha0;
        _tmp.text = "";
    }

    public void ShowUI()
    {
        if(Item!=null)
            uI_Inventory.ShowDetail(Idx);
    }
}
