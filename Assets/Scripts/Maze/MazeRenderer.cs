using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject pathPrefab;

    //public void DrawMaze(int[,] maze)
    //{
    //    for (int x = 0; x < maze.GetLength(0); x++)
    //    {
    //        for (int y = 0; y < maze.GetLength(1); y++)
    //        {
    //            Vector3 pos = new Vector3(x, y, 0);
    //            if (maze[x, y] == 0)
    //            {
    //                Instantiate(wallPrefab, pos, Quaternion.identity);
    //            }
    //            else
    //            {
    //                Instantiate(pathPrefab, pos, Quaternion.identity);
    //            }
    //        }
    //    }
    //}
    public void DrawMaze(int[,] maze)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        // 计算中心偏移量
        float centerX = (width - 1) * 0.5f;
        float centerY = (height - 1) * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 计算相对于中心的位置
                Vector3 pos = new Vector3(x - centerX, y - centerY, 0);

                if (maze[x, y] == 0)
                {
                    Instantiate(wallPrefab, pos, Quaternion.identity);
                }
                else
                {
                    Instantiate(pathPrefab, pos, Quaternion.identity);
                }
            }
        }
    }
}