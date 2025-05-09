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
        if(hasMask==false) UIManager.Instance.ShowMessage("����̫Ũ�����޷�ǰ����");
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
