using System.Collections;
using UnityEngine;

public class Ladder : InteractableObject
{
    //when using the ladder, the ladder would be thrown on the ground and leave the package

    //I divide it into such procedure:
    //First: pick it up
    /* Second: drop it on the ground and it lengthen automatically
     * Third: interact with it to climb up
     * 
     */
    [Header("��������")]
    [SerializeField] private GameObject ladderPrefab;

    protected override void Start()
    {
        base.Start();
        player = FindObjectOfType<PlayerController>(); // ��ȡ���
        destroyOnUse = false;
    }

    public override void OnInteract()
    {
        base.OnInteract();
        Debug.Log("������ʰȡ�����뱳����");
    }
    protected override void HandleUse()
    {
        base.HandleUse();
        //remove it from inventory
        Instantiate(ladderPrefab,player.transform.position,transform.rotation,ground);
        InventorySystem.Instance.UnequipItem(this);
        InventorySystem.Instance.RemoveItem(this);
    }

    
    
    

    
}
