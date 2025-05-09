using UnityEngine;
using static UnityEditor.Progress;

public abstract class InteractableObject : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    protected bool isEquipped = false;

    protected PlayerController player;
    public string itemDescription;

    public enum CarryType { None, Mask, NPC, Item }
    public enum UseTrigger { KeyF,RightClick,OnEquip}

    [Header("Я������")]
    public CarryType carryType = CarryType.None;
    private SpriteRenderer spriteRenderer;


    [Header("��Ʒ����")]
    public bool destroyOnUse = false;
    public string poolTag = "Items";
    public GameObject prefabReference;
    public UseTrigger useTrigger;
    public bool isContinuousUse = false;
    //[SerializeField] protected Transform ground;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name} ȱ�� SpriteRenderer �����");
            enabled = false; 
        }
    }
    protected virtual void Start()
    {
        player = InventorySystem.Instance?.GetPlayer();
        if (player == null)
        {
            Debug.LogWarning("PlayerController δ��ȷ���ã���������ʧ�ܣ�");
        }
    }

    public virtual void OnInteract()
    {
        if (InventorySystem.Instance.AddItem(this))
        {
            if (InventorySystem.Instance.isFirstAdd == false)
            {
                InventorySystem.Instance.isFirstAdd = true;
                InventorySystem.Instance.ShowAddTips();
            }
            gameObject.SetActive(false); // ���õ�ǰʵ��
            player.nearestInteractable = null;
        }
    }

    public virtual void OnEquip(Transform parent)
    {
        if (isEquipped) return;
        isEquipped = true;
        transform.SetParent(parent);
        transform.localPosition = Vector2.zero;
        transform.localRotation = Quaternion.identity;
        HandleEquip();
    }

    public virtual void OnUnequip()
    {
        if (!isEquipped) return;

        isEquipped = false;
        gameObject.SetActive(false);
        HandleUnequip();
    }

    public virtual void UseItem()
    {
        if (!isEquipped)
        {
            Debug.LogWarning($"δװ�� {itemName}���޷�ʹ�ã�");
            return;
        }

        HandleUse();

        if (destroyOnUse)
        {
            Debug.Log("����һ������Ʒ");

            if (player.equippedItem == this)
                player.equippedItem = null;
            if (player.equippedMask == this)
                player.equippedMask = null;
            Destroy(gameObject);
            InventorySystem.Instance.UpdateEquippedUI();
        }
        else if(!isContinuousUse)
        {
            Debug.Log("��һ������Ʒ�ɼ���ʹ��");
        }
    }


    protected virtual void HandleEquip() { }
    protected virtual void HandleUnequip() { }
    protected virtual void HandleUse() { }

    public void SetEquippedLayer()
    {
        spriteRenderer.sortingLayerName = "AboveCharacter";
    }

    public void ResetLayer()
    {
        spriteRenderer.sortingLayerName = "Interactables";
    }
    public string GetDescription()
    {
        return $"����: {itemName}\n����: {itemDescription}";
    }

    void OnDrawGizmos()
    {
        if (this.gameObject.activeInHierarchy) // ȷ����Ʒ���ڻ״̬
        {
            Gizmos.color = Color.red; // ������ɫΪ��ɫ
            Gizmos.DrawSphere(transform.position, 0.2f); // ����Ʒλ�û�������
        }
    }

}
