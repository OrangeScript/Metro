using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MazePlayer : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D collder;

    public float speed = 5f;


    private List<Vector2> mousePositions = new List<Vector2>();
    private bool isTracking = false;
    [SerializeField] private float minDistance;
    [SerializeField] private Camera mazeCamera;
    private List<SpriteRenderer> SRs;

    public bool failTheMaze = false;
    public bool winTheMaze = false;

    private bool isLocked = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collder = GetComponent<BoxCollider2D>();
        SRs = new List<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isLocked)
        {
            MouseInput();
        }
    }

    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartTracing();
        }
        if (isTracking && Input.GetMouseButton(0))
        {
            ContinueTracing();
        }
        if (Input.GetMouseButtonUp(0))
        {
            EndTracing();
        }
    }

    private void EndTracing()
    {
        isTracking = false;
        Debug.Log("Stop tracking mouse positions");
        StartCoroutine(ResetRun());
    }

    private void ContinueTracing()
    {
        Vector2 currentPos = Input.mousePosition;
        Vector2 lastPos = mousePositions[mousePositions.Count - 1];
        if (Vector2.Distance(currentPos, lastPos) > minDistance)
        {
            mousePositions.Add(currentPos);
            Debug.Log("Tracking mouse positions");
            CheckForSpriteInteraction(currentPos);
        }
    }

    private void StartTracing()
    {
        mousePositions.Clear();


        Vector2 currentPos = Input.mousePosition;
        mousePositions.Add(currentPos);

        CheckForSpriteInteraction(currentPos);
        isTracking = true;
        Debug.Log("Start tracking mouse positions");
    }

    private void CheckForSpriteInteraction(Vector2 screenPosition)
    {
        Vector3 worldPos = mazeCamera.ScreenToWorldPoint(screenPosition);
        Vector2 worldPoint2D = new Vector2(worldPos.x, worldPos.y);
        int mazeLayerMask = 1 << LayerMask.NameToLayer("Maze");
        RaycastHit2D hit = Physics2D.Raycast(worldPoint2D, Vector2.zero,0f,mazeLayerMask);

        if (hit.collider != null && hit.collider.CompareTag("MazeExit"))
        {
            isLocked = true;
            Debug.Log("到达迷宫出口");
            winTheMaze = true;
        }
        //if (hit.collider == null)
        //{
        //    Debug.Log("Raycast hit nothing!");
        //}
        //else
        //{
        //    Debug.Log($"Hit: {hit.collider.name}, Tag: {hit.collider.tag}");
        //}
        if (hit.collider != null && hit.collider.CompareTag("MazePath"))
        {
            Debug.Log("a");
            hit.collider.GetComponent<SpriteRenderer>().color = Color.blue;
            if (!SRs.Contains(hit.collider.GetComponent<SpriteRenderer>()))
                SRs.Add(hit.collider.GetComponent<SpriteRenderer>());
        }
        else if (hit.collider != null && hit.collider.CompareTag("MazeWall"))
        {
            Debug.Log("与墙壁碰撞");
            isLocked = true;
            StartCoroutine(ResetRunWithHitWall(hit.collider.GetComponent<SpriteRenderer>()));
        }

    }
    private IEnumerator ResetRunWithHitWall(SpriteRenderer sr)
    {
        sr.color = Color.blue;
        yield return new WaitForSeconds(.1f);
        StartCoroutine(ResetRun());
        sr.color = Color.clear;
    }
    private IEnumerator ResetColor(SpriteRenderer sr)
    {
        yield return new WaitForSeconds(0.5f);
        sr.color = Color.clear;
    }
    private IEnumerator ResetRun()
    {

        for (int i = 0; i < SRs.Count; i++)
        {
            Debug.Log(SRs.Count);
            SRs[i].color = Color.clear;
            yield return new WaitForSeconds(0.1f);
        }
        isLocked = false;
        failTheMaze = true;
    }

    // abadoned
    //private void MoveCharacter()
    //{
    //    // 获取玩家输入
    //    float moveX = Input.GetAxis("Horizontal");
    //    float moveY = Input.GetAxis("Vertical");
    //    // 计算移动方向sdaw
    //    Vector2 moveDirection = new Vector2(moveX, moveY);
    //    // 移动玩家
    //    if (Input.GetKey(KeyCode.LeftShift))
    //    {
    //        rb.velocity = moveDirection * speed * 2; // 按住左Shift加速
    //    }
    //    else
    //    {
    //        rb.velocity = moveDirection * speed; // 恢复正常速度
    //    }
    //}

}
