using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    
    public MazeRenderer mazeRenderer;

    public Vector3 start;
    public Vector3 end;

    private int mazeSize = 11; // �Թ��ߴ磨����Ϊ��������������޷���֤���/�յ���ͨ·��
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
        Debug.Log($"Ԥ�ڳߴ�: {mazeSize}x{mazeSize}��ʵ������ߴ�: {maze.GetLength(0)}x{maze.GetLength(1)}");
    }
    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        // ����������ת��Ϊ��������
        float cellSize = 14.76f / (mazeSize - 1); // ������ĳ�����ȼ���
        int x = Mathf.RoundToInt((worldPos.x + 7.38f) / cellSize);
        int y = Mathf.RoundToInt((worldPos.y + 3.47f) / cellSize);
        return new Vector2Int(x, y);
    }

    private Vector2Int ClampCell(Vector2Int cell)
    {
        // ȷ���������Թ���Χ��
        cell.x = Mathf.Clamp(cell.x, 1, mazeSize - 2);
        cell.y = Mathf.Clamp(cell.y, 1, mazeSize - 2);
        return cell;
    }

    private void GenerateConnectedPath(Vector2Int start, Vector2Int end)
    {
        // ʹ��A*�㷨������·��
        List<Vector2Int> mainPath = AStar(start, end);

        // ������·��
        foreach (var cell in mainPath)
        {
            maze[cell.x, cell.y] = 1;
        }

        // ��������֧
        AddRandomBranches(mainPath);
    }

    private List<Vector2Int> AStar(Vector2Int start, Vector2Int end)
    {
        // ��A*�㷨ʵ��
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;

        while (current != end)
        {
            // ���յ㷽���ƶ�
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

            // ��ֹԽ��
            current.x = Mathf.Clamp(current.x, 1, mazeSize - 2);
            current.y = Mathf.Clamp(current.y, 1, mazeSize - 2);

            path.Add(current);
        }

        return path;
    }

    private void AddRandomBranches(List<Vector2Int> mainPath)
    {
        // �����ӷ�֧·��
        foreach (var cell in mainPath)
        {
            if (Random.Range(0, 100) < 30) // 30%�������ɷ�֧
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