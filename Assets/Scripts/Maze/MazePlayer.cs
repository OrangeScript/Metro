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
        // ��ȡ�������
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        // �����ƶ�����sdaw
        Vector2 moveDirection = new Vector2(moveX, moveY);
        // �ƶ����
        if(Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity = moveDirection * speed * 2; // ��ס��Shift����
        }
        else
        {
            rb.velocity = moveDirection * speed; // �ָ������ٶ�
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MazeWall"))
        {
            // ������ǽ�ڵ���ײ
            Debug.Log("��ǽ����ײ");

            //show fail message


            //Destroy(gameObject);
        }
        else if (collision.CompareTag("MazeExit"))
        {
            // ��������ڵ��߼�
            Debug.Log("�����Թ�����");
            //MazeManager.S.StartMazePuzzle();
        }
    }
}
