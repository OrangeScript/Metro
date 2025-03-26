using UnityEngine;
using System;

public class MazeManager : MonoBehaviour
{
    public static MazeManager S;
    private Action onMazeComplete; // 迷宫解谜成功后的回调

    public MazeGenerator mazeGenerator; // 迷宫生成器
    public PlayerController playerController; // 监听玩家解谜进度

    private void Awake()
    {
        S = this;
    }

    public void StartMazePuzzle(Action onComplete)
    {
        Debug.Log("开启迷宫解谜...");
        onMazeComplete = onComplete;

        // 生成新的迷宫
        mazeGenerator.GenerateMaze();
        mazeGenerator.RenderMaze();  // 让迷宫可见

        // 显示 UI
        UIManager.Instance.ShowMazeUI();
    }

    private void OnMazeSolved()
    {
        Debug.Log("迷宫解谜成功！");
        onMazeComplete?.Invoke();

        //关闭迷宫UI
        UIManager.Instance.HideMazeUI();
    }
}
