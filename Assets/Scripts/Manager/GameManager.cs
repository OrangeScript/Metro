using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool isGameStarted = false;
    public bool isGameWon = false;
    public bool isGameOver = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DialogManager.Instance.LoadDialogueFromCSV("Assets/Resources/DataTable/dialog.csv");
    }

    public void StartGame()
    {
        //Time.timeScale = 1;
        isGameStarted = true;
        isGameWon = false;
    }

    public void EndGame()
    {
        //Time.timeScale = 0;
        isGameStarted = false;
        if (isGameWon)
        {
            //victory
        }
        else
        {
            //defeat
        }
    }
}