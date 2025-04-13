using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    public GameObject wallPrefab; // 墙壁预制体（如方块）
    public GameObject pathPrefab; // 路径预制体（如平面）
    public Material wallMaterial; // 可选：墙壁材质

    public GameObject startPrefab;
    public GameObject endPrefab;

    public float cellSize = 1f;
    public void RenderStartAndEnd(Vector3 startPos,Vector3 endPos)
    {
        Instantiate(startPrefab, startPos, Quaternion.identity,transform);

        Instantiate(endPrefab, endPos, Quaternion.identity,transform);
    }

    public void DrawMaze(int[,] maze)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        // 计算中心偏移
        float offsetX = -(width * cellSize) / 2f + cellSize / 2f;
        float offsetY = -(height * cellSize) / 2f + cellSize / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 0)
                {
                    Vector3 pos = new Vector3(x * cellSize + offsetX, y*cellSize+offsetY,0);
                    Instantiate(wallPrefab, pos, Quaternion.identity, transform);
                }
                else if (maze[x, y] == 1)
                {
                    Vector3 pos = new Vector3(x * cellSize + offsetX, y * cellSize + offsetY, 0);
                    Instantiate(pathPrefab, pos, Quaternion.identity, transform);
                }
            }
        }
        
    }
}
