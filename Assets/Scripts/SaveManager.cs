using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;


using UnityEngine.SceneManagement;
using System;
public class SaveManager : MonoBehaviour
{
    public GameData gameData1;
    public static SaveManager instance;
    private FileDataHandler dataHandler;
    private List<ISaveManager> saveManagers;


    public Action OnSceneLoadedAction;

    [Header("Settings for data")]
    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;


    /*  Change Scene should use this method only
     */
    public void LoadScene(string sceneName,Action onLoaded=null)
    {
        OnSceneLoadedAction = onLoaded;
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        SaveGame(); // 同步保存
        SceneManager.sceneLoaded += OnSceneLoaded;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

       
    }
    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 取消订阅

        LoadGame(); // 加载数据
        OnSceneLoadedAction?.Invoke(); // 执行回调
        OnSceneLoadedAction = null;
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [ContextMenu("Delete save file")]
    public void DeleteSavedFile()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }
    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        saveManagers = FindAllSaveManagers();
        LoadGame();
    }
    public void NewGame()
    {
        gameData1 = new GameData();
    }

    public void LoadGame()
    {
        gameData1 = dataHandler.Load();
        if (this.gameData1 == null)
        {
            Debug.Log("no data found");
            NewGame();
        }
        foreach (ISaveManager manager in saveManagers)
        {
            manager.LoadData(gameData1);
        }
    }

    public void SaveGame()
    {
        foreach (ISaveManager save in saveManagers)
        {
            save.SaveData(ref gameData1);
        }
        dataHandler.Save(gameData1);
        Debug.Log("Saved");

    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();
        return new List<ISaveManager>(saveManagers);
    }

    public bool HasSavedData()
    {
        if (dataHandler.Load() != null)
        {
            return true;
        }
        return false;

    }
}
