using UnityEngine;

public class AirWallController : MonoBehaviour
{
    public bool airWallEnabled = true;

    private Collider2D col2D;
    public bool destroyAfterParticleGone = true;
    private ParticleSystem ps;

    void Awake()
    {
        col2D = GetComponent<Collider2D>();
        ps = GetComponentInChildren<ParticleSystem>();
        ApplyState();
    }

    public void SetMaskState(bool hasMask)
    {
        airWallEnabled = !hasMask; 
        ApplyState();
    }

    private void ApplyState()
    {
        if (col2D != null)
        {
            col2D.enabled = airWallEnabled;
        }

    }

    void Update()
    {
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
