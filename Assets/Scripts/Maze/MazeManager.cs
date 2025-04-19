using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class MazeManager : MonoBehaviour,ISaveManager
{
    public static MazeManager instance;
    private Action onMazeComplete; // �Թ����ճɹ���Ļص�

    public MazeGenerator mazeGenerator; // �Թ�������
    public MazePlayer player; // ������ҽ��ս���
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
            //// �Թ����ճɹ���ִ�лص�
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
            Destroy(child.gameObject); // ����������
        }
    }

    public void StartMazePuzzle(Action action)
    {
        player = mazeGenerator.GenerateMaze();
        onMazeComplete = action; // ���ûص�
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