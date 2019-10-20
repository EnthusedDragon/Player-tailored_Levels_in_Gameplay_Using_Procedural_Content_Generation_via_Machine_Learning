using UnityEngine;
using MLAgents;
using TMPro;

public class MazePlayerArea : Area
{
    public MazePlayerAgent[] MazePlayerAgents;
    public GameObject PlayerGoal;
    public float minimumDistanceFromPlayerGoal = 0.1f;
    public float timeModifierToPlayerGoal;
    public GameObject WallPrefab;
    public GameObject WallParent;
    public GameObject CellLocationPrefab;
    public bool RandomSeed = true;
    public int size = 2;
    public int mazeRows;
    public int mazeColumns;

    public TextMeshPro Score;
    public int levelCompleted = 0;
    public int scoreTotal;
    public int penaltyThreshold;
    public int score;

    public float CellSize = 5.4f;
    private string seed = "";
    public MazeCell[,] mazeCells;
    public int distanceFromStartToEnd = 0;

    public override void ResetArea()
    {
        if (WallParent.activeSelf)
        {
            if (size == 0)
            {
                mazeRows = Random.Range(2, 10);
                mazeColumns = Random.Range(2, 10);
            }
            else
            {
                mazeRows = size;
                mazeColumns = size;
            }
            Score.text = $"Score: {score}";
            SetupMaze();
        }
    }

    private void SetupMaze()
    {
        if (mazeCells != null && mazeCells.Length > 0)
        {
            DestroyMaze();
        }

        if (RandomSeed)
        {
            var newSeed = "";
            while (newSeed.Length < 20)
            {
                newSeed += (Random.Range(0, 4) + 1).ToString();

                if (newSeed.Length == 20)
                {
                    // validate seed
                    //// Contains at least one of each [1,2,3,4]
                    if (!newSeed.Contains("1") || !newSeed.Contains("2") || !newSeed.Contains("3") || !newSeed.Contains("4"))
                    {
                        newSeed = "";
                        //Debug.Log($"Invalid Seed");
                    }
                }
            }
            seed = newSeed;
            //Debug.Log($"Random Seed: {seed}");
        }

        InitializeMaze();
        CreateMaze();
        CompleteMaze();
        CalculateDistanceFromEnd();
        scoreTotal = Mathf.CeilToInt(distanceFromStartToEnd * 1.5f);
        penaltyThreshold = distanceFromStartToEnd;
    }

    private void InitializeMaze()
    {

        mazeCells = new MazeCell[mazeRows, mazeColumns];

        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                mazeCells[r, c] = new MazeCell
                {
                    cellParentAndLocation = Instantiate(CellLocationPrefab, WallParent.transform.position + new Vector3(r * CellSize, 0, c * CellSize), Quaternion.identity, WallParent.transform) as GameObject
                };
                mazeCells[r, c].cellParentAndLocation.name = $"Cell [{r},{c}]";
                mazeCells[r, c].cellParentAndLocation.AddComponent<MazeCellValues>();

                if (c == 0)
                {
                    mazeCells[r, c].leftWall = Instantiate(WallPrefab, WallParent.transform.position + new Vector3(r * CellSize, 0, (c * CellSize) - (CellSize / 2f)), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                    mazeCells[r, c].leftWall.name = "Left Wall " + r + "," + c;
                }

                mazeCells[r, c].rightWall = Instantiate(WallPrefab, WallParent.transform.position + new Vector3(r * CellSize, 0, (c * CellSize) + (CellSize / 2f)), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                mazeCells[r, c].rightWall.name = "Right Wall " + r + "," + c;

                if (r == 0)
                {
                    mazeCells[r, c].topWall = Instantiate(WallPrefab, WallParent.transform.position + new Vector3((r * CellSize) - (CellSize / 2f), 0, c * CellSize), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                    mazeCells[r, c].topWall.name = "Top Wall " + r + "," + c;
                    mazeCells[r, c].topWall.transform.Rotate(Vector3.up * 90f);
                }

                mazeCells[r, c].bottomWall = Instantiate(WallPrefab, WallParent.transform.position + new Vector3((r * CellSize) + (CellSize / 2f), 0, c * CellSize), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                mazeCells[r, c].bottomWall.name = "Bottom Wall " + r + "," + c;
                mazeCells[r, c].bottomWall.transform.Rotate(Vector3.up * 90f);
            }
        }

        //startLocation.x -= 2.7f;
        //var endLocation = mazeCells[mazeRows - 1, mazeColumns - 1].bottomWall.transform.position;
        //endLocation.x -= 2.7f;

        foreach (MazePlayerAgent mazePlayerAgent in MazePlayerAgents)
        {
            mazePlayerAgent.transform.position = mazeCells[0, 0].cellParentAndLocation.transform.position;
            mazePlayerAgent.currentCell = mazeCells[0, 0];
        }

        PlayerGoal.transform.position = mazeCells[mazeRows - 1, mazeColumns - 1].cellParentAndLocation.transform.position;
    }

    private void CompleteMaze()
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

    private void CreateMaze()
    {
        //MazeAlgorithm ma = new HuntAndKillMazeAlgorithm(mazeCells, seed);
        //ma.CreateMaze();
    }

    private void DestroyMaze()
    {
        foreach (var cell in mazeCells)
        {
            DestroyCell(cell);
        }
    }

    private void DestroyCell(MazeCell mazeCell)
    {
        Destroy(mazeCell?.rightWall);
        Destroy(mazeCell?.leftWall);
        Destroy(mazeCell?.topWall);
        Destroy(mazeCell?.bottomWall);
        Destroy(mazeCell?.cellParentAndLocation);
    }

    private void CalculateDistanceFromEnd()
    {
        distanceFromStartToEnd = DistanceToEnd(mazeCells[0, 0], FromCell.start);

        //Debug.Log($"DISTANCE TO END: {distanceFromStartToEnd}");
    }

    public int DistanceToEnd(MazeCell mazeCell, FromCell fromCell)
    {
        var distance = 0;

        if (mazeCell.endCell)
        {
            return ++distance;
        }

        if (mazeCell.topCell != null && fromCell != FromCell.top)
        {
            distance += DistanceToEnd(mazeCell.topCell, FromCell.bottom);
        }

        if (mazeCell.bottomCell != null && fromCell != FromCell.bottom)
        {
            distance += DistanceToEnd(mazeCell.bottomCell, FromCell.top);
        }

        if (mazeCell.leftCell != null && fromCell != FromCell.left)
        {
            distance += DistanceToEnd(mazeCell.leftCell, FromCell.right);
        }

        if (mazeCell.rightCell != null && fromCell != FromCell.right)
        {
            distance += DistanceToEnd(mazeCell.rightCell, FromCell.left);
        }

        if (distance > 0)
        {
            distance++;
        }

        return distance;
    }
}
