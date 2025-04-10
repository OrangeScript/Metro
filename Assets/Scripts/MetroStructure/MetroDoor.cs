using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public static bool isDancing;
    public int correctInputs;

    public static MetroDoor S;
    private PlayerController player;
    private void Awake()
    {
        if (S != null && S != this)
        {
            Debug.LogWarning("发现多个 MetroDoor 实例，已销毁冗余实例");
            Destroy(this.gameObject);
            return;
        }
        S = this;
        if (ArrowManager.S == null)
        {
            ArrowManager.S = FindObjectOfType<ArrowManager>();
        }
        if (MazeManager.instance == null)
        {
            MazeManager.instance = FindObjectOfType<MazeManager>();
        }
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
        if (currentFault == lastHandledFault)
            return;
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

    private void StartArrowPuzzle()
    {
        Debug.Log("进入QQ炫舞解谜界面...");
        if (ArrowManager.S != null)
        {
            ArrowManager.S.CreateWave(10);
        }
        else
        {
            Debug.LogError("ArrowManager instance is null");
        }
        isDancing = true;
        correctInputs = 0;
    }

    private void StartMazePuzzle()
    {
        Debug.Log("进入迷宫解谜界面...");
        //MazeManager.S?.StartMazePuzzle(OnMazeSolved);
    }

    public void StartMazePuzzleWithNoChange()
    {
        Debug.Log("进入迷宫解谜界面但不会影响门状态...");
        //MazeManager.S?.StartMazePuzzle(OnMazeSolvedNoChange);
    }

    private void OnMazeSolved()
    {
        Debug.Log("迷宫解谜成功，门变为卡住状态");
        currentState = DoorState.Jammed;
        anim.SetInteger("DoorState", (int)currentState);
    }

    private void OnMazeSolvedNoChange()
    {
        Debug.Log("迷宫解谜成功，但门状态不变");
    }

    public void FinishWave()
    {
        isDancing = false;
        ArrowManager.S?.ClearWave();
        Debug.Log($"结束解谜，正确数为 {correctInputs}");

        if (correctInputs == 10)  
        {
            Debug.Log("QQ炫舞解谜成功，门变为撬棍状态");
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
}
