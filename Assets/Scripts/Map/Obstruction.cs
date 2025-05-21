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
