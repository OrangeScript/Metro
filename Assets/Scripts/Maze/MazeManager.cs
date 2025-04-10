using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class MazeManager : MonoBehaviour
{
    public static MazeManager instance;
    private Action onMazeComplete; // �Թ����ճɹ���Ļص�

    public MazeGenerator mazeGenerator; // �Թ�������
    public MazePlayer player; // ������ҽ��ս���
    [SerializeField] private GameObject playerPrefab;
    
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        /*         * �����ǲ��Դ��룬���ո�������Թ������
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
        Debug.Log("�����Թ�����...");
        player.gameObject.transform.position = Vector3.one;
        // �����µ��Թ�
        mazeGenerator.GenerateMaze();
        mazeGenerator.RenderMaze();  // ���Թ��ɼ�

        //// ��ʾ UI
        //UIManager.Instance.ShowMazeUI();
    }

    private void OnMazeSolved()
    {
        Debug.Log("�Թ����ճɹ���");
        //onMazeComplete?.Invoke();
        SceneManager.LoadScene("Normal");
        ////�ر��Թ�UI
        //UIManager.Instance.HideMazeUI();
    }
}