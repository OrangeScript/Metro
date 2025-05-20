using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class SmokeShapeTransition : MonoBehaviour
{
    private ParticleSystem ps;
    private Vector3 originalPosition;
    [SerializeField] private float shapeChangeDelay = 6f;   
    [SerializeField] private float destroyDelay = 4f;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        var shape = ps.shape;
        originalPosition = shape.position;       
        StartCoroutine(ChangeShapeOverTime());
    }

    IEnumerator ChangeShapeOverTime()
    {
        yield return new WaitForSeconds(shapeChangeDelay);

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
        yield return new WaitForSeconds(destroyDelay); 
        Destroy(gameObject);
    }
}

