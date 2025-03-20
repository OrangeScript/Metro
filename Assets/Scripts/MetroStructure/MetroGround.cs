using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroStructure : MonoBehaviour
{
    [Header("�ߴ�����")]
    public float characterHeight = 2f; // ���Ǹ߶�
    public float lengthMultiplier = 17f;
    public float heightMultiplier = 2f;

    [Header("��������")]
    public int maxSmokeLevel1 = 50;
    public int maxSmokeLevel2 = 50;
    public int maxSmokeLevel3 = 50;

    [Header("���ӵ�")]
    public Transform ventilationEntry; // ͨ��ܵ����
    public Transform[] windowPoints; // ����λ��

    void Start()
    {
        // �Զ����óߴ�
        float finalHeight = characterHeight * heightMultiplier;
        float finalLength = characterHeight * lengthMultiplier;
        GetComponent<BoxCollider2D>().size = new Vector2(finalLength, finalHeight);
    }

    // ���������
    public void AddSmoke(int level, int amount) {/*...*/}
    public void ClearSmoke() {/*...*/}
}
