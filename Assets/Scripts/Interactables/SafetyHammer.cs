using UnityEngine;

public class SafetyHammer : InteractableObject
{
    [Header("ÆÆ´°ÉèÖÃ")]
    public int hitsRequired = 3;
    public float hitCooldown = 0.5f;

    private int currentHits;
    private float lastHitTime;

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
        Collider2D[] windows = Physics2D.OverlapCircleAll(
            transform.position,
            1.5f,
            LayerMask.GetMask("Window")
        );

        foreach (var window in windows)
        {
            Window windowComp = window.GetComponent<Window>();
            if (windowComp != null)
            {
                windowComp.Break();
            }
        }
    }
}