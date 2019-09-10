using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MLAgents;

public class MazeGeneratorArea : Area
{
    public MazeGeneratorPlayerAgent MazeGeneratorPlayerAgent;
    public MazeGeneratorAgent MazeGeneratorAgent;
    public GameObject PlayerGoal;

    public TextMeshPro Score;
    public int levelCompleted = 0;
    public int scoreTotal = 0;
    public int penaltyThreshold = 0;
    public int score = 0;

    public override void ResetArea()
    {
        // Reset Maze
        MazeGeneratorAgent.AgentReset();
       
        Score.text = $"Score: {score}";
    }
}
