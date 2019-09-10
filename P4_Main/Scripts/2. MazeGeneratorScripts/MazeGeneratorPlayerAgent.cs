﻿using UnityEngine;
using MLAgents;
using System;

public class MazeGeneratorPlayerAgent : Agent
{
    public MazeGeneratorAgent MazeGeneratorAgent;
    private MazeGeneratorArea MazeGeneratorArea;
    private RayPerception3D RayPerception3D;
    private GameObject PlayerGoal;

    public MazeCell currentCell;

    public int stepsTaken = 0;
    public float minimumDistanceFromPlayerGoal = 0.1f;

    public bool mazeReady = false;

    enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        stepsTaken++;

        switch ((int)vectorAction[0])
        {
            case 1: // UP                
                //Debug.Log("UP");
                if (currentCell.topCell != null && currentCell.topWall == null && currentCell.topCell.bottomWall == null)
                {
                    currentCell = currentCell.topCell;
                    transform.position = currentCell.cellParentAndLocation.transform.position;
                }
                break;
            case 2: // DOWN         
                //Debug.Log("DOWN");
                if (currentCell.bottomCell != null && currentCell.bottomWall == null && currentCell.bottomCell.topWall == null)
                {
                    currentCell = currentCell.bottomCell;
                    transform.position = currentCell.cellParentAndLocation.transform.position;
                }
                break;
            case 3: // LEFT         
                //Debug.Log("LEFT");
                if (currentCell.leftCell != null && currentCell.leftWall == null && currentCell.leftCell.rightWall == null)
                {
                    currentCell = currentCell.leftCell;
                    transform.position = currentCell.cellParentAndLocation.transform.position;
                }
                break;
            case 4: // RIGHT         
                //Debug.Log("RIGHT");
                if (currentCell.rightCell != null && currentCell.rightWall == null && currentCell.rightCell.leftWall == null)
                {
                    currentCell = currentCell.rightCell;
                    transform.position = currentCell.cellParentAndLocation.transform.position;
                }
                break;
            case 0: // DO NOTHING
                break;
        }
    }

    public override void AgentReset()
    {
        stepsTaken = 0;
        MazeGeneratorArea.ResetArea();
    }

    public override void AgentOnDone()
    {
        MazeGeneratorArea.ResetArea();
    }

    public override void CollectObservations()
    {
        AddVectorObs(NextCellToEnd(currentCell, FromCell.player).cellParentAndLocation.transform.position);
        AddVectorObs(currentCell.cellParentAndLocation.transform.position);

        float rayDistance = 3f;
        float[] rayAngles = { 0f, 90f, 180f, 270f };
        string[] detectableObjects = { "wall" };
        AddVectorObs(RayPerception3D.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
    }

    public override void InitializeAgent()
    {
        MazeGeneratorArea = GetComponentInParent<MazeGeneratorArea>();
        PlayerGoal = MazeGeneratorArea.PlayerGoal;
        RayPerception3D = GetComponent<RayPerception3D>();
    }

    private void FixedUpdate()
    {
        if (mazeReady)
        {
            RequestDecision();

            // Change to cell distance from goal
            if (Vector3.Distance(transform.position, PlayerGoal.transform.position) < minimumDistanceFromPlayerGoal)
            {
                float score = MazeGeneratorArea.scoreTotal + (MazeGeneratorArea.penaltyThreshold - stepsTaken);
                float percentage = score / MazeGeneratorArea.scoreTotal;
                int finalScore = Mathf.RoundToInt(percentage * 100f);
                MazeGeneratorArea.score = finalScore < 0 ? 0 : finalScore > 100 ? 100 : finalScore;
                Done();
                MazeGeneratorAgent.SetReward(1f / stepsTaken / 2);
                var targetPercentage = 70;
                var diff = Mathf.Abs(targetPercentage - MazeGeneratorArea.score);

                if (diff > 10)
                {
                    MazeGeneratorAgent.AddReward(-((diff - 10) / 100 / stepsTaken / 2));
                }

                MazeGeneratorArea.levelCompleted++;
                mazeReady = false;
            }
        }
    }

    public MazeCell NextCellToEnd(MazeCell mazeCell, FromCell fromCell)
    {
        if (mazeCell.endCell)
        {
            return mazeCell;
        }

        if (mazeCell.topCell != null && fromCell != FromCell.top)
        {
            var topCell = NextCellToEnd(mazeCell.topCell, FromCell.bottom);
            if (topCell != null && fromCell != FromCell.player)
            {
                return mazeCell;
            }
            else if (topCell != null && fromCell == FromCell.player)
            {
                return topCell;
            }
        }

        if (mazeCell.bottomCell != null && fromCell != FromCell.bottom)
        {
            var bottomCell = NextCellToEnd(mazeCell.bottomCell, FromCell.top);
            if (bottomCell != null && fromCell != FromCell.player)
            {
                return mazeCell;
            }
            else if (bottomCell != null && fromCell == FromCell.player)
            {
                return bottomCell;
            }
        }

        if (mazeCell.leftCell != null && fromCell != FromCell.left)
        {
            var leftCell = NextCellToEnd(mazeCell.leftCell, FromCell.right);
            if (leftCell != null && fromCell != FromCell.player)
            {
                return mazeCell;
            }
            else if (leftCell != null && fromCell == FromCell.player)
            {
                return leftCell;
            }
        }

        if (mazeCell.rightCell != null && fromCell != FromCell.right)
        {
            var rightCell = NextCellToEnd(mazeCell.rightCell, FromCell.left);
            if (rightCell != null && fromCell != FromCell.player)
            {
                return mazeCell;
            }
            else if (rightCell != null && fromCell == FromCell.player)
            {
                return rightCell;
            }
        }

        return null;
    }
}