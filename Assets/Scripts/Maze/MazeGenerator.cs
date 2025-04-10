using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int mazeSize = 11;
    public int[,] maze;
    public MazeRenderer mazeRenderer;
    [SerializeField] private Transform MazeCanvas;

    public Vector2Int start;
    public Vector2Int end;

    public void GenerateMaze()
    {
        maze = new int[mazeSize, mazeSize];

        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                maze[x, y] = 0; // 初始化所有格子为墙壁
            }
        }

        start = new Vector2Int(1, 1);
        end = new Vector2Int(mazeSize - 2, mazeSize - 2);

        GeneratePath(start);

        // 确保终点是路径
        maze[end.x, end.y] = 1;
    }

    private void GeneratePath(Vector2Int start)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(start);
        maze[start.x, start.y] = 1;

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                Vector2Int between = new Vector2Int((current.x + next.x) / 2, (current.y + next.y) / 2);
                maze[between.x, between.y] = 1; // 打通墙壁
                maze[next.x, next.y] = 1; // 走到下一个点
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }
    }

    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = { new(0, 2), new(0, -2), new(2, 0), new(-2, 0) };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = cell + dir;
            if (neighbor.x > 0 && neighbor.x < mazeSize - 1 &&
                neighbor.y > 0 && neighbor.y < mazeSize - 1 &&
                maze[neighbor.x, neighbor.y] == 0)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public void RenderMaze()
    {
        mazeRenderer.DrawMaze(maze,MazeCanvas);
    }
}