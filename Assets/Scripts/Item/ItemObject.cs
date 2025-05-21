using UnityEngine;

public interface IInteractable
{

}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData Data;

}