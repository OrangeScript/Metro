using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duct : MonoBehaviour
{
    [Header("管道设置")]
    public Transform[] standardExits;
    public Transform hiddenExit;
    public float crawlSpeedMultiplier = 0.8f;

    [Header("碰撞体")]
    public Collider2D ventCollider;

    private PlayerController player;
    private bool hasFlashlight;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();
            if (player.isCrawling)
            {
                AdjustPlayerMovement();
                HandleExitDiscovery();
            }
        }
    }

    void AdjustPlayerMovement()
    {
        player.crawlSpeed *= crawlSpeedMultiplier;
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    void HandleExitDiscovery()
    {
        Flashlight flashlight = player.GetComponentInChildren<Flashlight>();
        if (flashlight != null && flashlight.isFlashlightOn)
        {
            hiddenExit.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.crawlSpeed /= crawlSpeedMultiplier;
            player.GetComponent<Rigidbody2D>().gravityScale = 1;
            player = null;
        }
    }

    public Transform GetRandomExit()
    {
        List<Transform> availableExits = new List<Transform>(standardExits);
        if (hiddenExit.gameObject.activeSelf)
            availableExits.Add(hiddenExit);

        return availableExits[Random.Range(0, availableExits.Count)];
    }
}
