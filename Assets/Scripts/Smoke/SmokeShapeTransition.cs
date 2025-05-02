using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class SmokeShapeTransition : MonoBehaviour
{
    private ParticleSystem ps;
    private Vector3 originalPosition;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        var shape = ps.shape;
        originalPosition = shape.position;

        // ����������ײ��2D��
        var collision = ps.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;   // ��ײ����Ϊ�볡��������ײ
        collision.mode = ParticleSystemCollisionMode.Collision2D; // 2D ��ײģʽ

        // �����в㷢����ײ
        collision.collidesWith = -1;  // -1 ��ʾ�����в���ײ

        // ������ײ�����Ϊ
        collision.dampen = 1f;         // ��ײ�����Ӽ��٣�1 = ��ȫֹͣ
        collision.bounce = 0f;         // ������
        collision.lifetimeLoss = 1f;   // ��ײ��������������

        StartCoroutine(ChangeShapeOverTime());
    }

    IEnumerator ChangeShapeOverTime()
    {
        yield return new WaitForSeconds(6f);

        // �л�Ϊ Hemisphere
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 1.2f;

        //����ƫ��
        shape.position = originalPosition + new Vector3(0, 0.5f, 0);

        //�޸����Ӳ��������� Hemisphere ״̬�£�
        var main = ps.main;
        main.startSize = 0.3f;

        //�޸� alpha
        Color newColor = main.startColor.color;
        newColor.a = 0.5f;
        main.startColor = newColor;

        yield return new WaitForSeconds(4f); // ��������������ʱ��
        Destroy(gameObject);
    }
}

