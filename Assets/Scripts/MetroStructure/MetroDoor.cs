using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    //QQ����
    private float arrowDuration = 5f;
    private float arrowTime = 0;
    public static bool isDancing;
    private int correctInputs;

    //�Թ�����

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
            Debug.Log("��Ҫ���õ�ز����޸��ţ�");
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
                Debug.Log("���޹��ϣ��޷�������");
                break;
        }
    }

    private void StartFault1()
    {
        //TODO:UI��ʾ����F
        Debug.Log("����QQ������ܽ���...");
        ArrowPhase();
    }

    private void StartFault2()
    {
        //TODO:UI��ʾ����F
        Debug.Log("�����Թ����ܽ���...");
        MazePhase();
    }

    private void StartFault5()
    {
        //TODO:UI��ʾ����F
        Debug.Log("�����Թ����ܽ��浫���ܸı���״̬...");
        MazePhaseNULL();
        Debug.Log("����QQ������ܽ���...");
        ArrowPhase();
    }


    public void OpenDoor()
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
            Debug.Log("�Ų�Ϊ�˹�״̬���޷����˹��򿪣�");
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
        Debug.Log("�Թ����ճɹ����ű�Ϊ�˹�״̬");
        currentState = DoorState.Jammed; 
    }

    private void OnMazeSolvedNULL()
    { 
        Debug.Log("�Թ����ճɹ�����״̬����"); 
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
