using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MazeInput : MonoBehaviour
{
    private List<Vector2Int> playerPath = new List<Vector2Int>();
    private bool isInMazeMode = false;
    private Action onMazeComplete;

    public MazeGenerator mazeGenerator;

    //public void StartMazeMode(Action onComplete)
    //{
    //    isInMazeMode = true;
    //    onMazeComplete = onComplete;
    //    playerPath.Clear();
    //    playerPath.Add(mazeGenerator.start);
    //}

    //void Update()
    //{
    //    if (!isInMazeMode) return;

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector2Int cell = new Vector2Int(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));

    //        if (mazeGenerator.maze[cell.x, cell.y] == 1 && !playerPath.Contains(cell))
    //        {
    //            playerPath.Add(cell);

    //            if (cell == mazeGenerator.end)
    //            {
    //                isInMazeMode = false;
    //                onMazeComplete?.Invoke();
    //            }
    //        }
    //    }
    //}
}