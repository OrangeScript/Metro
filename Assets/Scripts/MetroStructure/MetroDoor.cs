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
                Debug.Log("��Ҫ���õ�ز����޸��ţ�");
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
    #region Arrow
    private void StartArrowPuzzle()
    {
        Debug.Log("����QQ������ս���...");
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
        Debug.Log($"�������գ���ȷ��Ϊ {correctInputs}");
        UIManager.Instance.ShowMessage("����ʧ�ܣ���״̬����!");

        if (correctInputs == 10)
        {
            Debug.Log("QQ������ճɹ����ű�Ϊ�˹�״̬");
            UIManager.Instance.ShowMessage("���ճɹ����ű�Ϊ��ס״̬!");
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
        Debug.Log("�����Թ����ս���...");

        SceneManager.sceneLoaded += OnMazeSceneLoaded;
        SceneManager.LoadScene("Maze", LoadSceneMode.Additive);
    }

    private void OnMazeSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
            Debug.Log("�Թ�����������ɣ���ʼ�Թ������߼�");

            MazeManager.instance?.StartMazePuzzle(OnMazeSolved, OnMazeFailed);
            SceneManager.SetActiveScene(scene);
            SceneManager.sceneLoaded -= OnMazeSceneLoaded;
        }
    }
    public IEnumerator HandleMazePuzzleWithNoChange(MetroDoor door)
    {
        yield return StartCoroutine(door.StartMazePuzzleWithNoChange(() =>
        {
            Debug.Log("�Թ�������ɣ���������ִ�У�");
            door.currentFault = MetroDoor.FaultType.Type1;  
        }));

    }
    public IEnumerator StartMazePuzzleWithNoChange(Action onCompleted) { 

        Debug.Log("�����Թ����ս��浫����Ӱ����״̬...");

        // ����ֲ��ص�����
        bool isCompleted = false;
        Action onLocalSolved = () =>
        {
            Debug.Log("�Թ����ճɹ�������״̬����");
            isCompleted = true;
            onCompleted?.Invoke();
        };

        Action onLocalFailed = () =>
        {
            Debug.Log("�Թ�����ʧ�ܣ�");
            isCompleted = true;
            onCompleted?.Invoke();
        };

        // ע��һ���Գ������ؼ���
        SceneManager.sceneLoaded += OnMazeSceneLoaded_NoChange;
        SceneManager.LoadScene("Maze", LoadSceneMode.Additive);

        // �ȴ��������ز�����
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Maze").isLoaded);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Maze"));

        // ȷ��ֻ�ڴ˴����������߼�
        if (MazeManager.instance != null)
        {
            MazeManager.instance.StartMazePuzzle(onLocalSolved, onLocalFailed);
        }
        else
        {
            Debug.LogError("MazeManager ʵ��δ�ҵ���");
        }

        // �ȴ��������
        yield return new WaitUntil(() => isCompleted);

        // ������Դ
        SceneManager.sceneLoaded -= OnMazeSceneLoaded_NoChange;
        yield return StartCoroutine(UnloadMazeScene());
    }


    private void OnMazeSceneLoaded_NoChange(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Maze")
        {
            // �Ƴ������StartMazePuzzle���ã���ע���¼�
            SceneManager.sceneLoaded -= OnMazeSceneLoaded_NoChange;
        }
    }
    private void OnMazeSolved()
    {
        Debug.Log("�Թ����ճɹ����ű�Ϊ��ס״̬");

        StartCoroutine(UnloadMazeScene());
        UIManager.Instance.ShowMessage("�Թ����ճɹ����ű�Ϊ��ס״̬!");
        currentState = DoorState.Jammed;
        anim.SetInteger("DoorState", (int)currentState);
    }


    private void OnMazeSolvedNoChange()
    {
        Debug.Log("�Թ����ճɹ�������״̬����");
        UIManager.Instance.ShowMessage("�Թ����ճɹ�����״̬����!");

        StartCoroutine(UnloadMazeScene());
    }

    private void OnMazeFailed()
    {
        Debug.Log("�Թ�����ʧ�ܣ�");
        UIManager.Instance.ShowMessage("�Թ�����ʧ��!");
        StartCoroutine(UnloadMazeScene());
    }

    private IEnumerator UnloadMazeScene()
    {
        Scene mazeScene = SceneManager.GetSceneByName("Maze");
        if (mazeScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(mazeScene);
            Debug.Log("�Թ�������ж��");
            Resources.UnloadUnusedAssets(); // ���������Դ
        }
    }
    #endregion

}
