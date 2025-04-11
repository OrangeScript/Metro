using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SmokeDetector : InteractableObject
{
    [Header("检测设置")]
    public float checkInterval = 0.5f;

    [Header("界面提示")]
    public GameObject warningUI;
    public Text warningText;

    private Coroutine detectionCoroutine;


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
            SmokeSystem.SmokeLevel highestLevel = SmokeSystem.S.DetectHighestSmokeLevel(transform.position);
            Debug.Log($"检测到烟雾等级: {highestLevel} 在位置: {transform.position}");
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
                warningText.text = "危险烟雾，请佩戴防毒面具";
                break;
            case SmokeSystem.SmokeLevel.Level3:
                warningText.text = "危险烟雾，请佩戴防毒面具";
                break;
        }
    }
}
