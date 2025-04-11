using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePlayer : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D collder;

    public float speed = 5f; 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collder = GetComponent<BoxCollider2D>();

    }

    private void Update()
    {
        // 获取玩家输入
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        // 计算移动方向sdaw
        Vector2 moveDirection = new Vector2(moveX, moveY);
        // 移动玩家
        if(Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity = moveDirection * speed * 2; // 按住左Shift加速
        }
        else
        {
            rb.velocity = moveDirection * speed; // 恢复正常速度
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MazeWall"))
        {
            // 处理与墙壁的碰撞
            Debug.Log("与墙壁碰撞");

            //show fail message


            //Destroy(gameObject);
        }
        else if (collision.CompareTag("MazeExit"))
        {
            // 处理到达出口的逻辑
            Debug.Log("到达迷宫出口");
            //MazeManager.S.StartMazePuzzle();
        }
    }
}
