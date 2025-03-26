using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager S;

    private void Awake()
    {
        S = this;
    }

    void Update()
    {
        if (!MetroDoor.isDancing)
        {
            return;  // 如果游戏暂停，不处理输入
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            ArrowManager.S.TypeArrow(KeyCode.UpArrow);

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            ArrowManager.S.TypeArrow(KeyCode.DownArrow);

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            ArrowManager.S.TypeArrow(KeyCode.LeftArrow);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ArrowManager.S.TypeArrow(KeyCode.RightArrow);

        // 如果玩家按下回车键，表示结束当前关卡
        if (Input.GetKeyDown(KeyCode.Return))
        {
            MetroDoor.S.FinishWave();  // 完成关卡

        }
    }
}