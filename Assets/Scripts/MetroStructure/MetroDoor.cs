using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    //QQ炫舞
    private float arrowDuration = 5f;
    private float arrowTime = 0;
    public static bool isDancing;
    private int correctInputs;

    //迷宫解谜

    public static MetroDoor S;
    private void Awake()
    {
        S = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TryInteract(PlayerController player)
    {
        if (!Battery.isPowered)
        {
            Debug.Log("需要备用电池才能修复门！");
            return;
        }

        switch (currentFault)
        {
            case FaultType.Type1:
                StartFault1();
                break;
            case FaultType.Type2:
                StartFault2();
                break;
            case FaultType.Type3:
                if (Battery.isPowered) StartFault1();
                break;
            case FaultType.Type4:
                if (Battery.isPowered) StartFault2();
                break;
            case FaultType.Type5:
                if (Battery.isPowered) StartFault5();
                break;
            default:
                Debug.Log("门无故障，无法交互！");
                break;
        }
    }

    private void StartFault1()
    {
        //TODO:UI显示按下F
        Debug.Log("进入QQ炫舞解密界面...");
        ArrowPhase();
    }

    private void StartFault2()
    {
        //TODO:UI显示按下F
        Debug.Log("进入迷宫解密界面...");
        MazePhase();
    }

    private void StartFault5()
    {
        //TODO:UI显示按下F
        Debug.Log("进入迷宫解密界面但不能改变门状态...");
        MazePhaseNULL();
        Debug.Log("进入QQ炫舞解密界面...");
        ArrowPhase();
    }


    public void OpenDoor()
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
            Debug.Log("门不为撬棍状态，无法被撬棍打开！");
        }
    }

    public void ArrowPhase()
    {
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
        if (arrowTime > arrowDuration)
        {
            FinishWave();
        }
    }

    public void MazePhase()
    {
        if (MazeManager.S != null)
        {
            MazeManager.S.StartMazePuzzle(OnMazeSolved);
        }
        else
        {
            Debug.LogError("MazeManager instance is null");
        }
    }

    public void MazePhaseNULL()
    {
        if (MazeManager.S != null)
        {
            MazeManager.S.StartMazePuzzle(OnMazeSolvedNULL);
        }
        else
        {
            Debug.LogError("MazeManager instance is null");
        }
    }
    private void OnMazeSolved()
    {
        Debug.Log("迷宫解谜成功，门变为撬棍状态");
        currentState = DoorState.Jammed; 
    }

    private void OnMazeSolvedNULL()
    { 
        Debug.Log("迷宫解谜成功，门状态不变"); 
    }

    public void FinishWave()
    {
        isDancing = false;
        ArrowManager.S.ClearWave();
        if (correctInputs == 10)
        {
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
