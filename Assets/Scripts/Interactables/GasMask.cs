using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasMask : InteractableObject
{
    [Header("√Êæﬂ…Ë÷√")]
    public GameObject maskVisual;
    public float oxygenCapacity = 300f;
    public AudioClip breathingSound;

    private float currentOxygen;
    private AudioSource audioSource;

    private PlayerController player;

    protected override void Start()
    {
        base.Start();
        player = GameManager.Instance.player;
        destroyOnUse = false ;
    }
    public override void OnInteract()
    {
        if (!isEquipped)
        {
            InventorySystem.Instance.AddItem(this);
            gameObject.SetActive(false);
        }
    }

    public override void OnEquip(Transform parent)
    {
        base.OnEquip(parent);
        maskVisual.SetActive(true);
        currentOxygen = oxygenCapacity;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = breathingSound;
        audioSource.loop = true;
        audioSource.Play();

        player.HasGasMask=true;
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        maskVisual.SetActive(false);
        audioSource.Stop();
        player.HasGasMask = false;
        StopAllCoroutines();
    }

}