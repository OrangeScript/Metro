using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MetroDoor : MonoBehaviour
{
    public enum DoorState { Open, Closed, Jammed }
    public enum FaultType { None, Type1, Type2, Type3, Type4, Type5 }

    [Header("�ŵ�״̬")]
    public DoorState currentState = DoorState.Closed;
    public FaultType currentFault = FaultType.None;
    private FaultType lastHandledFault = FaultType.None;

    [Header("�ŵ�����")]
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
            Debug.LogWarning("���ֶ�� MetroDoor ʵ��������������ʵ��");
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
                Debug.Log("��Ҫ���õ�ز����޸��ţ�");
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
            Debug.Log("�����ڴ�...");
            currentState = DoorState.Open;
            anim.SetInteger("DoorState", (int)currentState);
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
        //MazeManager.S?.StartMazePuzzle(OnMazeSolved);
    }

    public void StartMazePuzzleWithNoChange()
    {
        Debug.Log("�����Թ����ս��浫����Ӱ����״̬...");
        //MazeManager.S?.StartMazePuzzle(OnMazeSolvedNoChange);
    }

    private void OnMazeSolved()
    {
        Debug.Log("�Թ����ճɹ����ű�Ϊ��ס״̬");
        currentState = DoorState.Jammed;
        anim.SetInteger("DoorState", (int)currentState);
    }

    private void OnMazeSolvedNoChange()
    {
        Debug.Log("�Թ����ճɹ�������״̬����");
    }

    public void FinishWave()
    {
        isDancing = false;
        ArrowManager.S?.ClearWave();
        Debug.Log($"�������գ���ȷ��Ϊ {correctInputs}");

        if (correctInputs == 10)  
        {
            Debug.Log("QQ������ճɹ����ű�Ϊ�˹�״̬");
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
