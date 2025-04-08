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

    // ������ͷ��
    public void CreateWave(int length)
    {
        arrowsHolder.gameObject.SetActive(true);
        Debug.Log($"�������ɼ�ͷ��ʣ�����: {length}");
        arrows = new Queue<Arrow>();
        isFinish = false;

        for (int i = 0; i < length; i++)
        {
            Arrow arrow = Instantiate(
            arrowPrefab,
            arrowsHolder.position, // ʹ�ø�����λ��
            Quaternion.identity,
            arrowsHolder)
            .GetComponent<Arrow>();

            arrow.transform.localPosition += new Vector3(i * 100, 0, 0);
            int randomDir = Random.Range(0, 4);  // ������ɼ�ͷ����
            arrow.Setup(randomDir);

            arrows.Enqueue(arrow);
        }

        currentArrow = arrows.Dequeue();  // ��ȡ��һ����ͷ
    }

    // �����������
    public void TypeArrow(KeyCode inputKey)
    {
        if (isFinish)
            return;
        if (ConvertKeyCodeToInt(inputKey) == currentArrow.arrowDir)
        {
            currentArrow.SetFinish();  // ������ȷ������Ϊ���״̬
            //victorySound.Play();
            MetroDoor.S.RecordInput(true);  // ��¼��ȷ����
        }
        else
        {
            currentArrow.SetError();  // ���������ʾ������ɫ
            //missSound.Play();
            MetroDoor.S.RecordInput(false);  // ��¼��������
        }

        // ��������һ����ͷ
        if (arrows.Count > 0)
        {
            currentArrow = arrows.Dequeue();  // ������һ����ͷ
        }
        else
        {
            isFinish = true;  // ������м�ͷ�������꣬��ǹؿ����
        }

        Debug.Log($"������ȷ����ǰ correctInputs: {MetroDoor.S.correctInputs}");
    }

    // ��ռ�ͷ��
    public void ClearWave()
    {
        arrows = new Queue<Arrow>();
        foreach (Transform arrow in arrowsHolder)
        {
            Destroy(arrow.gameObject);
        }
        arrowsHolder.gameObject.SetActive(false);
    }

    // ����������ת��Ϊ��ͷ����0-3��
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