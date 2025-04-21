using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerManager : MonoBehaviour,ISaveManager
{
    public static PlayerManager instance;
    [SerializeField]public PlayerController player;
    [SerializeField] private int money;
    public void LoadData(GameData _data)
    {
        money = _data.money;
        if(player != null) 
            player.transform.position=_data.NormalScenePlayerPosition;
    }

    public void SaveData(ref GameData _data)
    {
        _data.money = money;
        _data.NormalScenePlayerPosition = player.transform.position;
    }

    public void AddMoney(int money)
    {
        this.money += money;
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }
   
}
