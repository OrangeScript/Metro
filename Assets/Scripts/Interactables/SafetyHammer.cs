using UnityEngine;

public class SafetyHammer : InteractableObject
{
    [Header("ÆÆ´°ÉèÖÃ")]
    public int hitsRequired = 3;
    public float hitCooldown = 0.5f;

    private int currentHits;
    private float lastHitTime;

    protected override void Start()
    {
        base.Start();
        destroyOnUse = false;
    }

    protected override void HandleUse()
    {
        if (Time.time - lastHitTime > hitCooldown)
        {
            currentHits++;
            lastHitTime = Time.time;

            if (currentHits >= hitsRequired)
            {
                BreakWindow();
                currentHits = 0;
            }
        }
    }

    private void BreakWindow()
    {
        if(player==null) {return;}
        if (player.nearestInteractable != null && player.nearestInteractable.CompareTag("Window"))
        {
            Window window = player.nearestInteractable.GetComponent<Window>();
            if(window!=null)window.Break();
        }
    }
}