using UnityEngine;
using static PlayerController;

public class IllusionWorld : MonoBehaviour
{
    public GameObject lightPoint;
    public float lightPointXPosition = 10f;

    private PlayerController player;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        lightPoint.transform.position = new Vector2(lightPointXPosition, lightPoint.transform.position.y);
    }

    private void Update()
    {
        // 检查玩家是否接触到光点
        if (player.currentState == PlayerState.Illusion && Vector2.Distance(player.transform.position, lightPoint.transform.position) < 1f)
        {
            // 玩家接触到光点，恢复到正常状态
            player.RecoverFromIllusion();
        }
    }
}

