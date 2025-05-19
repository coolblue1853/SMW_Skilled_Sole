using Unity.VisualScripting;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData Data;

    public string GetInteractPrompt()
    {
        string str = $"{Data.DisplayName}\n{Data.Descrition}";
        return str;
    }

    public void OnInteract()
    {
        //Player ��ũ��Ʈ ���� ����
       // CharacterManager.Instance.Player.itemData = data;
       // CharacterManager.Instance.Player.addItem?.Invoke();
       // Destroy(gameObject);
    }
}