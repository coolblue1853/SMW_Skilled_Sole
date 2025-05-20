using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Obstruction : MonoBehaviour
{
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
