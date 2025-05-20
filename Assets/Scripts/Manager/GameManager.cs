using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool isGameStarted = false;
    public bool isGameWon = false;
    public bool isGameOver = false;
    public int rescuedNPC = 0;
    public int maxNPC;
    private string startMessage= @"��һƬ����֮���㻺�������۾���
��ǰ��һƬ�ڰ���
�㳢���ź����������ƺ�Ҳ������һ�㡣
����ͼ�ڼ���֮����ƴ�ս�ʣ����Ƭ����ֻ��ͽ�Ͱ��ˡ�
�Ժ���ֻʣ�´̶���ɲ���������ȵ����ˣ��Լ���Ⱥ�ļ�С�

�����Ǳ����Ի������ġ���
һ������������д��������з�Ů��ȴ���ŵ�����������
����Щѡ�񡭡�����޷��ı��𣿡�
����ͼ��Ӧ��ȴ���ֺ��������ε��ֶ�ס��

�ڰ���ȥ��
";
    private string wonMessage= @"�����˹���ס���һ���ţ�˻�������Ҵ��߳�����档
��Ⱥ����ײײ����¥�ݣ��������ǽ���ҿ��ԣ��������Ǻ�Ѫ��
ת�Ǵ���һ�����͸�룬�����˫����Ҳ�޷�Ų����
��Ұ���������ɣ����������������
������������������ǵġ�Ӣ�ۡ�����˭�ǵ�������֣���
�������������иı��𣿡�
";
    private string failMessage = @"Ũ�����������Ƴ��ᣬ�ֵ�Ͳ�����𽥰�����
����������ͼ�ƶ����Եĳ˿ͣ����β����հ�ľ�ʹ������ڵء�
�ֻ��ӿڴ����䣬��Ļ����ĸ�׵�δ�����磬��ʾ���������и���̶���
��Ұ����������ǰ�����������������ĵ��
�������Լ����Ȳ��ˡ���̸�����ˣ���
�����㲥��е���ظ��š���Ԯ�ӳ�֪ͨ����������һ��������һ�С�
";
    public UIPanelFader panelFader;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            EndGame();
        }
    }

    public void StartGame()
    {
        UIPanelFader.Instance.gameObject.SetActive(true);
        UIPanelFader.Instance.Setup(UIPanelFader.FadeMode.FadeTextOnly, startMessage);
        UIPanelFader.Instance.Play(() =>
        {
            Debug.Log("��ʼ��������ɣ���������");
            UIPanelFader.Instance.gameObject.SetActive(false);
            UIManager.Instance.ShowSet();
        });
        isGameStarted = true;
        isGameWon = false;
    }

    public void EndGame()
    {
        UIManager.Instance.HideSet();
        isGameOver = true;
        isGameStarted = false;
        string message = isGameWon ? wonMessage : failMessage;
        UIPanelFader.Instance.gameObject.SetActive(true);
        UIPanelFader.Instance.Setup(UIPanelFader.FadeMode.FadePanelAndText, message);
        UIPanelFader.Instance.Play(() =>
        {
            Debug.Log("�����������ɣ���ʼ��������������");
            UIPanelFader.Instance.gameObject.SetActive(false);
            CreditsScroller.Instance.gameObject.SetActive(true);
            CreditsScroller.Instance.StartScrolling();
        });
        //Time.timeScale = 0;
    }
}