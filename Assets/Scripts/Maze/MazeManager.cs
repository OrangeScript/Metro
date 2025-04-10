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
        //press space to begin
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mazeGenerator.GenerateDynamicMaze();
            GameObject p=Instantiate(playerPrefab,mazeGenerator.start,Quaternion.identity);
            player=p.GetComponent<MazePlayer>();
            //mazeGenerator.RenderMaze();
        }
        //test return
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("NormalScene");
        }
    }
    
}