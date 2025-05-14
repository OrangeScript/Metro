using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAirWall : MonoBehaviour
{
    private Collider2D col2D;
    private PlayerController player;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        col2D = GetComponent<Collider2D>();
    }

    void Update()
    {
        bool shouldBlock = !(player.equippedItem is Flashlight);
        col2D.enabled = shouldBlock; // ���ֵ�Ͳʱ������ײ��
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player") && col2D.enabled)
        {
            UIManager.Instance.ShowMessage("���������Я���ֵ�Ͳ���ܽ���");
        }
    }
}