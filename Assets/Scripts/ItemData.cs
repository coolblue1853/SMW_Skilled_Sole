using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Resource,
    Equipable,
    Consumable
}

public enum ConsumableType
{
    Health
}

[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType Type;
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
    public bool CanStack;
    public int MaxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;
}
