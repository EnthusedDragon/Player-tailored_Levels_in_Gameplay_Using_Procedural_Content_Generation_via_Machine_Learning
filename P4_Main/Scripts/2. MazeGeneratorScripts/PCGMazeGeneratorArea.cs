using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MLAgents;

public class PCGMazeGeneratorArea : Area
{
    public PCGMazeGeneratorPlayerAgent PCGMazeGeneratorPlayerAgent;
    public PCGMazeGenerator PCGMazeGenerator;
    public GameObject PlayerGoal;

    public TextMeshPro Score;
    public int levelCompleted = 0;
    public int scoreTotal = 0;
    public int penaltyThreshold = 0;
    public int TotalScore = 0;
    public int AverageScore = 0;
    public int score = 0;
    public float complexity = 0;

    // RESEARCH DATA
    public bool PlayerFileCreated = false;
    public bool GeneratorFileCreated = false;

    public override void ResetArea()
    {
        // Reset Maze       
        Score.text = $"Complexity: {complexity}";

        PCGMazeGenerator.AgentReset();
    }
}
