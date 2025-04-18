using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class MazeManager : MonoBehaviour
{
    public static MazeManager instance;
    private Action onMazeComplete; // �Թ����ճɹ���Ļص�
    private Action onMazeFailed; // �Թ�����ʧ�ܺ�Ļص�

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
            // �Թ����ճɹ���ִ�лص�
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
            Destroy(child.gameObject); // ����������
        }
    }

    public void StartMazePuzzle(Action action,Action failedAction)
    {
        player = mazeGenerator.GenerateMaze();
        onMazeComplete = action; // ���ûص�
        onMazeFailed = failedAction;
    }
    
}