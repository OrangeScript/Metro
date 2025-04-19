using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroStructure : MonoBehaviour
{
    [Header("烟雾容量")]
    public int maxSmokeLevel1 = 50;
    public int maxSmokeLevel2 = 50;
    public int maxSmokeLevel3 = 50;

    [Header("连接点")]
    public Transform ventilationEntry; // 通风管道入口

    private int currentSmokeLevel1;
    private int currentSmokeLevel2;
    private int currentSmokeLevel3;

    void Start()
    { 
        ClearSmoke();
    }

    // 添加指定等级的烟雾
    public void AddSmoke(int level, int amount)
    {
        switch (level)
        {
            case 1:
                currentSmokeLevel1 = Mathf.Clamp(currentSmokeLevel1 + amount, 0, maxSmokeLevel1);
                break;
            case 2:
                currentSmokeLevel2 = Mathf.Clamp(currentSmokeLevel2 + amount, 0, maxSmokeLevel2);
                break;
            case 3:
                currentSmokeLevel3 = Mathf.Clamp(currentSmokeLevel3 + amount, 0, maxSmokeLevel3);
                break;
            default:
                Debug.LogWarning($"未知的烟雾等级: {level}");
                break;
        }
    }

    // 清除所有烟雾
    public void ClearSmoke()
    {
        currentSmokeLevel1 = 0;
        currentSmokeLevel2 = 0;
        currentSmokeLevel3 = 0;
    }

    public int GetCurrentSmoke(int level)
    {
        return level switch
        {
            1 => currentSmokeLevel1,
            2 => currentSmokeLevel2,
            3 => currentSmokeLevel3,
            _ => -1
        };
    }
}