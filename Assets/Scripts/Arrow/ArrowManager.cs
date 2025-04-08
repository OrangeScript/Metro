using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager S;
    [SerializeField] private AudioSource victorySound;
    [SerializeField] private AudioSource missSound;

    private void Awake()
    {
        S = this;
    }

    public GameObject arrowPrefab;
    public Transform arrowsHolder;
    public static bool isFinish;

    Queue<Arrow> arrows = new Queue<Arrow>();
    Arrow currentArrow;

    public float waveTime = 9f;  

    private void Start() {}

    // 创建箭头波
    public void CreateWave(int length)
    {
        arrowsHolder.gameObject.SetActive(true);
        Debug.Log($"正在生成箭头，剩余次数: {length}");
        arrows = new Queue<Arrow>();
        isFinish = false;

        for (int i = 0; i < length; i++)
        {
            Arrow arrow = Instantiate(
            arrowPrefab,
            arrowsHolder.position, // 使用父物体位置
            Quaternion.identity,
            arrowsHolder)
            .GetComponent<Arrow>();

            arrow.transform.localPosition += new Vector3(i * 100, 0, 0);
            int randomDir = Random.Range(0, 4);  // 随机生成箭头方向
            arrow.Setup(randomDir);

            arrows.Enqueue(arrow);
        }

        currentArrow = arrows.Dequeue();  // 获取第一个箭头
    }

    // 处理玩家输入
    public void TypeArrow(KeyCode inputKey)
    {
        if (isFinish)
            return;
        if (ConvertKeyCodeToInt(inputKey) == currentArrow.arrowDir)
        {
            currentArrow.SetFinish();  // 输入正确，设置为完成状态
            //victorySound.Play();
            MetroDoor.S.RecordInput(true);  // 记录正确输入
        }
        else
        {
            currentArrow.SetError();  // 输入错误，显示错误颜色
            //missSound.Play();
            MetroDoor.S.RecordInput(false);  // 记录错误输入
        }

        // 继续到下一个箭头
        if (arrows.Count > 0)
        {
            currentArrow = arrows.Dequeue();  // 继续下一个箭头
        }
        else
        {
            isFinish = true;  // 如果所有箭头都输入完，标记关卡完成
        }

        Debug.Log($"输入正确，当前 correctInputs: {MetroDoor.S.correctInputs}");
    }

    // 清空箭头波
    public void ClearWave()
    {
        arrows = new Queue<Arrow>();
        foreach (Transform arrow in arrowsHolder)
        {
            Destroy(arrow.gameObject);
        }
        arrowsHolder.gameObject.SetActive(false);
    }

    // 将键盘输入转换为箭头方向（0-3）
    int ConvertKeyCodeToInt(KeyCode key)
    {
        int result = 0;
        switch (key)
        {
            case KeyCode.UpArrow:
                result = 0;
                break;
            case KeyCode.DownArrow:
                result = 1;
                break;
            case KeyCode.LeftArrow:
                result = 2;
                break;
            case KeyCode.RightArrow:
                result = 3;
                break;
        }
        return result;
    }
}