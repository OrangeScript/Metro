using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;
using static UIManager;

public class Window : InteractableObject
{
    [Header("窗口设置")]
    public Collider2D windowCollider;
    //public GameObject escapePoint;

    private bool isBroken = false;

    public override void OnInteract() {

        if (!isBroken) return;

        if (player != null && player.currentState == PlayerState.Carrying)
        {
            player.carriedNPC = null;
            player.TransitionState(PlayerState.Normal);
            GameManager.Instance.rescuedNPC++;
            Debug.Log("已救出 NPC！");
            if (GameManager.Instance.rescuedNPC == GameManager.Instance.maxNPC)
            {
                GameManager.Instance.isGameWon = true;
                GameManager.Instance.EndGame();
            }
        }
    }
    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        Debug.Log("窗户被砸开，可以跳出！");
    }

    public bool IsBroken()
    {
        return isBroken;
    }
}

