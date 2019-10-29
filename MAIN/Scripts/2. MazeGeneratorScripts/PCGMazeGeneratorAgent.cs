using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Timers;

public class PCGMazeGeneratorAgent : MonoBehaviour
{
    public MazeGeneratorArea MazeGeneratorArea;
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
        //AgentReset();
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

        if (!PcgHKMG.IsSeedValid(Seed))
        {
            MazeGeneratorArea.MazeGeneratorPlayerAgent.mazeReady = false;
            MistakesMade++;
            MazeGeneratorArea.ResetArea();
            return;
        }

        // initialize maze
        mazeCells = PcgHKMG.InitializeMaze(gameObject, MazeRows, MazeColumns, WallPrefab, CellLocationPrefab, CellSize, MazeGeneratorArea, PlayerGoal);
        PcgHKMG.CreateMaze(mazeCells, Seed);
        PcgHKMG.CompleteMaze(mazeCells, MazeRows, MazeColumns);
        distanceFromStartToEnd = PcgHKMG.CellDistanceToEnd(mazeCells[0, 0], FromCell.start);

        MazeGeneratorArea.scoreTotal = Mathf.CeilToInt(distanceFromStartToEnd * 1.5f);
        MazeGeneratorArea.penaltyThreshold = distanceFromStartToEnd;

        PerfectMazesGenerated++;

        // CAPTURE DATA
        var diff = DateTime.UtcNow - startGenerating;

        MazeComplexity = PcgHKMG.CalculateMazeComplexity(mazeCells);
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

        MazeGeneratorArea.MazeGeneratorPlayerAgent.mazeReady = true;
    }

    public void AgentReset()
    {
        // Destroy all walls
        if (mazeCells != null && mazeCells.Length > 0)
        {
            PcgHKMG.DestroyMaze(mazeCells);
        }
        // Clear parameters
        startGenerating = DateTime.UtcNow;
        GenerateMaze();
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
