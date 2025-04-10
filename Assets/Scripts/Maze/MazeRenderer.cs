using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject pathPrefab;

    public void DrawMaze(int[,] maze,Transform mazeCanvas)
    {
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                Vector3 pos = new Vector3(x, y, 0);
                if (maze[x, y] == 0)
                {
                    Instantiate(wallPrefab, pos, Quaternion.identity,mazeCanvas);
                }
                else
                {
                    Instantiate(pathPrefab, pos, Quaternion.identity,mazeCanvas);
                }
            }
        }
    }
}