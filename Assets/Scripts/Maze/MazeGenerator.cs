using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    
    public MazeRenderer mazeRenderer;

    public Vector3 start;
    public Vector3 end;

    private int mazeSize = 11; // 迷宫尺寸（必须为奇数，否则可能无法保证起点/终点是通路）
    public int[,] maze;
    public int mainPathBias = 3;
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject endPrefab;


    public void GenerateMaze()
    {
        maze=new int[mazeSize,mazeSize];
        for(int i = 0; i < mazeSize; i++)
        {
            for (int j = 0; j < mazeSize; j++)
            {
                maze[i,j]=0;
            }
        }
        start=new Vector3(-7.38f, Random.Range(-3.47f,3.47f), 0);
        end = new Vector3(7.38f, Random.Range(-3.47f, 3.47f), 0);
        Vector2Int startCell = WorldToGrid(start);
        Vector2Int endCell = WorldToGrid(end);
        startCell = ClampCell(startCell);
        endCell = ClampCell(endCell);
        GenerateConnectedPath(startCell, endCell);
    }
    public void RenderMaze()
    {
        Instantiate(startPrefab, start, Quaternion.identity);
        Instantiate(endPrefab, end, Quaternion.identity);

        mazeRenderer.DrawMaze(maze);
        Debug.Log($"预期尺寸: {mazeSize}x{mazeSize}，实际数组尺寸: {maze.GetLength(0)}x{maze.GetLength(1)}");
    }
    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        // 将世界坐标转换为网格索引
        float cellSize = 14.76f / (mazeSize - 1); // 根据你的场景宽度计算
        int x = Mathf.RoundToInt((worldPos.x + 7.38f) / cellSize);
        int y = Mathf.RoundToInt((worldPos.y + 3.47f) / cellSize);
        return new Vector2Int(x, y);
    }

    private Vector2Int ClampCell(Vector2Int cell)
    {
        // 确保坐标在迷宫范围内
        cell.x = Mathf.Clamp(cell.x, 1, mazeSize - 2);
        cell.y = Mathf.Clamp(cell.y, 1, mazeSize - 2);
        return cell;
    }

    private void GenerateConnectedPath(Vector2Int start, Vector2Int end)
    {
        // 使用A*算法生成主路径
        List<Vector2Int> mainPath = AStar(start, end);

        // 生成主路径
        foreach (var cell in mainPath)
        {
            maze[cell.x, cell.y] = 1;
        }

        // 添加随机分支
        AddRandomBranches(mainPath);
    }

    private List<Vector2Int> AStar(Vector2Int start, Vector2Int end)
    {
        // 简单A*算法实现
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;

        while (current != end)
        {
            // 向终点方向移动
            int dx = end.x - current.x;
            int dy = end.y - current.y;

            if (Mathf.Abs(dx) > Mathf.Abs(dy))
            {
                current.x += (int)Mathf.Sign(dx);
            }
            else
            {
                current.y += (int)Mathf.Sign(dy);
            }

            // 防止越界
            current.x = Mathf.Clamp(current.x, 1, mazeSize - 2);
            current.y = Mathf.Clamp(current.y, 1, mazeSize - 2);

            path.Add(current);
        }

        return path;
    }

    private void AddRandomBranches(List<Vector2Int> mainPath)
    {
        // 随机添加分支路径
        foreach (var cell in mainPath)
        {
            if (Random.Range(0, 100) < 30) // 30%概率生成分支
            {
                int branchLength = Random.Range(1, 4);
                Vector2Int dir = GetRandomDirection();

                Vector2Int current = cell;
                for (int i = 0; i < branchLength; i++)
                {
                    current += dir;
                    if (current.x < 1 || current.x >= mazeSize - 1 ||
                        current.y < 1 || current.y >= mazeSize - 1) break;

                    maze[current.x, current.y] = 1;
                }
            }
        }
    }

    private Vector2Int GetRandomDirection()
    {
        Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        return directions[Random.Range(0, 4)];
    }
}