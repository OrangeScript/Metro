using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroStructure : MonoBehaviour
{
    [Header("尺寸设置")]
    public float characterHeight = 2f; // 主角高度
    public float lengthMultiplier = 17f;
    public float heightMultiplier = 2f;

    [Header("烟雾容量")]
    public int maxSmokeLevel1 = 50;
    public int maxSmokeLevel2 = 50;
    public int maxSmokeLevel3 = 50;

    [Header("连接点")]
    public Transform ventilationEntry; // 通风管道入口
    public Transform[] windowPoints; // 玻璃位置

    void Start()
    {
        // 自动设置尺寸
        float finalHeight = characterHeight * heightMultiplier;
        float finalLength = characterHeight * lengthMultiplier;
        GetComponent<BoxCollider2D>().size = new Vector2(finalLength, finalHeight);
    }

    // 烟雾管理方法
    public void AddSmoke(int level, int amount) {/*...*/}
    public void ClearSmoke() {/*...*/}
}
