using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmokeSystem : MonoBehaviour
{
    public enum SmokeLevel { Level1, Level2, Level3, Sober }
    public static SmokeSystem S { get; private set; }

    void Awake()
    {
        if (S == null) S = this;
        else Destroy(gameObject);
    }

    public SmokeLevel GetSmokeLevelAtPosition(Vector2 position)
    {
        SmokeArea smoke = activeSmoke.Find(s => s.position == position);
        return smoke != null ? smoke.level : SmokeLevel.Level1;
    }
    [System.Serializable]
    public class SmokeArea
    {
        public Vector2 position;
        public SmokeLevel level;
        public int amount;
    }

    public float spreadInterval = 1.0f; // ������ɢ��ʱ����
    public float smokeLifetime = 10f;   // �������ʱ��
    private List<SmokeArea> activeSmoke = new List<SmokeArea>();

    void Start()
    {
        StartCoroutine(SpreadSmoke());
    }

    public void AddSmoke(Vector2 position, SmokeLevel level, int amount)
    {
        SmokeArea existingSmoke = activeSmoke.Find(s => s.position == position);
        if (existingSmoke != null)
        {
            existingSmoke.amount += amount;
            existingSmoke.level = level;
        }
        else
        {
            activeSmoke.Add(new SmokeArea { position = position, level = level, amount = amount });
        }

        Debug.Log($"�� {position} ���� {amount} ��λ�� {level} ������");

        if (level == SmokeLevel.Sober)
        {
            Debug.Log("����������Ч���������״̬��");
            ClearNearbyEffects(position);
        }

        StartCoroutine(RemoveSmokeAfterTime(position, smokeLifetime));
    }


    private IEnumerator RemoveSmokeAfterTime(Vector2 position, float time)
    {
        yield return new WaitForSeconds(time);
        activeSmoke.RemoveAll(s => s.position == position);
        Debug.Log($"������ {position} ��ɢ");
    }

    private IEnumerator SpreadSmoke()
    {
        while (true)
        {
            yield return new WaitForSeconds(spreadInterval);

            List<SmokeArea> newSmoke = new List<SmokeArea>();

            foreach (var smoke in activeSmoke)
            {
                List<Vector2> neighbors = GetValidNeighborPositions(smoke.position);
                foreach (var neighbor in neighbors)
                {
                    SmokeArea neighborSmoke = activeSmoke.Find(s => s.position == neighbor);
                    if (neighborSmoke == null)
                    {
                        newSmoke.Add(new SmokeArea { position = neighbor, level = smoke.level, amount = smoke.amount / 2 });
                    }
                    else if (neighborSmoke.level == smoke.level && neighborSmoke.amount < smoke.amount)
                    {
                        neighborSmoke.amount += smoke.amount / 2;
                    }
                }

                smoke.amount = Mathf.Max(smoke.amount - 1, 0);
            }

            activeSmoke.AddRange(newSmoke);
            activeSmoke.RemoveAll(s => s.amount <= 0);
        }
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
            NPCController npcController = npc.GetComponent<NPCController>();
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
}
