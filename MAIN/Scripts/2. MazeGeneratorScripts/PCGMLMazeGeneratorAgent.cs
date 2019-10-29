using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Diagnostics;

public class PCGMLMazeGeneratorAgent : Agent
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
    private DateTime startTime;
    private TimeSpan generationTime;
    private TimeSpan observationTime;

    public override void InitializeAgent()
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
                    "MazeNumber",
                    "MazeRows",
                    "MazeColumns",
                    "Seed",
                    "MazeComplexity",
                    "TotalMillisecondsToObserve",
                    "TotalMillisecondsToGenerate"
                };
                DataRecorder.WriteRecordToCSV(headings, CSVFilePath);
                MazeGeneratorArea.PlayerFileCreated = true;
            }
        }
        //AgentReset();
    }

    public override void CollectObservations()
    {
        // Previous Player Score
        AddVectorObs(MazeGeneratorArea.score);

        // Previous Difficulty
        //AddVectorObs(MazeComplexity);

        // Previous Maze Size
        AddVectorObs(MazeRows);
        AddVectorObs(MazeColumns);

        // Previous Seed
        AddVectorObs(int.Parse($"{Seed[0]}"));
        AddVectorObs(int.Parse($"{Seed[1]}"));
        AddVectorObs(int.Parse($"{Seed[2]}"));
        AddVectorObs(int.Parse($"{Seed[3]}"));
        AddVectorObs(int.Parse($"{Seed[4]}"));
        AddVectorObs(int.Parse($"{Seed[5]}"));
        AddVectorObs(int.Parse($"{Seed[6]}"));
        AddVectorObs(int.Parse($"{Seed[7]}"));
        AddVectorObs(int.Parse($"{Seed[8]}"));
        AddVectorObs(int.Parse($"{Seed[9]}"));
        AddVectorObs(int.Parse($"{Seed[10]}"));
        AddVectorObs(int.Parse($"{Seed[11]}"));
        AddVectorObs(int.Parse($"{Seed[12]}"));
        AddVectorObs(int.Parse($"{Seed[13]}"));
        AddVectorObs(int.Parse($"{Seed[14]}"));
        AddVectorObs(int.Parse($"{Seed[15]}"));
        AddVectorObs(int.Parse($"{Seed[16]}"));
        AddVectorObs(int.Parse($"{Seed[17]}"));
        AddVectorObs(int.Parse($"{Seed[18]}"));
        AddVectorObs(int.Parse($"{Seed[19]}"));

        observationTime = DateTime.UtcNow - startTime;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        startTime = DateTime.UtcNow;
        MazeNumber++;

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
            // Reward(-1f);
            // Done();
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
        generationTime = DateTime.UtcNow - startTime;

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
                MazeNumber.ToString(),
                MazeRows.ToString(),
                MazeColumns.ToString(),
                $"[{Seed}]",
                MazeComplexity.ToString(), // TO DO
                observationTime.TotalMilliseconds.ToString(),
                generationTime.TotalMilliseconds.ToString()
            };
            DataRecorder.WriteRecordToCSV(data, CSVFilePath);
        }

        MazeGeneratorArea.MazeGeneratorPlayerAgent.mazeReady = true;
    }


    public override void AgentReset()
    {
        // Destroy all walls
        if (mazeCells != null && mazeCells.Length > 0)
        {
            PcgHKMG.DestroyMaze(mazeCells);
        }
        // Clear parameters
        startTime = DateTime.UtcNow;
        RequestDecision();
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
