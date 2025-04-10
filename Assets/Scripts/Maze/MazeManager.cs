using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class MazeManager : MonoBehaviour
{
    public static MazeManager instance;
    private Action onMazeComplete; // 迷宫解谜成功后的回调

    public MazeGenerator mazeGenerator; // 迷宫生成器
    public MazePlayer player; // 监听玩家解谜进度
    [SerializeField] private GameObject playerPrefab;
    
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        /*         * 这里是测试代码，按空格键生成迷宫和玩家
         */
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mazeGenerator.GenerateMaze();
            GameObject p=Instantiate(playerPrefab,mazeGenerator.start,Quaternion.identity);
            player=p.GetComponent<MazePlayer>();
            mazeGenerator.RenderMaze();
        }
        //test return
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("NormalScene");
        }
    }
    public void StartMazePuzzle()
    {
        Debug.Log("开启迷宫解谜...");
        player.gameObject.transform.position = Vector3.one;
        // 生成新的迷宫
        mazeGenerator.GenerateMaze();
        mazeGenerator.RenderMaze();  // 让迷宫可见

        //// 显示 UI
        //UIManager.Instance.ShowMazeUI();
    }

    private void OnMazeSolved()
    {
        Debug.Log("迷宫解谜成功！");
        //onMazeComplete?.Invoke();
        SceneManager.LoadScene("Normal");
        ////关闭迷宫UI
        //UIManager.Instance.HideMazeUI();
    }
}