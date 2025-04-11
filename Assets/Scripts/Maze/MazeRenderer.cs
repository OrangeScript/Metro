using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    public GameObject wallPrefab; // ǽ��Ԥ���壨�緽�飩
    public GameObject pathPrefab; // ·��Ԥ���壨��ƽ�棩
    public Material wallMaterial; // ��ѡ��ǽ�ڲ���

    public void Render(int[,] maze, Bounds bounds, float cellSize)
    {
        ClearOldMaze();

        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = new Vector3(
                    bounds.min.x + x * cellSize + cellSize / 2,
                    bounds.min.y + y * cellSize + cellSize / 2,
                    0
                );

                if (maze[x, y] == 0)
                {
                    // ����ǽ��
                    GameObject wall = Instantiate(wallPrefab, worldPos, Quaternion.identity);
                    wall.transform.localScale = new Vector3(cellSize, cellSize, 1);
                    wall.transform.SetParent(transform);
                    if (wallMaterial) wall.GetComponent<Renderer>().material = wallMaterial;
                }
                else
                {
                    // ����·��
                    GameObject path = Instantiate(pathPrefab, worldPos, Quaternion.identity);
                    path.transform.localScale = new Vector3(cellSize, cellSize ,0);
                    path.transform.SetParent(transform);
                }
            }
        }
    }

    private void ClearOldMaze()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }
}