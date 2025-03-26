using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroDoor : MonoBehaviour
{
    public enum DoorState { Open, Closed, Jammed }
    public enum FaultType { None, Type1, Type2, Type3, Type4, Type5 }

    [Header("��״̬")]
    public DoorState currentState = DoorState.Closed;
    public FaultType currentFault = FaultType.None;

    [Header("�ŵ�����")]
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
                Debug.Log("��ز��㣬�޷��޸��˹��ϣ�");
            }
            else
            {
                Debug.Log("��Ҫ���õ�ز����޸��ţ�");
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
            Debug.Log("�����ڴ�...");
            currentState = DoorState.Open;
            animator.SetTrigger("Open");
            if (openSound)
                audioSource.PlayOneShot(openSound);
        }
        else
        {
            Debug.Log("���޷��򿪣�");
        }
    }

    private void StartArrowPuzzle()
    {
        Debug.Log("����QQ������ս���...");
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
        Debug.Log("�����Թ����ս���...");
        MazeManager.S?.StartMazePuzzle(OnMazeSolved);
    }

    private void StartMazePuzzleWithNoChange()
    {
        Debug.Log("�����Թ����ս��浫����Ӱ����״̬...");
        MazeManager.S?.StartMazePuzzle(OnMazeSolvedNoChange);
    }

    private void OnMazeSolved()
    {
        Debug.Log("�Թ����ճɹ����ű�Ϊ��ס״̬");
        currentState = DoorState.Jammed;
    }

    private void OnMazeSolvedNoChange()
    {
        Debug.Log("�Թ����ճɹ�������״̬����");
    }

    public void FinishWave()
    {
        isDancing = false;
        ArrowManager.S?.ClearWave();

        if (correctInputs >= 10)  // ȷ�����ٰ��� 10 ��
        {
            Debug.Log("QQ������ճɹ����ű�Ϊ�˹�״̬");
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
