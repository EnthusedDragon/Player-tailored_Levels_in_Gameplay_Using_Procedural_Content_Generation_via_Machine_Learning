using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class PCGMazeGenerator : MonoBehaviour
{
    public PCGMazeGeneratorArea MazeGeneratorArea;
    public PCGMazeGeneratorPlayerAgent MazeGeneratorPlayerAgent;
    public GameObject PlayerGoal;
    public GameObject CellLocationPrefab;
    public GameObject WallPrefab;

    public MazeCell[,] mazeCells;
    public float CellSize = 5.4f;
    private int distanceFromStartToEnd = 0;

    // RESEARCH DATA TO BE SAVED
    public bool CaptureData = false;
    public string CSVFilePath;
    // ONCE OFF STATIC
    public string GeneratorName;
    public string GeneratorType;
    public int StepsTrained;

    // ONCE OFF UPDATED
    private int PerfectMazesGenerated;
    private int MistakesMade;

    // CONTINUOUS
    private int MazeNumber = 0;
    private int MazeRows = 0, MazeColumns = 0;
    private string Seed = "12341234123412341234";
    public float MazeComplexity;
    private DateTime startGenerating;

    void Awake()
    {
        if (CaptureData)
        {
            if (!MazeGeneratorArea.PlayerFileCreated)
            {
                var headings = new List<string>()
                {
                    "GeneratorName",
                    "GeneratorType",
                    "StepsTrained",
                    "PerfectMazesGenerated",
                    "MistakesMade",
                    "MazeRows",
                    "MazeColumns",
                    "Seed",
                    "MazeComplexity",
                    "TotalMillisecondsToGenerate"
                };
                DataRecorder.WriteRecordToCSV(headings, CSVFilePath);
                MazeGeneratorArea.PlayerFileCreated = true;
            }
        }
        AgentReset();
    }

    public void GenerateMaze()
    {
        MazeNumber++;

        var vectorAction = new float[22]
        {
            UnityEngine.Random.Range(0,8),
            UnityEngine.Random.Range(0,8),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
            UnityEngine.Random.Range(0,4),
        };

        // Call once on demand
        MazeRows = (int)vectorAction[0] + 2;
        MazeColumns = (int)vectorAction[1] + 2;

        Seed = $"" +
            $"{(int)vectorAction[2] + 1}" +
            $"{(int)vectorAction[3] + 1}" +
            $"{(int)vectorAction[4] + 1}" +
            $"{(int)vectorAction[5] + 1}" +
            $"{(int)vectorAction[6] + 1}" +
            $"{(int)vectorAction[7] + 1}" +
            $"{(int)vectorAction[8] + 1}" +
            $"{(int)vectorAction[9] + 1}" +
            $"{(int)vectorAction[10] + 1}" +
            $"{(int)vectorAction[11] + 1}" +
            $"{(int)vectorAction[12] + 1}" +
            $"{(int)vectorAction[13] + 1}" +
            $"{(int)vectorAction[14] + 1}" +
            $"{(int)vectorAction[15] + 1}" +
            $"{(int)vectorAction[16] + 1}" +
            $"{(int)vectorAction[17] + 1}" +
            $"{(int)vectorAction[18] + 1}" +
            $"{(int)vectorAction[19] + 1}" +
            $"{(int)vectorAction[20] + 1}" +
            $"{(int)vectorAction[21] + 1}";

        Regex regex = new Regex(@"[05-9]");
        if (!Seed.Contains("1") || !Seed.Contains("2") || !Seed.Contains("3") || !Seed.Contains("4") || regex.IsMatch(Seed) || Seed.Length != 20)
        {
            MazeGeneratorPlayerAgent.mazeReady = false;
            MistakesMade++;
            MazeGeneratorArea.ResetArea();
            return;
        }

        // initialize maze
        InitializeMaze();
        CreateMaze();
        CompleteMaze();
        CalculateDistanceFromEnd();

        MazeGeneratorArea.scoreTotal = Mathf.CeilToInt(distanceFromStartToEnd * 1.5f);
        MazeGeneratorArea.penaltyThreshold = distanceFromStartToEnd;

        PerfectMazesGenerated++;

        // CAPTURE DATA
        var diff = (DateTime.UtcNow - startGenerating);

        MazeComplexity = CalculateComplexity();

        MazeGeneratorArea.complexity = MazeComplexity;

        if (CaptureData)
        {
            var data = new List<string>()
            {
                GeneratorName,
                GeneratorType,
                StepsTrained.ToString(),
                PerfectMazesGenerated.ToString(),
                MistakesMade.ToString(),
                MazeRows.ToString(),
                MazeColumns.ToString(),
                $"[{Seed}]",
                MazeComplexity.ToString(), // TO DO
                diff.TotalMilliseconds.ToString()
            };
            DataRecorder.WriteRecordToCSV(data, CSVFilePath);
        }

        MazeGeneratorPlayerAgent.mazeReady = true;
    }


    public void AgentReset()
    {
        // Destroy all walls
        if (mazeCells != null && mazeCells.Length > 0)
        {
            DestroyMaze();
        }
        // Clear parameters
        startGenerating = DateTime.UtcNow;
        GenerateMaze();
    }

    private void DestroyMaze()
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

    private void InitializeMaze()
    {

        mazeCells = new MazeCell[MazeRows, MazeColumns];

        for (int r = 0; r < MazeRows; r++)
        {
            for (int c = 0; c < MazeColumns; c++)
            {
                mazeCells[r, c] = new MazeCell
                {
                    cellParentAndLocation = Instantiate(CellLocationPrefab, transform.position + new Vector3(r * CellSize, 0, c * CellSize), Quaternion.identity, transform) as GameObject
                };
                mazeCells[r, c].cellParentAndLocation.name = $"Cell [{r},{c}]";
                mazeCells[r, c].cellParentAndLocation.AddComponent<MazeCellValues>();

                if (c == 0)
                {
                    mazeCells[r, c].leftWall = Instantiate(WallPrefab, transform.position + new Vector3(r * CellSize, 0, (c * CellSize) - (CellSize / 2f)), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                    mazeCells[r, c].leftWall.name = "Left Wall " + r + "," + c;
                }

                mazeCells[r, c].rightWall = Instantiate(WallPrefab, transform.position + new Vector3(r * CellSize, 0, (c * CellSize) + (CellSize / 2f)), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                mazeCells[r, c].rightWall.name = "Right Wall " + r + "," + c;

                if (r == 0)
                {
                    mazeCells[r, c].topWall = Instantiate(WallPrefab, transform.position + new Vector3((r * CellSize) - (CellSize / 2f), 0, c * CellSize), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                    mazeCells[r, c].topWall.name = "Top Wall " + r + "," + c;
                    mazeCells[r, c].topWall.transform.Rotate(Vector3.up * 90f);
                }

                mazeCells[r, c].bottomWall = Instantiate(WallPrefab, transform.position + new Vector3((r * CellSize) + (CellSize / 2f), 0, c * CellSize), Quaternion.identity, mazeCells[r, c].cellParentAndLocation.transform) as GameObject;
                mazeCells[r, c].bottomWall.name = "Bottom Wall " + r + "," + c;
                mazeCells[r, c].bottomWall.transform.Rotate(Vector3.up * 90f);
            }
        }

        MazeGeneratorPlayerAgent.transform.position = mazeCells[0, 0].cellParentAndLocation.transform.position;

        var temp = Instantiate(MazeGeneratorPlayerAgent, transform.parent);
        Destroy(MazeGeneratorPlayerAgent.gameObject);
        MazeGeneratorPlayerAgent = temp;

        MazeGeneratorPlayerAgent.currentCell = mazeCells[0, 0];

        PlayerGoal.transform.position = mazeCells[MazeRows - 1, MazeColumns - 1].cellParentAndLocation.transform.position;
    }

    private void CreateMaze()
    {
        MazeAlgorithm ma = new HuntAndKillMazeAlgorithm(mazeCells, Seed);
        ma.CreateMaze();
    }

    private void CompleteMaze()
    {
        for (int r = 0; r < MazeRows; r++)
        {
            for (int c = 0; c < MazeColumns; c++)
            {
                if (c == 0 & r == 0)
                    mazeCells[r, c].startCell = true;

                if (c == MazeColumns - 1 & r == MazeRows - 1)
                    mazeCells[r, c].endCell = true;

                // LOOK DOWN
                if (r < MazeRows - 1)
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
                if (c < MazeColumns - 1)
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

    private void CalculateDistanceFromEnd()
    {
        distanceFromStartToEnd = DistanceToEnd(mazeCells[0, 0], FromCell.start);
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

    public float CalculateComplexity()
    {
        MazeCell chosenCell;
        var complexity = 0f;

        foreach (MazeCell mazeCell in mazeCells)
        {
            if (mazeCell.calculated < 1 && mazeCell.cellType == CellType.DeadEnd)
            {
                chosenCell = mazeCell;
                complexity += CalculatePathComplexity(GetPath(mazeCell, FromCell.player));
            }

            if (mazeCell.calculated < 3 && mazeCell.cellType == CellType.TJunction)
            {
                chosenCell = mazeCell;
                complexity += CalculatePathComplexity(GetPath(mazeCell, FromCell.player));
                mazeCell.calculated++;
            }

            if (mazeCell.calculated < 4 && mazeCell.cellType == CellType.XJunction)
            {
                chosenCell = mazeCell;
                complexity += CalculatePathComplexity(GetPath(mazeCell, FromCell.player));
                mazeCell.calculated++;
            }
        }
        return complexity;
    }

    public List<MazeCell> GetPath(MazeCell mazeCell, FromCell fromCell)
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
            path.AddRange(GetPath(mazeCell.topCell, FromCell.bottom));
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
            path.AddRange(GetPath(mazeCell.bottomCell, FromCell.top));
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
            path.AddRange(GetPath(mazeCell.leftCell, FromCell.right));
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
            path.AddRange(GetPath(mazeCell.rightCell, FromCell.left));
            path.Add(mazeCell);
            mazeCell.calculated++;
            return path;
        }

        path.Add(mazeCell);
        mazeCell.calculated++;

        return path;
    }

    public float CalculatePathComplexity(List<MazeCell> mazeCells)
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

    public bool Screenshot;
    private void Update()
    {
        if (Screenshot)
        {
            DataRecorder.TakeTheShot(Camera.main, width: Screen.width, height: Screen.height);
            Screenshot = false;
        }
    }
}
