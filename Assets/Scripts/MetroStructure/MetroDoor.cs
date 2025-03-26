using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroDoor : MonoBehaviour
{
    public enum DoorState { Open, Closed, Jammed }
    public enum FaultType { None, Type1, Type2, Type3, Type4, Type5 }

    [Header("门状态")]
    public DoorState currentState = DoorState.Closed;
    public FaultType currentFault = FaultType.None;

    [Header("门的属性")]
    public float openSpeed = 1.0f;
    public AudioClip openSound;

    private Animator animator;
    private AudioSource audioSource;

    private float arrowDuration = 5f;
    private float arrowTime = 0;
    public static bool isDancing;
    private int correctInputs;

    public static MetroDoor S;

    private void Awake()
    {
        S = this;
        if (ArrowManager.S == null)
        {
            ArrowManager.S = FindObjectOfType<ArrowManager>();
        }
        if (MazeManager.S == null)
        {
            MazeManager.S = FindObjectOfType<MazeManager>();
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
    }

    public void TryInteract(PlayerController player)
    {
        if (!Battery.isPowered)
        {
            if (currentFault == FaultType.Type3 || currentFault == FaultType.Type4)
            {
                Debug.Log("电池不足，无法修复此故障！");
            }
            else
            {
                Debug.Log("需要备用电池才能修复门！");
            }
            return;
        }

        switch (currentFault)
        {
            case FaultType.Type1:
            case FaultType.Type3:
                StartArrowPuzzle();
                break;
            case FaultType.Type2:
            case FaultType.Type4:
                StartMazePuzzle();
                break;
            case FaultType.Type5:
                StartMazePuzzleWithNoChange();
                StartArrowPuzzle();
                break;
            default:
                break;
        }
    }


    private void OpenDoor()
    {
        if (currentState == DoorState.Jammed)
        {
            Debug.Log("门正在打开...");
            currentState = DoorState.Open;
            animator.SetTrigger("Open");
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
        MazeManager.S?.StartMazePuzzle(OnMazeSolved);
    }

    private void StartMazePuzzleWithNoChange()
    {
        Debug.Log("进入迷宫解谜界面但不会影响门状态...");
        MazeManager.S?.StartMazePuzzle(OnMazeSolvedNoChange);
    }

    private void OnMazeSolved()
    {
        Debug.Log("迷宫解谜成功，门变为卡住状态");
        currentState = DoorState.Jammed;
    }

    private void OnMazeSolvedNoChange()
    {
        Debug.Log("迷宫解谜成功，但门状态不变");
    }

    public void FinishWave()
    {
        isDancing = false;
        ArrowManager.S?.ClearWave();

        if (correctInputs >= 10)  // 确保至少按对 10 次
        {
            Debug.Log("QQ炫舞解谜成功，门变为撬棍状态");
            currentState = DoorState.Jammed;
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
