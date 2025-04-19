using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
public class MetroDoor : MonoBehaviour
{
    public enum DoorState { Open, Closed, Jammed }
    public enum FaultType { None, Type1, Type2, Type3, Type4, Type5 }

    [Header("门的状态")]
    public DoorState currentState = DoorState.Closed;
    public FaultType currentFault = FaultType.None;
    private FaultType lastHandledFault = FaultType.None;

    [Header("门的属性")]
    public float openSpeed = 1.0f;
    public AudioClip openSound;

    private Animator anim;
    private AudioSource audioSource;

    private float arrowDuration = 5f;
    private float arrowTime = 0;
    public bool isDancing;
    public int correctInputs;
    private PlayerController player;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (ArrowManager.S == null)
            ArrowManager.S = FindObjectOfType<ArrowManager>();

        if (MazeManager.instance == null)
            MazeManager.instance = FindObjectOfType<MazeManager>();

        player = GetComponent<PlayerController>();
    }


    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentState= DoorState.Closed;
        anim.SetInteger("DoorState", (int)currentState);
    }

    void Update()
    {
        if (isDancing)
        {
            arrowTime += Time.deltaTime;
            if (arrowTime > arrowDuration)
            {
                FinishWave();
            }
        }
        anim.SetInteger("DoorState", (int)currentState);
    }


    public void TryInteract(PlayerController player)
    {
        if (!Battery.isPowered)
        {
            if (currentFault == FaultType.Type3 || currentFault == FaultType.Type4||currentFault==FaultType.Type5)
            {
                Debug.Log("需要备用电池才能修复门！");
                return;
            }
        }

        HandleFaultUpdate();
    }

    private void HandleFaultUpdate()
    {
        //if (currentFault == lastHandledFault)
            //return;
        switch (currentFault)
        {
            case FaultType.Type1:
                StartArrowPuzzle();
                break;
            case FaultType.Type2:
                StartMazePuzzle();
                break;
            default:
                break;
        }
    }


    public void OpenDoor()
    {
        if (currentState == DoorState.Jammed)
        {
            Debug.Log("门正在打开...");
            currentState = DoorState.Open;
            anim.SetInteger("DoorState", (int)currentState);
            if (openSound)
                audioSource.PlayOneShot(openSound);
        }
        else
        {
            Debug.Log("门无法打开！");
        }
    }
    #region Arrow
    private void StartArrowPuzzle()
    {
        Debug.Log("进入QQ炫舞解谜界面...");
        if (ArrowManager.S != null)
        {
            ArrowManager.S.CreateWave(10, this);
        }
        else
        {
            Debug.LogError("ArrowManager instance is null");
        }
        isDancing = true;
        correctInputs = 0;
    }

    public void FinishWave()
    {
        isDancing = false;
        ArrowManager.S?.ClearWave();
        Debug.Log($"结束解谜，正确数为 {correctInputs}");
        UIManager.Instance.ShowMessage("解谜失败，门状态不变!");

        if (correctInputs == 10)
        {
            Debug.Log("QQ炫舞解谜成功，门变为撬棍状态");
            UIManager.Instance.ShowMessage("解谜成功，门变为卡住状态!");
            currentState = DoorState.Jammed;
            anim.SetInteger("DoorState", (int)currentState);
        }
    }

    public void RecordInput(bool isCorrect)
    {
        if (isCorrect)
        {
            correctInputs++;
        }
    }
    #endregion

    #region Maze
    private void StartMazePuzzle()
    {
        Debug.Log("进入迷宫解谜界面...");

        SceneManager.sceneLoaded += OnMazeSceneLoaded;
        SceneManager.LoadScene("Maze", LoadSceneMode.Additive);
    }

    private void OnMazeSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
            Debug.Log("迷宫场景加载完成，开始迷宫解谜逻辑");

            MazeManager.instance?.StartMazePuzzle(OnMazeSolved, OnMazeFailed);
            SceneManager.SetActiveScene(scene);
            SceneManager.sceneLoaded -= OnMazeSceneLoaded;
        }
    }
    public IEnumerator HandleMazePuzzleWithNoChange(MetroDoor door)
    {
        yield return StartCoroutine(door.StartMazePuzzleWithNoChange(() =>
        {
            Debug.Log("迷宫解谜完成，后续操作执行！");
            door.currentFault = MetroDoor.FaultType.Type1;  
        }));

    }
    public IEnumerator StartMazePuzzleWithNoChange(Action onCompleted) { 

        Debug.Log("进入迷宫解谜界面但不会影响门状态...");

        // 定义局部回调函数
        bool isCompleted = false;
        Action onLocalSolved = () =>
        {
            Debug.Log("迷宫解谜成功，但门状态不变");
            isCompleted = true;
            onCompleted?.Invoke();
        };

        Action onLocalFailed = () =>
        {
            Debug.Log("迷宫解谜失败！");
            isCompleted = true;
            onCompleted?.Invoke();
        };

        // 注册一次性场景加载监听
        SceneManager.sceneLoaded += OnMazeSceneLoaded_NoChange;
        SceneManager.LoadScene("Maze", LoadSceneMode.Additive);

        // 等待场景加载并激活
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Maze").isLoaded);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Maze"));

        // 确保只在此处触发解谜逻辑
        if (MazeManager.instance != null)
        {
            MazeManager.instance.StartMazePuzzle(onLocalSolved, onLocalFailed);
        }
        else
        {
            Debug.LogError("MazeManager 实例未找到！");
        }

        // 等待解谜完成
        yield return new WaitUntil(() => isCompleted);

        // 清理资源
        SceneManager.sceneLoaded -= OnMazeSceneLoaded_NoChange;
        yield return StartCoroutine(UnloadMazeScene());
    }


    private void OnMazeSceneLoaded_NoChange(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
            // 移除多余的StartMazePuzzle调用，仅注销事件
            SceneManager.sceneLoaded -= OnMazeSceneLoaded_NoChange;
        }
    }
    private void OnMazeSolved()
    {
        Debug.Log("迷宫解谜成功，门变为卡住状态");

        StartCoroutine(UnloadMazeScene());
        UIManager.Instance.ShowMessage("迷宫解谜成功，门变为卡住状态!");
        currentState = DoorState.Jammed;
        anim.SetInteger("DoorState", (int)currentState);
    }


    private void OnMazeSolvedNoChange()
    {
        Debug.Log("迷宫解谜成功，但门状态不变");
        UIManager.Instance.ShowMessage("迷宫解谜成功，门状态不变!");

        StartCoroutine(UnloadMazeScene());
    }

    private void OnMazeFailed()
    {
        Debug.Log("迷宫解谜失败！");
        UIManager.Instance.ShowMessage("迷宫解谜失败!");
        StartCoroutine(UnloadMazeScene());
    }

    private IEnumerator UnloadMazeScene()
    {
        Scene mazeScene = SceneManager.GetSceneByName("Maze");
        if (mazeScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(mazeScene);
            Debug.Log("迷宫场景已卸载");
            Resources.UnloadUnusedAssets(); // 清理残留资源
        }
    }
    #endregion

}
