using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.ParticleSystem;

public class SmokeSystem : MonoBehaviour
{
    public enum SmokeLevel { Level1, Level2, Level3, Sober }
    public static SmokeSystem S { get; private set; }

    [Header("��ͬ�ȼ����������Ԥ��")]
    public GameObject level1ParticlePrefab;
    public GameObject level2ParticlePrefab;
    public GameObject level3ParticlePrefab;
    public GameObject soberParticlePrefab;
    private Dictionary<Vector2, GameObject> smokeParticles = new Dictionary<Vector2, GameObject>();

    public float detectionRadius=20f;
    void Awake()
    {
        if (S == null) S = this;
        else Destroy(gameObject);
    }

    [System.Serializable]
    public class SmokeArea
    {
        public GameObject smokeObject;
        public SmokeLevel level;
        public int amount=100;
    }

    public float spreadInterval = 1.0f; // ������ɢ��ʱ����
    public float smokeLifetime = 10f;   // �������ʱ��
    public List<SmokeArea> activeSmoke = new List<SmokeArea>();


    void Start()
    {
        //StartCoroutine(SpreadSmoke());
    }

    private void Update()
    {
        //Debug.Log($"��ǰ����������{activeSmoke.Count}");
        //TestParticles();
    }


    public SmokeLevel GetSmokeLevelAtPosition(Vector2 position)
    {
        foreach (var s in activeSmoke)
        {
            if (s.smokeObject == null) continue;

            // ʹ�þ����жϴ��澫ȷ����Ƚ�
            float distance = Vector2.Distance(s.smokeObject.transform.position, position);
            if (distance < 0.1f) // ������ֵ����ʵ�����
            {
                Debug.Log($"�ҵ��������� {position}, ʵ��λ�� {s.smokeObject.transform.position}");
                return s.level;
            }
        }
        return SmokeLevel.Level1;
    }
    public void AddSmoke(Vector2 position, SmokeLevel level, int amount)
    {
        SmokeArea existingSmoke = activeSmoke.Find(s => s.smokeObject != null && (Vector2)s.smokeObject.transform.position == position);
        if (existingSmoke != null)
        {
            existingSmoke.amount += amount;
            existingSmoke.level = level;
        }
        else
        {
            GameObject particle = Instantiate(GetParticlePrefab(level), position, Quaternion.identity);
            particle.tag = GetTagFromLevel(level);
            activeSmoke.Add(new SmokeArea { smokeObject = particle, level = level, amount = amount });

            smokeParticles[position] = particle; // ��������

            Debug.Log($"�� {position} ���� {amount} ��λ�� {level} ������");
        }

        if (level == SmokeLevel.Sober)
        {
            Debug.Log("����������Ч���������״̬��");
            ClearNearbyEffects(position);
        }

        StartCoroutine(RemoveSmokeAfterTime(position, smokeLifetime));
    }
    private string GetTagFromLevel(SmokeSystem.SmokeLevel level)
    {
        switch (level)
        {
            case SmokeSystem.SmokeLevel.Level1:
                return "Level1Smoke";
            case SmokeSystem.SmokeLevel.Level2:
                return "Level2Smoke";
            case SmokeSystem.SmokeLevel.Level3:
                return "Level3Smoke";
            case SmokeSystem.SmokeLevel.Sober:
                return "SoberSmoke";
            default:
                return "Level1Smoke";
        }
    }
    private GameObject GetParticlePrefab(SmokeLevel level)
    {
        switch (level)
        {
            case SmokeLevel.Level1: return level1ParticlePrefab;
            case SmokeLevel.Level2: return level2ParticlePrefab;
            case SmokeLevel.Level3: return level3ParticlePrefab;
            case SmokeLevel.Sober: return soberParticlePrefab;
            default: return level1ParticlePrefab;
        }
    }

    private IEnumerator RemoveSmokeAfterTime(Vector2 position, float time)
    {
        yield return new WaitForSeconds(time);
        activeSmoke.RemoveAll(s => s.smokeObject != null && (Vector2)s.smokeObject.transform.position == position);

        if (smokeParticles.TryGetValue(position, out GameObject particle))
        {
            Destroy(particle);
            smokeParticles.Remove(position); // ��ȷ�Ƴ�
        }
    }

    private IEnumerator SpreadSmoke()
    {
        while (true)
        {
            yield return new WaitForSeconds(spreadInterval);
            Debug.Log("Spreading smoke...");
            Debug.Log($"��ǰ����������{activeSmoke.Count}");

            List<SmokeArea> currentSmokeSnapshot = new List<SmokeArea>(activeSmoke);

            foreach (var smoke in currentSmokeSnapshot)
            {
                if (smoke.smokeObject == null) continue;

                Vector2 position = smoke.smokeObject.transform.position;
                List<Vector2> neighbors = GetValidNeighborPositions(position);
                foreach (var neighbor in neighbors)
                {
                    SmokeArea neighborSmoke = activeSmoke.Find(s => s.smokeObject != null && (Vector2)s.smokeObject.transform.position == neighbor);
                    if (neighborSmoke == null)
                    {
                        AddSmoke(neighbor, smoke.level, smoke.amount / 2);
                        Debug.Log($"Spread smoke from {position} to {neighbor}.");
                    }
                    else if (neighborSmoke.level == smoke.level && neighborSmoke.amount < smoke.amount)
                    {
                        neighborSmoke.amount += smoke.amount / 2;
                        Debug.Log($"Increased smoke amount at {neighbor} to {neighborSmoke.amount}.");
                    }
                }

                smoke.amount = Mathf.Max(smoke.amount - 1, 0);
            }

            activeSmoke.RemoveAll(s => s.amount <= 0 || s.smokeObject == null);
        }
    }



