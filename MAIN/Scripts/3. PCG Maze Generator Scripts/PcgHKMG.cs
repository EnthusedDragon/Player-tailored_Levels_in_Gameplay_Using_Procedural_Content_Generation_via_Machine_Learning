using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PcgHKMG : MonoBehaviour
{
    public static bool IsSeedValid(string seed)
    {
        Regex regex = new Regex(@"[05-9]");
        if (!seed.Contains("1") || !seed.Contains("2") || !seed.Contains("3") || !seed.Contains("4") || regex.IsMatch(seed) || seed.Length != 20)
        {
            return false;
        }

        return true;
    }

    public static MazeCell[,] InitializeMaze(GameObject agent, int mazeRows, int mazeColumns, GameObject wallPrefab, GameObject cellLocationPrefab, float cellSize, MazeGeneratorArea mazeGeneratorArea, GameObject playerGoal)
    {

        MazeCell[,] mazeCells = new MazeCell[mazeRows, mazeColumns];

        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                mazeCells[r, c] = new MazeCell
                {
                    cellParentAndLocation = Instantiate(cellLocationPrefab, agent.transform.position + new Vector3(r * cellSize, 0, c * cellSize), Quaternion.identity, agent.transform) as GameObject
                };
                mazeCells[r, c].cellParentAndLocation.name = $"Cell [{r},{c}]";
                mazeCells[r, c].cellParentAndLocation.AddComponent<MazeCellValues>();

                if (c == 0)
                {
                    mazeCells[r, c].leftWall = Instantiate(wallPrefab, agent.transform.position + new Vector3(r * cellSize, 0, (c * cellSize) - (cellSize / 2f)), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                    mazeCells[r, c].leftWall.name = "Left Wall " + r + "," + c;
                }

                mazeCells[r, c].rightWall = Instantiate(wallPrefab, agent.transform.position + new Vector3(r * cellSize, 0, (c * cellSize) + (cellSize / 2f)), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                mazeCells[r, c].rightWall.name = "Right Wall " + r + "," + c;

                if (r == 0)
                {
                    mazeCells[r, c].topWall = Instantiate(wallPrefab, agent.transform.position + new Vector3((r * cellSize) - (cellSize / 2f), 0, c * cellSize), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                    mazeCells[r, c].topWall.name = "Top Wall " + r + "," + c;
                    mazeCells[r, c].topWall.transform.Rotate(Vector3.up * 90f);
                }

                mazeCells[r, c].bottomWall = Instantiate(wallPrefab, agent.transform.position + new Vector3((r * cellSize) + (cellSize / 2f), 0, c * cellSize), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                mazeCells[r, c].bottomWall.name = "Bottom Wall " + r + "," + c;
                mazeCells[r, c].bottomWall.transform.Rotate(Vector3.up * 90f);
            }
        }

        mazeGeneratorArea.MazeGeneratorPlayerAgent.transform.position = mazeCells[0, 0].cellParentAndLocation.transform.position;

        var temp = Instantiate(mazeGeneratorArea.MazeGeneratorPlayerAgent.gameObject, agent.transform.parent);
        Destroy(mazeGeneratorArea.MazeGeneratorPlayerAgent.gameObject);
        mazeGeneratorArea.MazeGeneratorPlayerAgent = temp.GetComponent<PCGMazeGeneratorPlayerAgent>();

        mazeGeneratorArea.MazeGeneratorPlayerAgent.currentCell = mazeCells[0, 0];

        playerGoal.transform.position = mazeCells[mazeRows - 1, mazeColumns - 1].cellParentAndLocation.transform.position;

        return mazeCells;
    }

    public static void CompleteMaze(MazeCell[,] mazeCells, int mazeRows, int mazeColumns)
    {
        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                if (c == 0 & r == 0)
                    mazeCells[r, c].startCell = true;

                if (c == mazeColumns - 1 & r == mazeRows - 1)
                    mazeCells[r, c].endCell = true;

                // LOOK DOWN
                if (r < mazeRows - 1)
                {
                    if (mazeCells[r, c].bottomWall == null && mazeCells[r + 1, c].topWall == null)
                    {
                        mazeCells[r, c].bottomCell = mazeCells[r + 1, c];
                    }
                }

                // LOOK UP
                if (r > 0)
                {
                    if (mazeCells[r, c].topWall == null && mazeCells[r - 1, c].bottomWall == null)
                    {
                        mazeCells[r, c].topCell = mazeCells[r - 1, c];
                    }
                }

                // LOOK LEFT
                if (c > 0)
                {
                    if (mazeCells[r, c].leftWall == null && mazeCells[r, c - 1].rightWall == null)
                    {
                        mazeCells[r, c].leftCell = mazeCells[r, c - 1];
                    }
                }

                // LOOK RIGHT
                if (c < mazeColumns - 1)
                {
                    if (mazeCells[r, c].rightWall == null && mazeCells[r, c + 1].leftWall == null)
                    {
                        mazeCells[r, c].rightCell = mazeCells[r, c + 1];
                    }
                }

                if (mazeCells[r, c].leftCell != null && mazeCells[r, c].rightCell != null && mazeCells[r, c].topCell != null && mazeCells[r, c].bottomCell != null)
                {
                    mazeCells[r, c].cellType = CellType.XJunction;
                }
                else if (
                    (mazeCells[r, c].leftCell != null && mazeCells[r, c].rightCell != null && mazeCells[r, c].topCell != null && mazeCells[r, c].bottomCell == null) ||
                    (mazeCells[r, c].leftCell != null && mazeCells[r, c].rightCell != null && mazeCells[r, c].topCell == null && mazeCells[r, c].bottomCell != null) ||
                    (mazeCells[r, c].leftCell != null && mazeCells[r, c].rightCell == null && mazeCells[r, c].topCell != null && mazeCells[r, c].bottomCell != null) ||
                    (mazeCells[r, c].leftCell == null && mazeCells[r, c].rightCell != null && mazeCells[r, c].topCell != null && mazeCells[r, c].bottomCell != null)
                    )
                {
                    mazeCells[r, c].cellType = CellType.TJunction;
                }
                else if (
                    (mazeCells[r, c].leftCell != null && mazeCells[r, c].rightCell == null && mazeCells[r, c].topCell != null && mazeCells[r, c].bottomCell == null) || // LEFT & TOP
                    (mazeCells[r, c].leftCell != null && mazeCells[r, c].rightCell == null && mazeCells[r, c].topCell == null && mazeCells[r, c].bottomCell != null) || // LEFT & BOTTOM
                    (mazeCells[r, c].leftCell == null && mazeCells[r, c].rightCell != null && mazeCells[r, c].topCell != null && mazeCells[r, c].bottomCell == null) || // RIGHT & TOP
                    (mazeCells[r, c].leftCell == null && mazeCells[r, c].rightCell != null && mazeCells[r, c].topCell == null && mazeCells[r, c].bottomCell != null) // RIGHT & BOTTOM
                    )
                {
                    mazeCells[r, c].cellType = CellType.Corner;
                }
                else if (
                    (mazeCells[r, c].leftCell != null && mazeCells[r, c].rightCell == null && mazeCells[r, c].topCell == null && mazeCells[r, c].bottomCell == null) ||
                    (mazeCells[r, c].leftCell == null && mazeCells[r, c].rightCell != null && mazeCells[r, c].topCell == null && mazeCells[r, c].bottomCell == null) ||
                    (mazeCells[r, c].leftCell == null && mazeCells[r, c].rightCell == null && mazeCells[r, c].topCell != null && mazeCells[r, c].bottomCell == null) ||
                    (mazeCells[r, c].leftCell == null && mazeCells[r, c].rightCell == null && mazeCells[r, c].topCell == null && mazeCells[r, c].bottomCell != null)
                    )
                {
                    mazeCells[r, c].cellType = CellType.DeadEnd;
                }
                else if (
                   (mazeCells[r, c].leftCell != null && mazeCells[r, c].rightCell != null && mazeCells[r, c].topCell == null && mazeCells[r, c].bottomCell == null) ||
                   (mazeCells[r, c].leftCell == null && mazeCells[r, c].rightCell == null && mazeCells[r, c].topCell != null && mazeCells[r, c].bottomCell != null)
                   )
                {
                    mazeCells[r, c].cellType = CellType.Straight;
                }
                else
                {
                    //Debug.LogError($"COMPLETE MAZE(): Something went wrong [{r},{c}]\n" + mazeCells[r, c].ToString());
                }

                var script = mazeCells[r, c].cellParentAndLocation.GetComponent<MazeCellValues>();
                script.MazeCell = mazeCells[r, c];
                script.MazeCellTop = mazeCells[r, c].topCell;
                script.MazeCellBottom = mazeCells[r, c].bottomCell;
                script.MazeCellLeft = mazeCells[r, c].leftCell;
                script.MazeCellRight = mazeCells[r, c].rightCell;
            }
        }
    }

    public static void DestroyMaze(MazeCell[,] mazeCells)
    {
        foreach (var mazeCell in mazeCells)
        {
            Destroy(mazeCell?.rightWall);
            Destroy(mazeCell?.leftWall);
            Destroy(mazeCell?.topWall);
            Destroy(mazeCell?.bottomWall);
            Destroy(mazeCell?.cellParentAndLocation);
        }
    }

    public static void CreateMaze(MazeCell[,] mazeCells, string seed)
    {
        MazeAlgorithm ma = new HuntAndKillMazeAlgorithm(mazeCells, seed);
        ma.CreateMaze();
    }

    public static int CellDistanceToEnd(MazeCell mazeCell, FromCell fromCell)
    {
        var distance = 0;

        if (mazeCell.endCell)
        {
            return ++distance;
        }

        if (mazeCell.topCell != null && fromCell != FromCell.top)
        {
            distance += CellDistanceToEnd(mazeCell.topCell, FromCell.bottom);
        }

        if (mazeCell.bottomCell != null && fromCell != FromCell.bottom)
        {
            distance += CellDistanceToEnd(mazeCell.bottomCell, FromCell.top);
        }

        if (mazeCell.leftCell != null && fromCell != FromCell.left)
        {
            distance += CellDistanceToEnd(mazeCell.leftCell, FromCell.right);
        }

        if (mazeCell.rightCell != null && fromCell != FromCell.right)
        {
            distance += CellDistanceToEnd(mazeCell.rightCell, FromCell.left);
        }

        if (distance > 0)
        {
            distance++;
        }

        return distance;
    }

    public static float CalculateMazeComplexity(MazeCell[,] mazeCells)
    {
        MazeCell chosenCell;
        var complexity = 0f;

        foreach (MazeCell mazeCell in mazeCells)
        {
            if (mazeCell.calculated < 1 && mazeCell.cellType == CellType.DeadEnd)
            {
                chosenCell = mazeCell;
                complexity += CalculatePathComplexity(GetPathFromCell(mazeCell, FromCell.player));
            }

            if (mazeCell.calculated < 3 && mazeCell.cellType == CellType.TJunction)
            {
                chosenCell = mazeCell;
                complexity += CalculatePathComplexity(GetPathFromCell(mazeCell, FromCell.player));
                mazeCell.calculated++;
            }

            if (mazeCell.calculated < 4 && mazeCell.cellType == CellType.XJunction)
            {
                chosenCell = mazeCell;
                complexity += CalculatePathComplexity(GetPathFromCell(mazeCell, FromCell.player));
                mazeCell.calculated++;
            }
        }
        return complexity;
    }

    public static List<MazeCell> GetPathFromCell(MazeCell mazeCell, FromCell fromCell)
    {
        var path = new List<MazeCell>();

        if ((mazeCell.cellType == CellType.TJunction || mazeCell.cellType == CellType.XJunction) && fromCell != FromCell.player)
        {
            path.Add(mazeCell);
            mazeCell.calculated++;
            return path;
        }

        if (mazeCell.topCell != null && fromCell != FromCell.top &&
            (
                (mazeCell.topCell.calculated < 1 && (mazeCell.topCell.cellType == CellType.DeadEnd || mazeCell.topCell.cellType == CellType.Corner || mazeCell.topCell.cellType == CellType.Straight)) ||
                (mazeCell.topCell.calculated < 3 && mazeCell.topCell.cellType == CellType.TJunction) ||
                (mazeCell.topCell.calculated < 4 && mazeCell.topCell.cellType == CellType.XJunction)
            )
        )
        {
            path.AddRange(GetPathFromCell(mazeCell.topCell, FromCell.bottom));
            path.Add(mazeCell);
            mazeCell.calculated++;
            return path;
        }

        if (mazeCell.bottomCell != null && fromCell != FromCell.bottom &&
            (
                (mazeCell.bottomCell.calculated < 1 && (mazeCell.bottomCell.cellType == CellType.DeadEnd || mazeCell.bottomCell.cellType == CellType.Corner || mazeCell.bottomCell.cellType == CellType.Straight)) ||
                (mazeCell.bottomCell.calculated < 3 && mazeCell.bottomCell.cellType == CellType.TJunction) ||
                (mazeCell.bottomCell.calculated < 4 && mazeCell.bottomCell.cellType == CellType.XJunction)
            )
        )
        {
            path.AddRange(GetPathFromCell(mazeCell.bottomCell, FromCell.top));
            path.Add(mazeCell);
            mazeCell.calculated++;
            return path;
        }

        if (mazeCell.leftCell != null && fromCell != FromCell.left &&
            (
                (mazeCell.leftCell.calculated < 1 && (mazeCell.leftCell.cellType == CellType.DeadEnd || mazeCell.leftCell.cellType == CellType.Corner || mazeCell.leftCell.cellType == CellType.Straight)) ||
                (mazeCell.leftCell.calculated < 3 && mazeCell.leftCell.cellType == CellType.TJunction) ||
                (mazeCell.leftCell.calculated < 4 && mazeCell.leftCell.cellType == CellType.XJunction)
            )
        )
        {
            path.AddRange(GetPathFromCell(mazeCell.leftCell, FromCell.right));
            path.Add(mazeCell);
            mazeCell.calculated++;
            return path;
        }

        if (mazeCell.rightCell != null && fromCell != FromCell.right &&
            (
                (mazeCell.rightCell.calculated < 1 && (mazeCell.rightCell.cellType == CellType.DeadEnd || mazeCell.rightCell.cellType == CellType.Corner || mazeCell.rightCell.cellType == CellType.Straight)) ||
                (mazeCell.rightCell.calculated < 3 && mazeCell.rightCell.cellType == CellType.TJunction) ||
                (mazeCell.rightCell.calculated < 4 && mazeCell.rightCell.cellType == CellType.XJunction)
            )
        )
        {
            path.AddRange(GetPathFromCell(mazeCell.rightCell, FromCell.left));
            path.Add(mazeCell);
            mazeCell.calculated++;
            return path;
        }

        path.Add(mazeCell);
        mazeCell.calculated++;

        return path;
    }

    public static float CalculatePathComplexity(List<MazeCell> mazeCells)
    {
        var pathSizeCount = 0;
        var pathlength = 0;
        var shortPathLength = 0;
        var complexity = 0f;

        foreach (MazeCell mazeCell in mazeCells)
        {
            pathSizeCount++;
            pathlength++;
            shortPathLength++;

            if (mazeCell.cellType == CellType.Corner || pathSizeCount == mazeCells.Count)
            {
                if (mazeCell.cellType == CellType.Corner)
                    pathlength++;

                complexity += 1f / shortPathLength;
                shortPathLength = 1;
            }
        }

        return complexity * pathlength;
    }
}
