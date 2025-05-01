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
    public float smokeLifetime = 20f;
    public int initialSmokeAmount = 100;
    public CameraEffectsController cameraEffects;

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

    private Dictionary<Vector3, GameObject> smokeParticles = new Dictionary<Vector3, GameObject>();
    public List<SmokeArea> activeSmoke = new List<SmokeArea>();

    [Header("3D网格设置")]
    public Vector3 gridCellSize = new Vector3(2f, 2f, 2f);
    public int maxSmokePerLevel = 50;

    [Header("3D扩散设置")]
    public float spreadCheckInterval = 2.0f;
    public LayerMask obstacle3DLayers;
    public int spreadAmount = 5;

    private class GridCell3D
    {
        public Dictionary<SmokeLevel, int> smokeLevels = new Dictionary<SmokeLevel, int>();
        public Vector3Int gridCoord;

        public GridCell3D(Vector3Int coord)
        {
            gridCoord = coord;
            foreach (SmokeLevel level in System.Enum.GetValues(typeof(SmokeLevel)))
            {
                smokeLevels[level] = 0;
            }
        }
    }

    private Dictionary<Vector3Int, GridCell3D> gridMap3D = new Dictionary<Vector3Int, GridCell3D>();

    private PlayerController player;

    void Start()
    {
        InitializeGridSystem3D();
        InitializeSceneSmoke();
        StartCoroutine(SpreadSmokeRoutine());
    }

    void Awake()
    {
        if (S == null) S = this;
        else Destroy(gameObject);
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        HandleCharacterEnterSmoke(player, player.transform.position);
    }
    #region 初始化
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
    private void InitializeGridSystem3D()
    {
        for (int x = -10; x <= 10; x++)
        {
            for (int y = -10; y <= 10; y++)
            {
                for (int z = -10; z <= 10; z++)
                {
                    Vector3Int coord = new Vector3Int(x, y, z);
                    gridMap3D[coord] = new GridCell3D(coord);
                }
            }
        }
    }
    #endregion
    #region 烟雾管理
    public void AddSmoke(Vector3 position, SmokeLevel level, int amount, Quaternion rotation = default)
    {
        Vector3Int gridCoord = WorldToGrid(position);
        if (!gridMap3D.ContainsKey(gridCoord))
        {
            gridMap3D[gridCoord] = new GridCell3D(gridCoord);
        }
        Vector2 roundedPos = new Vector2(
        Mathf.Round(position.x * 10) / 10,
        Mathf.Round(position.y * 10) / 10
         );

        if (smokeParticles.TryGetValue(roundedPos, out GameObject existing))
        {
            UpdateExistingSmoke(roundedPos, level, amount);
            return;
        }

        GameObject prefab = GetParticlePrefab(level);
        GameObject particle = Instantiate(prefab, position, rotation);
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
        UpdateSmokeVisual3D(gridCoord, level);
        StartCoroutine(RemoveSmokeAfterTime(position, smokeLifetime));
    }
    private void UpdateSmokeVisual3D(Vector3Int coord, SmokeLevel level)
    {
        Vector3 worldPos = GridToWorld(coord);
        int amount = gridMap3D[coord].smokeLevels[level];

        if (smokeParticles.TryGetValue(worldPos, out GameObject particle))
        {
            if (amount <= 0)
            {
                RemoveSmoke(worldPos);
                return;
            }

            // 更新现有粒子
            //UpdateParticleProperties(particle, level, amount);
        }
        else if (amount > 0)
        {
            // 创建新粒子
            CreateNewParticle3D(worldPos, level, amount);
        }
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

    private void CreateNewParticle3D(Vector3 position, SmokeLevel level, int amount)
    {
        GameObject prefab = GetParticlePrefab(level);
        GameObject particle = Instantiate(prefab, position, Quaternion.identity);

        var ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var emission = ps.emission;
            emission.rateOverTime = amount / 2f; // 根据浓度调整发射率
            ps.Play();
        }

        smokeParticles[position] = particle;
        activeSmoke.Add(new SmokeArea
        {
            smokeObject = particle,
            level = level,
            amount = amount
        });
    }

    private void RemoveSmoke(Vector3 position)
    {
        if (smokeParticles.TryGetValue(position, out GameObject particle))
        {
            activeSmoke.RemoveAll(s => s.smokeObject == particle);
            smokeParticles.Remove(position);
            Destroy(particle);
        }
    }
    #endregion

    #region 烟雾扩散
    private IEnumerator SpreadSmokeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spreadCheckInterval);
            Process3DSpread();
        }
    }

    private void Process3DSpread()
    {
        List<Vector3Int> keys = new List<Vector3Int>(gridMap3D.Keys);

        foreach (var coord in keys)
        {
            GridCell3D cell = gridMap3D[coord];

            // 按等级从高到低处理
            foreach (SmokeLevel level in System.Enum.GetValues(typeof(SmokeLevel)).Cast<SmokeLevel>().Reverse())
            {
                if (cell.smokeLevels[level] <= 0) continue;

                Vector3Int[] directions = {
                Vector3Int.up,
                Vector3Int.down,
                Vector3Int.left,
                Vector3Int.right,
                Vector3Int.forward,
                Vector3Int.back
            };

                foreach (var dir in directions)
                {
                    Vector3Int neighborCoord = coord + dir;
                    if (!gridMap3D.ContainsKey(neighborCoord)) continue;

                    GridCell3D neighbor = gridMap3D[neighborCoord];
                    Vector3 worldPos = GridToWorld(coord);
                    Vector3 neighborPos = GridToWorld(neighborCoord);

                    // 增强障碍物检测
                    if (Physics.CheckBox((worldPos + neighborPos) / 2,
                        gridCellSize / 2,
                        Quaternion.identity,
                        obstacle3DLayers))
                    {
                        continue;
                    }

                    int transfer = CalculateTransferAmount(cell, neighbor, level);
                    if (transfer > 0)
                    {
                        cell.smokeLevels[level] -= transfer;
                        neighbor.smokeLevels[level] += transfer;

                        UpdateSmokeVisual3D(coord, level);
                        UpdateSmokeVisual3D(neighborCoord, level);
                    }
                }
            }
        }
    }

    private int CalculateTransferAmount(GridCell3D source, GridCell3D target, SmokeLevel level)
    {
        return Mathf.Min(
            spreadAmount,
            source.smokeLevels[level],
            maxSmokePerLevel - target.smokeLevels[level]
        );
    }

    private bool Has3DObstacle(Vector3 start, Vector3 end)
    {
        return Physics.Linecast(start, end, out RaycastHit hit, obstacle3DLayers);
    }

    private Vector3Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector3Int(
            Mathf.FloorToInt(worldPos.x / gridCellSize.x),
            Mathf.FloorToInt(worldPos.y / gridCellSize.y),
            Mathf.FloorToInt(worldPos.z / gridCellSize.z)
        );
    }

    private Vector3 GridToWorld(Vector3Int gridCoord)
    {
        return new Vector3(
            gridCoord.x * gridCellSize.x + gridCellSize.x / 2,
            gridCoord.y * gridCellSize.y + gridCellSize.y / 2,
            gridCoord.z * gridCellSize.z + gridCellSize.z / 2
        );
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

        //Debug.Log($"检测位置 {position} 最高等级：{highestLevel} | 检查粒子数：{particlesChecked}");
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
                    cameraEffects.EnterIllusionWorld();
                }
                break;
            case SmokeLevel.Level3:
                {
                    player.BlockMovement(); }
                break;
            case SmokeLevel.Level1:
                if (cameraEffects.isIllusion)
                {
                    cameraEffects.ReturnFromIllusionWorld();
                }
                break;
            case SmokeLevel.Sober:
                if (cameraEffects.isIllusion)
                {
                    cameraEffects.ReturnFromIllusionWorld();
                }
                //TODO:NPC
                break;
        }
    }
    #endregion
}