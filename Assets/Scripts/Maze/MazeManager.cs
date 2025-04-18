using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class MazeManager : MonoBehaviour
{
    public static MazeManager instance;
    private Action onMazeComplete; // 迷宫解谜成功后的回调
    private Action onMazeFailed; // 迷宫解谜失败后的回调

    public MazeGenerator mazeGenerator; // 迷宫生成器
    public MazePlayer player; // 监听玩家解谜进度
    [SerializeField] public GameObject playerPrefab;
    
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        //press space to begin//test method
        if (Input.GetKeyDown(KeyCode.F))
        {
            player=mazeGenerator.GenerateMaze();
           
            //mazeGenerator.RenderMaze();
        }
        //test return
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("NormalScene");
        }
        if (player != null && player.failTheMaze)
        {
            onMazeFailed?.Invoke();
            DestroyMaze();
            //RestartMaze();
        }
        if (player != null && player.winTheMaze)
        {
            // 迷宫解谜成功，执行回调
            onMazeComplete?.Invoke();
        }
    }

    private void RestartMaze()
    {
        player = mazeGenerator.GenerateMaze();
    }

    private void DestroyMaze()
    {
        Destroy(player.gameObject);
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); // 销毁子物体
        }
    }

    public void StartMazePuzzle(Action action,Action failedAction)
    {
        player = mazeGenerator.GenerateMaze();
        onMazeComplete = action; // 设置回调
        onMazeFailed = failedAction;
    }
    
}