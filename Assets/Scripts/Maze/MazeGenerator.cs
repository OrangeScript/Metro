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

    

    public MazePlayer GenerateMaze()
    {
        maze = new int[width, height];
        InitializeMaze();

        startCell = new Vector2Int(1, 1);
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

        endCell = FindFurthestCell(startCell);
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


    //public void GenerateDynamicMaze()
    //{
        
    //    start = new Vector3(7.3f, Random.Range(3.2f, -3.2f), 0);
    //    end = new Vector3(-7.3f, Random.Range(3.2f, -3.2f), 0);
    //    // �����Χ��
    //    Bounds bounds = new Bounds(start, Vector3.zero);
    //    bounds.Encapsulate(end);
    //    bounds.Expand(padding * cellSize * 2); // �߽���չ

    //    // ��������ߴ�
    //    int gridWidth = Mathf.CeilToInt(bounds.size.x / cellSize);
    //    int gridHeight = Mathf.CeilToInt(bounds.size.y / cellSize);
    //    gridWidth = (gridWidth % 2 == 0) ? gridWidth + 1 : gridWidth; // ȷ������
    //    gridHeight = (gridHeight % 2 == 0) ? gridHeight + 1 : gridHeight;

    //    // ��ʼ���Թ�����
    //    maze = new int[gridWidth, gridHeight];
    //    for (int x = 0; x < gridWidth; x++)
    //        for (int y = 0; y < gridHeight; y++)
    //            maze[x, y] = 0;

    //    // ת�����굽����ռ� ------------------------------------------------
    //    Vector2Int startGrid = WorldToGrid(start, bounds);
    //    Vector2Int endGrid = WorldToGrid(end, bounds);

    //    // �����Թ�·�� -----------------------------------------------------
    //    GenerateConnectedMaze(startGrid, endGrid);

    //    // �������յ�
    //    maze[startGrid.x, startGrid.y] = 1;
    //    maze[endGrid.x, endGrid.y] = 1;

    //    // �������ݸ���Ⱦ��
    //    mazeRenderer.Render(maze, bounds, cellSize);
    //    Instantiate(startPrefab, start, Quaternion.identity);
    //    Instantiate(endPrefab, end, Quaternion.identity);
    //}

    //private Vector2Int WorldToGrid(Vector3 worldPos, Bounds bounds)
    //{
    //    float x = (worldPos.x - bounds.min.x) / cellSize;
    //    float y = (worldPos.y - bounds.min.y) / cellSize;
    //    return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    //}

    //private void GenerateConnectedMaze(Vector2Int start, Vector2Int end)
    //{
    //    //when this method is fixed ,this part would be done quickly
    //    // ʹ��Prim�㷨���ɻ����Թ�
    //    RandomizedPrim();

    //    //// ǿ����ͨ·��// Do you know how to make it work properly?
    //    //List<Vector2Int> path = AStar(start, end);
    //    //foreach (var cell in path)
    //    //{
    //    //    maze[cell.x, cell.y] = 1;
    //    //    ConnectNeighbors(cell);
    //    //}
    //}
    ////TODO: this method still has lot of bugs and I don't know how to fix it
    //private void RandomizedPrim()
    //{
    //    List<Vector2Int> walls = new List<Vector2Int>();
    //    HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

    //    // �����㣨�������꣩
    //    Vector2Int startCell = new Vector2Int(
    //        Random.Range(1, maze.GetLength(0)- 1) | 1,
    //        Random.Range(1, maze.GetLength(1) - 1) | 1
    //    );
    //    maze[startCell.x, startCell.y] = 1;
    //    visited.Add(startCell);

    //    AddWalls(startCell, walls, visited);

    //    while (walls.Count > 0)
    //    {
    //        int randomIndex = Random.Range(0, walls.Count);
    //        Vector2Int wall = walls[randomIndex];
    //        walls.RemoveAt(randomIndex);

    //        List<Vector2Int> cells = GetAdjacentCells(wall);
    //        if (cells.Count == 2)
    //        {
    //            Vector2Int a = cells[0];
    //            Vector2Int b = cells[1];

    //            if (visited.Contains(a) ^ visited.Contains(b))
    //            {
    //                maze[wall.x, wall.y] = 1;
    //                Vector2Int newCell = visited.Contains(a) ? b : a;
    //                maze[newCell.x, newCell.y] = 1;
    //                visited.Add(newCell);
    //                AddWalls(newCell, walls, visited);
    //            }
    //        }
    //    }
    //}

    //// 3. ���ǽ���б��������ң�������ǽ��������һ����δ���ʵĸ��ӣ�
    //private void AddWalls(Vector2Int cell, List<Vector2Int> walls, HashSet<Vector2Int> visited)
    //{
    //    Vector2Int[] directions = {
    //    new Vector2Int(2, 0),
    //    new Vector2Int(-2, 0),
    //    new Vector2Int(0, 2),
    //    new Vector2Int(0, -2)
    //};

    //    foreach (var dir in directions)
    //    {
    //        Vector2Int neighbor = cell + dir;
    //        Vector2Int wall = cell + dir / 2;

    //        if (IsInBounds(neighbor) && !visited.Contains(neighbor) && maze[neighbor.x, neighbor.y] == 0)
    //        {
    //            walls.Add(wall);
    //        }
    //    }
    //}

    //// 4. ��ȡǽ�����������Ԫ��
    //private List<Vector2Int> GetAdjacentCells(Vector2Int wall)
    //{
    //    List<Vector2Int> cells = new List<Vector2Int>();

    //    Vector2Int[] directions = {
    //    new Vector2Int(1, 0),
    //    new Vector2Int(-1, 0),
    //    new Vector2Int(0, 1),
    //    new Vector2Int(0, -1)
    //};

    //    foreach (var dir in directions)
    //    {
    //        Vector2Int cell = wall + dir;
    //        if (IsInBounds(cell) && maze[cell.x, cell.y] == 1)
    //        {
    //            cells.Add(cell);
    //        }
    //    }

    //    return cells;
    //}

    //// 5. �ж��Ƿ��ڱ߽��ڣ���ȥ��Ե��
    //private bool IsInBounds(Vector2Int pos)
    //{
    //    return pos.x > 0 && pos.x < maze.GetLength(0) - 1 && pos.y > 0 && pos.y < maze.GetLength(1) - 1;
    //}

    //private class AStarNode
    //{
    //    public Vector2Int pos;
    //    public AStarNode parent;
    //    public int gCost;  // ������ʵ�ʴ���
    //    public int hCost;  // ���յ������ʽ����
    //    public int FCost => gCost + hCost;
    //}

    //private List<Vector2Int> AStar(Vector2Int start, Vector2Int targetEnd)
    //{
    //    // ��ʼ�����ݽṹ
    //    Dictionary<Vector2Int, AStarNode> allNodes = new Dictionary<Vector2Int, AStarNode>();
    //    List<AStarNode> openSet = new List<AStarNode>();
    //    HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

    //    // ������ʼ�ڵ�
    //    AStarNode startNode = new AStarNode
    //    {
    //        pos = start,
    //        gCost = 0,
    //        hCost = CalculateHeuristic(start, targetEnd)
    //    };
    //    allNodes.Add(start, startNode);
    //    openSet.Add(startNode);

    //    while (openSet.Count > 0)
    //    {
    //        // �ҵ���ǰ��ʹ��۽ڵ�
    //        openSet.Sort((a, b) => a.FCost.CompareTo(b.FCost));
    //        AStarNode current = openSet[0];
    //        openSet.RemoveAt(0);

    //        // �����յ�
    //        if (current.pos == targetEnd)
    //        {
    //            return ReconstructPath(current);
    //        }

    //        closedSet.Add(current.pos);

    //        // ����ĸ�����
    //        foreach (var dir in new[] { Vector2Int.up, Vector2Int.down,
    //                 Vector2Int.left, Vector2Int.right })
    //        {
    //            Vector2Int neighborPos = current.pos + dir;

    //            // ��Ч�Լ��
    //            if (!IsPositionValid(neighborPos) || closedSet.Contains(neighborPos))
    //                continue;

    //            // �����ƶ����ۣ�������ǽ�ڵ���Ҫ���ߴ��ۣ�
    //            int moveCost = maze[neighborPos.x, neighborPos.y] == 0 ? 10 : 1;

    //            // ����/��ȡ�ڵ�
    //            if (!allNodes.TryGetValue(neighborPos, out AStarNode neighbor))
    //            {
    //                neighbor = new AStarNode { pos = neighborPos };
    //                allNodes.Add(neighborPos, neighbor);
    //            }

    //            int tentativeGCost = current.gCost + moveCost;
    //            if (tentativeGCost < neighbor.gCost || !openSet.Contains(neighbor))
    //            {
    //                neighbor.gCost = tentativeGCost;
    //                neighbor.hCost = CalculateHeuristic(neighborPos, targetEnd);
    //                neighbor.parent = current;

    //                if (!openSet.Contains(neighbor))
    //                    openSet.Add(neighbor);
    //            }
    //        }
    //    }

    //    Debug.LogWarning("δ�ҵ�·����");
    //    return new List<Vector2Int>();
    //}

    //private int CalculateHeuristic(Vector2Int a, Vector2Int b)
    //{
    //    // ʹ�������پ���
    //    return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    //}

    //private List<Vector2Int> ReconstructPath(AStarNode endNode)
    //{
    //    List<Vector2Int> path = new List<Vector2Int>();
    //    AStarNode current = endNode;
    //    while (current != null)
    //    {
    //        path.Add(current.pos);
    //        current = current.parent;
    //    }
    //    path.Reverse();
    //    return path;
    //}

    //private bool IsPositionValid(Vector2Int pos)
    //{
    //    // ����Ƿ����Թ���Χ��
    //    return pos.x >= 0 && pos.x < maze.GetLength(0) &&
    //           pos.y >= 0 && pos.y < maze.GetLength(1);
    //}



    
}