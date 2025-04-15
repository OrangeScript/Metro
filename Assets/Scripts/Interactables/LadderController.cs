using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LadderController : MonoBehaviour
{

    public float extendedHeight = 5f;  // 最大伸展高度
    public float retractHeight = 1f;   // 收缩高度
    public float extendSpeed = 2f;     // 伸缩速度
    private bool isExtended = false;   // 记录是否已伸长
    private bool isPlayerOnLadder = false; // 记录玩家是否在梯子上
    private bool canBeCollected=false;
    [SerializeField]private PlayerController player;
    
    private void Start()
    {
        player = PlayerManager.instance.player;
        StartCoroutine(ExtendLadder());
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
        canBeCollected = true;
        //UIManager.Instance.ShowInteractionText("[F]收回背包"); // 显示UI提示
    }

    private void Update()
    {
            
        if (isExtended && isPlayerOnLadder)
        {
            HandleClimbing();
        }

        if (canBeCollected && Input.GetKeyDown(KeyCode.F))
        {
            CollectLadder();
        }

        if (player.transform.position.z == 0/**/&& Input.GetKeyDown(KeyCode.F)&&isExtended)
        {
            isExtended = false;
            StartCoroutine(RetractLadder());
        }
    }

    /// <TODO>
    /// at the same time, player script should turn into a climb state which will restrict 
    /// movement in other axis;
    /// </summary>
    private float targetZ = 0f;
    float climbSpeed = 3f;
    private void HandleClimbing()
    {
        if (player == null) return;
        

        float currentZ = -player.transform.position.z;

        // 按W上升
        if (Input.GetKey(KeyCode.W) && currentZ < extendedHeight)
        {
            targetZ = Mathf.Min(extendedHeight, currentZ + climbSpeed * Time.deltaTime);
        }
        // 按S下降
        else if (Input.GetKey(KeyCode.S) && currentZ > 0)
        {
            targetZ = Mathf.Max(0f, currentZ - climbSpeed * Time.deltaTime);
        }
        UpdateZPosition();
    }

    private void UpdateZPosition()
    {
        if (player == null) return;

        Vector3 pos = player.transform.position;
        float newZ = Mathf.MoveTowards(pos.z, -targetZ, climbSpeed * Time.deltaTime);
        player.transform.position = new Vector3(pos.x, pos.y, newZ);
    }

    private void CollectLadder()
    {
        Debug.Log("梯子已收回背包！");
        //InventorySystem.Instance.AddItem();
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerController>() != null)
        {
            isPlayerOnLadder = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            isPlayerOnLadder = false;
            player.transform.position=new Vector3(player.transform.position.x, player.transform.position.y, 0f);
        }
    }
}
