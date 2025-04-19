using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class MazeManager : MonoBehaviour,ISaveManager
{
    public static MazeManager instance;
    private Action onMazeComplete; // 迷宫解谜成功后的回调

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
        if (Input.GetKeyDown(KeyCode.Space))
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
            DestroyMaze();

            RestartMaze();
        }
        if (player != null && player.winTheMaze)
        {
            Debug.Log("win");
            //// 迷宫解谜成功，执行回调
            BackToNormalScene(onMazeComplete);
        }
    }

    private static void BackToNormalScene(Action action)
    {
        SaveManager.instance.gameData1.money += 10000;
        SaveManager.instance.LoadScene("NormalScene",action);
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

    public void StartMazePuzzle(Action action)
    {
        player = mazeGenerator.GenerateMaze();
        onMazeComplete = action; // 设置回调
    }
    [SerializeField] private int mazeM;
    public void LoadData(GameData _data)
    {
        mazeM = _data.mazeMoney;
    }

    public void SaveData(ref GameData _data)
    {
        _data.mazeMoney = mazeM;
    }
}