using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class SmokeSystem : MonoBehaviour
{
    public enum SmokeLevel { Sober,Level1, Level2, Level3 }
    public static SmokeSystem S { get; private set; }

    [Header("烟雾预制体")]
    public GameObject soberParticlePrefab;
    public GameObject level1ParticlePrefab;
    public GameObject level2ParticlePrefab;
    public GameObject level3ParticlePrefab;

    [Header("烟雾监测设置")]
    public float detectionRadius = 20f;
    public float spreadInterval = 1.0f;
    public float smokeLifetime = 10f;
    public int initialSmokeAmount = 100;

    [System.Serializable]
    public class SmokeArea
    {
        public GameObject smokeObject;
        public SmokeLevel level;
        public int amount = 100;
    }

    [System.Serializable]
    public class TagLevelPair
    {
        public string tag;
        public SmokeLevel level;
    }

    [Header("标签设置")]
    public List<TagLevelPair> tagLevelMap = new List<TagLevelPair>
    {
        new TagLevelPair { tag = "SoberSmoke", level = SmokeLevel.Sober },
        new TagLevelPair { tag = "Level1Smoke", level = SmokeLevel.Level1 },
        new TagLevelPair { tag = "Level2Smoke", level = SmokeLevel.Level2 },
        new TagLevelPair { tag = "Level3Smoke", level = SmokeLevel.Level3 }
    };

    private Dictionary<Vector2, GameObject> smokeParticles = new Dictionary<Vector2, GameObject>();
    public List<SmokeArea> activeSmoke = new List<SmokeArea>();

    private PlayerController player;

    void Awake()
    {
        if (S == null) S = this;
        else Destroy(gameObject);
        player = FindObjectOfType<PlayerController>();
    }

    void Start()
    {
        InitializeSceneSmoke();
    }

    private void Update()
    {
        HandleCharacterEnterSmoke(player, player.transform.position);
    }

    private void InitializeSceneSmoke()
    {
        foreach (var pair in tagLevelMap)
        {
            GameObject[] sceneSmokes = GameObject.FindGameObjectsWithTag(pair.tag);
            foreach (GameObject smokeObj in sceneSmokes)
            {
                Vector2 position = smokeObj.transform.position;
                AddSmoke(position, pair.level, initialSmokeAmount, smokeObj.transform.rotation);
                Destroy(smokeObj);
            }
        }
    }

    #region 烟雾管理
    public void AddSmoke(Vector2 position, SmokeLevel level, int amount, Quaternion rotation = default)
    {
        if (smokeParticles.TryGetValue(position, out GameObject existing))
        {
            UpdateExistingSmoke(position, level, amount);
            return;
        }

        GameObject prefab = GetParticlePrefab(level);
        GameObject particle = Instantiate(prefab, position, rotation); // 修改这里
        particle.transform.position = position;
        particle.tag = GetTagFromLevel(level);

        var ps = particle.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        smokeParticles.Add(position, particle);
        activeSmoke.Add(new SmokeArea
        {
            smokeObject = particle,
            level = level,
            amount = amount
        });

        StartCoroutine(RemoveSmokeAfterTime(position, smokeLifetime));
    }

    private void UpdateExistingSmoke(Vector2 position, SmokeLevel newLevel, int addAmount)
    {
        if (smokeParticles.TryGetValue(position, out GameObject particle))
        {
            var smoke = activeSmoke.Find(s => s.smokeObject == particle);
            if (smoke != null)
            {
                smoke.amount += addAmount;
                if (newLevel > smoke.level)
                {
                    smoke.level = newLevel;
                    particle.tag = GetTagFromLevel(newLevel);
                }
            }
        }
    }

    private IEnumerator RemoveSmokeAfterTime(Vector2 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (smokeParticles.TryGetValue(position, out GameObject particle))
        {
            activeSmoke.RemoveAll(s => s.smokeObject == particle);
            smokeParticles.Remove(position);
            Destroy(particle);
        }
    }
    #endregion

    #region 监测系统
    public SmokeLevel DetectHighestSmokeLevel(Vector2 position)
    {
        SmokeLevel highestLevel = SmokeLevel.Level1;
        int particlesChecked = 0;

        foreach (var particleObj in smokeParticles.Values.ToList())
        {
            if (particleObj == null)
            {
                CleanupDictionary();
                continue;
            }

            var ps = particleObj.GetComponent<ParticleSystem>();
            if (ps == null) continue;

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
            int count = ps.GetParticles(particles);
            particlesChecked += count;

            Vector3 psPos = ps.transform.position;

            for (int i = 0; i < count; i++)
            {
                Vector3 particlePos = psPos + particles[i].position;
                if (Vector2.Distance(particlePos, position) > detectionRadius) continue;

                var currentLevel = GetLevelFromTag(particleObj.tag);
                if (currentLevel > highestLevel)
                {
                    highestLevel = currentLevel;
                    // 如果已经找到最高等级可提前退出
                    if (highestLevel == SmokeLevel.Level3) break;
                }
            }

            // 如果已经找到最高等级可提前终止检测
            if (highestLevel == SmokeLevel.Level3) break;
        }

        Debug.Log($"检测位置 {position} 最高等级：{highestLevel} | 检查粒子数：{particlesChecked}");
        return highestLevel;
    }

    private void CleanupDictionary()
    {
        var nullEntries = smokeParticles.Where(kvp => kvp.Value == null).ToList();
        foreach (var entry in nullEntries)
        {
            smokeParticles.Remove(entry.Key);
        }
    }
    #endregion

    #region 工具方法
    private string GetTagFromLevel(SmokeLevel level)
    {
        var pair = tagLevelMap.FirstOrDefault(p => p.level == level);
        return pair != null ? pair.tag : "Level1Smoke";
    }

    private SmokeLevel GetLevelFromTag(string tag)
    {
        var pair = tagLevelMap.FirstOrDefault(p => p.tag == tag);
        return pair != null ? pair.level : SmokeLevel.Level1;
    }

    private GameObject GetParticlePrefab(SmokeLevel level)
    {
        return level switch
        {
            SmokeLevel.Sober => soberParticlePrefab,
            SmokeLevel.Level1 => level1ParticlePrefab,
            SmokeLevel.Level2 => level2ParticlePrefab,
            SmokeLevel.Level3 => level3ParticlePrefab,
            _ => level1ParticlePrefab
        };
    }
    #endregion

    #region 调试工具
    void OnDrawGizmosSelected()
    {
        foreach (var entry in smokeParticles)
        {
            if (entry.Value == null) continue;

            Gizmos.color = GetLevelColor(GetLevelFromTag(entry.Value.tag));
            Gizmos.DrawWireSphere(entry.Value.transform.position, detectionRadius);
        }
    }

    private Color GetLevelColor(SmokeLevel level)
    {
        return level switch
        {
            SmokeLevel.Sober => Color.gray,
            SmokeLevel.Level1 => Color.green,
            SmokeLevel.Level2 => Color.yellow,
            SmokeLevel.Level3 => Color.red,
            _ => Color.white
        };
    }
    #endregion

    #region 交互设置
    public void HandleCharacterEnterSmoke(PlayerController player, Vector2 position)
    {
        if (player == null)
        {
            Debug.LogError("Can't find the player");
        }
        SmokeLevel level = DetectHighestSmokeLevel(position);

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
    #endregion
}