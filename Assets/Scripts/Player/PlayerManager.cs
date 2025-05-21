using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public PlayerStatHandler StatHandler;
    public PlayerInteractController InteractController;
    public BuffController BuffController;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Init();
    }
    private void Init()
    {
        StatHandler = GetComponent<PlayerStatHandler>();
        InteractController = GetComponent<PlayerInteractController>();
        BuffController = GetComponent<BuffController>();
    }

}