    public SmokeLevel GetSmokeLevelFromTag(Vector2 position)
    {
        SmokeLevel highestLevel = SmokeLevel.Level1;
        int totalParticlesChecked = 0;
        int totalParticlesHit = 0;

        foreach (var particleObj in smokeParticles.Values)
        {
            var ps = particleObj.GetComponent<ParticleSystem>();

            // ��ȡ�������ݣ��Ż��ڴ���䣩
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
            int count = ps.GetParticles(particles);
            totalParticlesChecked += count;

            // ��ȡ����ϵͳ��������
            Vector3 psWorldPos = ps.transform.position;

            for (int i = 0; i < count; i++)
            {
                // ���������������
                Vector3 particleWorldPos = psWorldPos + particles[i].position;
                float distance = Vector2.Distance(particleWorldPos, position);

                if (distance < detectionRadius)
                {
                    totalParticlesHit++;
                    var currentLevel = GetLevelFromTag(particleObj.tag);
                    if (currentLevel > highestLevel)
                    {
                        highestLevel = currentLevel;
                    }
                }
            }
        }

        Debug.Log($"���λ�� {position}������������{totalParticlesChecked}��������������{totalParticlesHit}����ߵȼ���{highestLevel}");
        return highestLevel;
    }

    void TestParticles()
    {
        var go = Instantiate(level2ParticlePrefab, Vector2.zero, Quaternion.identity);
        var ps = go.GetComponent<ParticleSystem>();
        ps.Play();

        StartCoroutine(DebugParticleCount(ps));
    }

    IEnumerator DebugParticleCount(ParticleSystem ps)
    {
        yield return new WaitForSeconds(1f);
        var particles = new ParticleSystem.Particle[ps.main.maxParticles];
        int count = ps.GetParticles(particles);
        Debug.Log($"��������������{count}");
    }


    private SmokeLevel GetLevelFromTag(string tag)
    {
        return tag switch
        {
            "Level1Smoke" => SmokeLevel.Level1,
            "Level2Smoke" => SmokeLevel.Level2,
            "Level3Smoke" => SmokeLevel.Level3,
            "SoberSmoke" => SmokeLevel.Sober,
            _ => SmokeLevel.Level1
        };
    }

    private List<Vector2> GetValidNeighborPositions(Vector2 position)
    {
        List<Vector2> neighbors = new List<Vector2>
        {
            position + Vector2.up,
            position + Vector2.down,
            position + Vector2.left,
            position + Vector2.right
        };

        neighbors.RemoveAll(pos => !CanSmokePassThrough(pos));
        return neighbors;
    }

    private bool CanSmokePassThrough(Vector2 position)
    {
        Collider2D door = Physics2D.OverlapPoint(position, LayerMask.GetMask("Door"));
        if (door != null)
        {
            MetroDoor metrodoor = door.GetComponent<MetroDoor>();
            if (metrodoor != null && (metrodoor.currentState == MetroDoor.DoorState.Open))
            {
                return true;
            }
        }

        Collider2D wall = Physics2D.OverlapPoint(position, LayerMask.GetMask("Wall"));
        return wall == null;
    }

    private void ClearNearbyEffects(Vector2 position)
    {
        Collider2D[] npcs = Physics2D.OverlapCircleAll(position, 2f, LayerMask.GetMask("NPC"));
        foreach (var npc in npcs)
        {
            NPC npcController = npc.GetComponent<NPC>();
            if (npcController != null)
            {
                npcController.RecoverFromEffects();
            }
        }
    }

    public void HandleCharacterEnterSmoke(PlayerController player, Vector2 position)
    {
        SmokeLevel level = GetSmokeLevelAtPosition(position);

        switch (level)
        {
            case SmokeLevel.Level2:
                if (!player.equippedMask)
                {
                    player.EnterIllusionWorld();
                }
                break;
            case SmokeLevel.Level3:
                if (!player.equippedMask)
                {
                    player.BlockMovement();
                }
                break;
            case SmokeLevel.Sober:
                //TODO:NPC
                break;
        }
    }

    void ValidateParticleCollision()
    {
        var prefabs = new[] { level1ParticlePrefab, level2ParticlePrefab, level3ParticlePrefab };
        foreach (var prefab in prefabs)
        {
            var ps = prefab.GetComponent<ParticleSystem>();
            if (!ps.collision.enabled)
            {
                Debug.LogError($"Ԥ���� {prefab.name} δ������ײ��");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (var smoke in activeSmoke)
        {
            if (smoke.smokeObject != null)
            {
                Gizmos.DrawWireSphere(smoke.smokeObject.transform.position, detectionRadius);
            }
        }
    }
}
