using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public PlayerController player;

    private void OnTriggerEnter2D(Collider2D other)
    {     
        if (other.CompareTag("Player"))
        {
            Debug.Log("playerÓëexit²úÉúÅö×²");
            player.ReturnFromIllusionWorld();
        }
    }
}
