using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public PlayerStatHandler StatHandler;
    public PlayerInteractController InteractController;

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
    }

    private void Update()
    {
        // 테스트용
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StatHandler.Health -= 10;
        }
    }

}
