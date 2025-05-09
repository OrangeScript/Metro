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
                UIManager.Instance.ShowMessage("烟雾太浓，你无法前进");
            }
            else
            {
                UIManager.Instance.ShowMessage("危险烟雾，请佩戴防毒面具");
            }
        }
            ApplyState();
    }

    private void ApplyState()
    {
        if (col2D != null)
        {
            Debug.Log("空气墙消失!");
            col2D.enabled = airWallEnabled;
        }

    }

    void Update()
    {
        if (!GameManager.Instance.isGameStarted)
            return;
        if (!ps.IsAlive(true))  // 粒子全部死亡（包括子系统）
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
        Debug.Log("空气墙碰撞到：" + collision.gameObject.name);
    }

}
