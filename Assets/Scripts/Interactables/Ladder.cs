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
    [Header("梯子设置")]
    public float extendedHeight = 5f;  // 最大伸展高度
    public float retractHeight = 1f;   // 收缩高度
    public float extendSpeed = 2f;     // 伸缩速度
    private bool isExtended = false;   // 记录是否已伸长
    private bool isPlayerOnLadder = false; // 记录玩家是否在梯子上

    protected override void Start()
    {
        base.Start();
        player = FindObjectOfType<PlayerController>(); // 获取玩家
        destroyOnUse = false;
    }

    public override void OnInteract()
    {
        base.OnInteract();
        Debug.Log("梯子已拾取，存入背包！");
    }
    [SerializeField] private Transform layout;
    protected override void HandleUse()
    {
        base.HandleUse();
        //remove it from inventory
        Drop();
        StartCoroutine(ExtendLadder());
    }

    private void Drop()
    {
        gameObject.transform.position = player.transform.position;
        gameObject.transform.SetParent(layout);
        gameObject.SetActive(true);
    }

    
    private IEnumerator ExtendLadder()
    {
        Debug.Log("梯子正在伸展...");
        float startY = transform.localScale.y;
        float endY = extendedHeight;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            transform.localScale = new Vector3(1, Mathf.Lerp(startY, endY, elapsedTime), 1);
            elapsedTime += Time.deltaTime * extendSpeed;
            yield return null;
        }

        transform.localScale = new Vector3(1, extendedHeight, 1);
        isExtended = true;
        Debug.Log("梯子已伸长！");
    }

    private IEnumerator RetractLadder()
    {
        Debug.Log("梯子正在收缩...");
        float startY = transform.localScale.y;
        float endY = retractHeight;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            transform.localScale = new Vector3(1, Mathf.Lerp(startY, endY, elapsedTime), 1);
            elapsedTime += Time.deltaTime * extendSpeed;
            yield return null;
        }

        transform.localScale = new Vector3(1, retractHeight, 1);
        isExtended = false;
        //UIManager.Instance.ShowInteractionText("[F]收回背包"); // 显示UI提示
    }

    private void Update()
    {
        Debug.Log("isplayeron" + isPlayerOnLadder+isExtended);
        if (isExtended && isPlayerOnLadder)
        {
            HandleClimbing();
        }

        if (isExtended&& Input.GetKeyDown(KeyCode.F))
        {
            CollectLadder();
        }
    }

    private void HandleClimbing()
    {
        if (player == null) return;

        float climbSpeed = 3f;
        if (Input.GetKey(KeyCode.W))
        {
            player.transform.position += new Vector3(0,0,-1) * climbSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            player.transform.position += new Vector3(0,0,1) * climbSpeed * Time.deltaTime;
        }
    }

    private void CollectLadder()
    {
        Debug.Log("梯子已收回背包！");
        InventorySystem.Instance.AddItem(this);
        gameObject.SetActive(false); // 隐藏梯子
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerOnLadder = false;
        }
    }
}
