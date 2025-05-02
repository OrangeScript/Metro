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

        // 开启粒子碰撞（2D）
        var collision = ps.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;   // 碰撞类型为与场景物体碰撞
        collision.mode = ParticleSystemCollisionMode.Collision2D; // 2D 碰撞模式

        // 与所有层发生碰撞
        collision.collidesWith = -1;  // -1 表示与所有层碰撞

        // 设置碰撞后的行为
        collision.dampen = 1f;         // 碰撞后粒子减速，1 = 完全停止
        collision.bounce = 0f;         // 不反弹
        collision.lifetimeLoss = 1f;   // 碰撞后立即销毁粒子

        StartCoroutine(ChangeShapeOverTime());
    }

    IEnumerator ChangeShapeOverTime()
    {
        yield return new WaitForSeconds(6f);

        // 切换为 Hemisphere
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 1.2f;

        //向上偏移
        shape.position = originalPosition + new Vector3(0, 0.5f, 0);

        //修改粒子参数（仅在 Hemisphere 状态下）
        var main = ps.main;
        main.startSize = 0.3f;

        //修改 alpha
        Color newColor = main.startColor.color;
        newColor.a = 0.5f;
        main.startColor = newColor;

        yield return new WaitForSeconds(4f); // 根据需求调整这个时间
        Destroy(gameObject);
    }
}

