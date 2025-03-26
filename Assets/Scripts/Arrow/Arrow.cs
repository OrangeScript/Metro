using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    public Sprite[] arrowSprites;  // 箭头方向的Sprite图像
    Image image;

    [HideInInspector]
    public int arrowDir;

    public Color finishColor;  // 完成后箭头的颜色
    public Color errorColor;   // 错误时箭头的颜色

    void Awake()
    {
        image = GetComponent<Image>();
    }

    // 初始化箭头设置
    public void Setup(int dir)
    {
        arrowDir = dir;
        image.sprite = arrowSprites[dir];
        image.SetNativeSize();
    }

    // 设置为完成状态，改变箭头颜色
    public void SetFinish()
    {
        image.color = finishColor;
    }

    // 设置为错误状态，改变箭头颜色
    public void SetError()
    {
        image.color = errorColor;
    }
}