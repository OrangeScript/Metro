using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Settings")]
    public float cellSize = 1.0f; // ÿ�����Ӷ�Ӧ�����絥λ
    public int padding = 2; // �Թ��߽�����

    [Header("References")]
    public MazeRenderer mazeRenderer;
    

    private int[,] maze;
    public Vector3 start;
    public Vector3 end;

    [Header("Maze Settings")]
    public int width;  // ����Ϊ����
    public int height; // ����Ϊ����

    private Vector2Int startCell;
    private Vector2Int endCell;


    Vector2Int FindRandomDeadEnd()
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        // �������п��ܵ�·���㣨������Ե��
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (maze[x, y] == 1) // �����·��
                {
                    Vector2Int cell = new Vector2Int(x, y);
                    if (IsDeadEnd(cell))
                    {
                        deadEnds.Add(cell);
                    }
                }
            }
        }

        // ���û���ҵ���·���򷵻���㣨������㣩
        if (deadEnds.Count == 0)
        {
            Debug.LogWarning("No dead ends found in maze, using start cell");
            return startCell;
        }

        // ���ѡ��һ����·
        return deadEnds[Random.Range(0, deadEnds.Count)];
    }

    bool IsDeadEnd(Vector2Int cell)
    {
        int openCount = 0;

        // ����ĸ�����
        Vector2Int[] directions = {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = cell + dir;
            if (IsInBounds(neighbor) && maze[neighbor.x, neighbor.y] == 1)
            {
                openCount++;
                if (openCount > 1) // ����г���һ�����ڣ�������·
                    return false;
            }
        }

        return openCount == 1; // ֻ��һ�����ڵ�����·
    }
    public MazePlayer GenerateMaze()
    {
        maze = new int[width, height];
        InitializeMaze();

        startCell = new Vector2Int(Random.Range(1,width-1), Random.Range(1,height-1));
        maze[startCell.x, startCell.y] = 1;

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(startCell);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                stack.Push(current);

                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
                Vector2Int wall = current + (chosen - current) / 2;

                maze[wall.x, wall.y] = 1;
                maze[chosen.x, chosen.y] = 1;

                stack.Push(chosen);
            }
        }
        while ((endCell=FindRandomDeadEnd() )== startCell)
        {
        }
        mazeRenderer.DrawMaze(maze);
        float offsetX = -(width * cellSize) / 2f + cellSize / 2f;
        float offsetZ = -(height * cellSize) / 2f + cellSize / 2f;

        Vector3 startPos = new Vector3(startCell.x * cellSize + offsetX, startCell.y * cellSize + offsetZ,0);

        Vector3 endPos = new Vector3(endCell.x * cellSize + offsetX, endCell.y * cellSize + offsetZ,0);
        mazeRenderer.RenderStartAndEnd(startPos, endPos);
        GameObject p = Instantiate(MazeManager.instance.playerPrefab, startPos, Quaternion.identity);
        return p.GetComponent<MazePlayer>();
    }

    void InitializeMaze()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 0; // 0 ��ʾǽ��1 ��ʾ·��
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(0, 2),
            new Vector2Int(2, 0),
            new Vector2Int(0, -2),
            new Vector2Int(-2, 0)
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = cell + dir;
            if (IsInBounds(neighbor) && maze[neighbor.x, neighbor.y] == 0)
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x > 0 && pos.x < width - 1 && pos.y > 0 && pos.y < height - 1;
    }

    Vector2Int FindFurthestCell(Vector2Int from)
    {
        Vector2Int furthest = from;
        int maxDistance = -1;

        for (int x = 1; x < width; x += 2)
        {
            for (int y = 1; y < height; y += 2)
            {
                if (maze[x, y] == 1)
                {
                    int distance = Mathf.Abs(x - from.x) + Mathf.Abs(y - from.y);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        furthest = new Vector2Int(x, y);
                    }
                }
            }
        }

        return furthest;
    }





    
}