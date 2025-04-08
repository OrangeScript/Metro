using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    [Header("��������")]
    public Sprite brokenWindowSprite;
    public Collider2D windowCollider;
    //public GameObject escapePoint;

    private bool isBroken = false;

    public void Break()
    {
        if (isBroken) return;
        isBroken = true;
        GetComponent<SpriteRenderer>().sprite = brokenWindowSprite;
        windowCollider.enabled = false;
        //escapePoint.SetActive(true);
        Debug.Log("�������ҿ�������������");
    }
}

