using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventilation : MonoBehaviour
{
    [Header("管道设置")]
    public Transform[] standardExits;
    public Transform hiddenExit;
    public float crawlSpeedMultiplier = 0.8f;

    [Header("碰撞体")]
    public Collider2D ventCollider;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.EnterTunnel(this);
                CheckForHiddenExit(player);
            }
        }
    }

    private void CheckForHiddenExit(PlayerController player)
    {
        Flashlight flashlight = player.GetComponentInChildren<Flashlight>();
        if (flashlight != null && flashlight.isFlashlightOn)
        {
            hiddenExit.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ExitTunnel();
            }
        }
    }

    public Transform GetRandomExit()
    {
        List<Transform> availableExits = new List<Transform>(standardExits);
        if (hiddenExit.gameObject.activeSelf)
        {
            availableExits.Add(hiddenExit);
        }
        return availableExits[Random.Range(0, availableExits.Count)];
    }
}
