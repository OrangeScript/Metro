using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool isGameStarted = false;
    public bool isGameWon = false;
    public bool isGameOver = false;
    public int rescuedNPC = 0;
    public int maxNPC;
    private string startMessage= @"在一片混沌之中你缓缓睁开眼睛，
眼前是一片黑暗，
你尝试着呼吸，空气似乎也凝固了一般。
你试图在记忆之海里拼凑仅剩的碎片，但只是徒劳罢了。
脑海中只剩下刺耳的刹车声、灼热的气浪，以及人群的尖叫。

“他们本可以活下来的。”
一道声音从虚空中传来，非男非女，却带着电流般的震颤。
“那些选择……真的无法改变吗？”
你试图回应，却发现喉咙被无形的手扼住。

黑暗褪去，
";
    private string wonMessage= @"你用撬棍卡住最后一扇门，嘶吼着让幸存者冲向地面。
人群跌跌撞撞爬上楼梯，而你扶着墙剧烈咳嗽，掌心满是黑血。
转角处，一缕天光透入，但你的双腿再也无法挪动。
视野被灰雾吞噬，神秘声音讥讽道：
“看啊……你成了他们的‘英雄’，但谁记得你的名字？”
“你的命运真的有改变吗？”
";
    private string failMessage = @"浓烟如巨蟒般缠绕车厢，手电筒光束逐渐暗淡。
你踉跄着试图推动昏迷的乘客，但肺部灼烧般的剧痛让你跪倒在地。
手机从口袋滑落，屏幕上是母亲的未接来电，提示音在死寂中格外刺耳。
视野被灰雾吞噬前，你听见神秘声音的低语：
“你连自己都救不了……谈何他人？”
地铁广播机械地重复着“救援延迟通知”，火焰又一次吞噬了一切。
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
            Debug.Log("开始面板加载完成，继续流程");
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
            Debug.Log("结果面板加载完成，开始滚动制作人名单");
            UIPanelFader.Instance.gameObject.SetActive(false);
            CreditsScroller.Instance.gameObject.SetActive(true);
            CreditsScroller.Instance.StartScrolling();
        });
        //Time.timeScale = 0;
    }
}