using UnityEngine;

public class AirWallController : MonoBehaviour
{
    public bool airWallEnabled = true;

    private Collider2D col2D;
    public bool destroyAfterParticleGone = true;
    private ParticleSystem ps;
    private PlayerController player;

    void Awake()
    {
        player=FindObjectOfType<PlayerController>();
        col2D = GetComponent<Collider2D>();
        ps = GetComponentInChildren<ParticleSystem>();
        ApplyState();
    }

    public void SetMaskState(bool hasMask)
    {
        airWallEnabled = !hasMask;
        if (hasMask == false)
        {
            if (!(player.equippedItem is SmokeDetector smokeDetector))
            {
                UIManager.Instance.ShowMessage("����̫Ũ�����޷�ǰ��");
            }
            else
            {
                UIManager.Instance.ShowMessage("Σ������������������");
            }
        }
            ApplyState();
    }

    private void ApplyState()
    {
        if (col2D != null)
        {
            Debug.Log("����ǽ��ʧ!");
            col2D.enabled = airWallEnabled;
        }

    }

    void Update()
    {
        if (!GameManager.Instance.isGameStarted)
            return;
        if (!ps.IsAlive(true))  // ����ȫ��������������ϵͳ��
        {
            if (col2D != null)
                col2D.enabled = false;

            if (destroyAfterParticleGone)
                Destroy(gameObject);  

            enabled = false;  
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("����ǽ��ײ����" + collision.gameObject.name);
    }

}
