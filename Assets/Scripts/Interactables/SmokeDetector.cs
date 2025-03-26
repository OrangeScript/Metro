using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SmokeDetector : InteractableObject
{
    [Header("检测设置")]
    public float checkInterval = 0.5f;
    public float detectionRadius = 3f;

    [Header("界面提示")]
    public GameObject warningUI;
    public Text warningText;

    private Coroutine detectionCoroutine;

    protected override void Start()
    {
        base.Start();
        destroyOnUse = false;
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
        detectionCoroutine = StartCoroutine(DetectSmoke());
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        if (detectionCoroutine != null) StopCoroutine(detectionCoroutine);
        warningUI.SetActive(false);
    }

    IEnumerator DetectSmoke()
    {
        while (true)
        {
            SmokeSystem.SmokeLevel highestLevel = SmokeSystem.S.GetSmokeLevelAtPosition(transform.position);
            UpdateWarningUI(highestLevel);
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void UpdateWarningUI(SmokeSystem.SmokeLevel level)
    {
        warningUI.SetActive(level >= SmokeSystem.SmokeLevel.Level2);

        switch (level)
        {
            case SmokeSystem.SmokeLevel.Level2:
                warningText.text = "警告：二级致幻烟雾！";
                break;
            case SmokeSystem.SmokeLevel.Level3:
                warningText.text = "危险：三级昏迷烟雾！";
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
