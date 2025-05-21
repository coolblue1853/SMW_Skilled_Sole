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
