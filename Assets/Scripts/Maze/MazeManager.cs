using UnityEngine;
using System;

public class MazeManager : MonoBehaviour
{
    public static MazeManager S;
    private Action onMazeComplete; // �Թ����ճɹ���Ļص�

    public MazeGenerator mazeGenerator; // �Թ�������
    public PlayerController playerController; // ������ҽ��ս���

    private void Awake()
    {
        S = this;
    }

    public void StartMazePuzzle(Action onComplete)
    {
        Debug.Log("�����Թ�����...");
        onMazeComplete = onComplete;

        // �����µ��Թ�
        mazeGenerator.GenerateMaze();
        mazeGenerator.RenderMaze();  // ���Թ��ɼ�

        // ��ʾ UI
        UIManager.Instance.ShowMazeUI();
    }

    private void OnMazeSolved()
    {
        Debug.Log("�Թ����ճɹ���");
        onMazeComplete?.Invoke();

        //�ر��Թ�UI
        UIManager.Instance.HideMazeUI();
    }
}
