using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InteractableObject item;
    public Image itemIcon;
    public Image backgroundImage;
    public Text itemDescriptionText;

    private void Start()
    {
        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = "";
        }
    }

    public void LoadItem(InteractableObject targetItem)
    {
        if (targetItem == null)
        {
            Debug.LogWarning("尝试加载空物品到物品槽：" + gameObject.name);
            return;
        }

        item = targetItem;

        if (item.itemIcon != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = true;
        }

        backgroundImage.enabled = true;

        // 更新物品描述
        UpdateItemDescription();
    }

    public void UnloadItem()
    {
        if (item == null)
        {
            Debug.LogWarning("尝试从空物品槽中卸载物品：" + gameObject.name);
            return;
        }

        item = null;

        itemIcon.enabled = false;
        backgroundImage.enabled = false;
        itemDescriptionText.text = " ";
    }

    public InteractableObject GetItem()
    {
        return item;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("鼠标进入：" + gameObject.name);
        InventorySystem.Instance.SetHoveredSlot(this);
        if (item != null)
        {
            TooltipManager.Instance.Show(item.GetDescription());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("鼠标移出：" + gameObject.name);
        InventorySystem.Instance.SetHoveredSlot(null);

        TooltipManager.Instance.Hide();
    }

    private void UpdateItemDescription()
    {
        if (itemDescriptionText == null) return;
        itemDescriptionText.text = item != null ? item.GetDescription() : "未知物品";
    }
}
