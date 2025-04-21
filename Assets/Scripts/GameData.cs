using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
/*
 * Put everything needs to be regonized here
 */
public class GameData 
{

    public int money;
    public Vector3 NormalScenePlayerPosition;

    public int mazeMoney;
    public GameData()
    {
        money = 0;
        mazeMoney = 0;
        NormalScenePlayerPosition= Vector3.zero;
    }
}
