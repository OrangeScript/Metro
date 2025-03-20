using System.Collections;
using UnityEngine;

public class Ladder : InteractableObject
{
    [Header("��������")]
    public float extendedHeight = 5f;  // �����չ�߶�
    public float retractHeight = 1f;   // �����߶�
    public float extendSpeed = 2f;     // �����ٶ�
    private bool isExtended = false;   // ��¼�Ƿ����쳤
    private bool isPlayerOnLadder = false; // ��¼����Ƿ���������
    private PlayerController player;

    protected override void Start()
    {
        base.Start();
        player = FindObjectOfType<PlayerController>(); // ��ȡ���
    }

    public override void OnInteract()
    {
        base.OnInteract();
        Debug.Log("������ʰȡ�����뱳����");
    }

    public override void UseItem()
    {
        if (isEquipped)
        {
            if (isExtended)
            {
                StartCoroutine(RetractLadder());
            }
            else
            {
                StartCoroutine(ExtendLadder());
            }
        }
    }

    private IEnumerator ExtendLadder()
    {
        Debug.Log("����������չ...");
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
        Debug.Log("�������쳤��");
    }

    private IEnumerator RetractLadder()
    {
        Debug.Log("������������...");
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
        //UIManager.Instance.ShowInteractionText("[F]�ջر���"); // ��ʾUI��ʾ
    }

    private void Update()
    {
        if (isExtended && isPlayerOnLadder)
        {
            HandleClimbing();
        }

        if (isExtended && Input.GetKeyDown(KeyCode.F))
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
            player.transform.position += Vector3.up * climbSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            player.transform.position += Vector3.down * climbSpeed * Time.deltaTime;
        }
    }

    private void CollectLadder()
    {
        Debug.Log("�������ջر�����");
        InventorySystem.Instance.AddItem(this);
        gameObject.SetActive(false); // ��������
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
