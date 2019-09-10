using UnityEngine;
using MLAgents;
using System;

public class MazePlayerAgent : Agent
{
    public float PlayerMoveDistance = 5.4f;

    private MazePlayerArea MazePlayerArea;
    private RayPerception3D RayPerception3D;
    private GameObject PlayerGoal;

    public MazeCell currentCell;
    private int lastDistanceFromEnd;
    private int newDistanceFromEnd;

    private int stepsTaken;

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
                else
                {
                    //Debug.Log("Can't move through a wall.");
                    AddReward(-0.5f / agentParameters.maxStep);
                }
                break;
            case 2: // DOWN         
                //Debug.Log("DOWN");
                if (currentCell.bottomCell != null && currentCell.bottomWall == null && currentCell.bottomCell.topWall == null)
                {
                    currentCell = currentCell.bottomCell;
                    transform.position = currentCell.cellParentAndLocation.transform.position;
                }
                else
                {
                    //Debug.Log("Can't move through a wall.");
                    AddReward(-0.5f / agentParameters.maxStep);
                }
                break;
            case 3: // LEFT         
                //Debug.Log("LEFT");
                if (currentCell.leftCell != null && currentCell.leftWall == null && currentCell.leftCell.rightWall == null)
                {
                    currentCell = currentCell.leftCell;
                    transform.position = currentCell.cellParentAndLocation.transform.position;
                }
                else
                {
                    //Debug.Log("Can't move through a wall.");
                    AddReward(-0.5f / agentParameters.maxStep);
                }
                break;
            case 4: // RIGHT         
                //Debug.Log("RIGHT");
                if (currentCell.rightCell != null && currentCell.rightWall == null && currentCell.rightCell.leftWall == null)
                {
                    currentCell = currentCell.rightCell;
                    transform.position = currentCell.cellParentAndLocation.transform.position;
                }
                else
                {
                    AddReward(-0.5f / agentParameters.maxStep);
                    //Debug.Log("Can't move through a wall.");
                }
                break;
            case 0: // DO NOTHING
                AddReward(-1f / agentParameters.maxStep);
                //Debug.Log("Do nothing.");
                break;
        }

        newDistanceFromEnd = MazePlayerArea.DistanceToEnd(currentCell, FromCell.player);


        if (newDistanceFromEnd < lastDistanceFromEnd)
        {
            AddReward(0.5f / agentParameters.maxStep);
        }

        lastDistanceFromEnd = newDistanceFromEnd;
    }

    //private void CalculateAndMoveToNewPosition(int row, int col)
    //{
    //    if (row < 0 || row > MazePlayerArea.mazeRows - 1 || col < 0 || col > MazePlayerArea.mazeColumns - 1)
    //    {
    //        //AddReward(-0.2f);
    //        Debug.Log("Can't move out of bounds.");
    //        return;
    //    }

    //    var newPos = MazePlayerArea.mazeCells[row, col].cellParentAndLocation.transform.position;
    //    if (Physics.Linecast(transform.position, newPos, out RaycastHit raycastHit))
    //    {
    //        if (raycastHit.transform.CompareTag("wall"))
    //        {
    //            Debug.Log("Can't move through a wall.");
    //            //AddReward(-0.2f);
    //            return;
    //        }
    //        else
    //        {
    //            Debug.Log($"Unexpected hit of {raycastHit.transform.tag}");
    //        }
    //    }

    //    transform.position = newPos;
    //    currentCellRow = row;
    //    currentCellCollumn = col;
    //    newDistanceFromEnd = MazePlayerArea.DistanceToEnd(MazePlayerArea.mazeCells[currentCellRow, currentCellCollumn], FromCell.player);
    //    Debug.Log($"DISTANCE FROM PLAYER TO END: {newDistanceFromEnd}");
    //}

    public override void AgentReset()
    {
        MazePlayerArea.ResetArea();
        lastDistanceFromEnd = 0;
        newDistanceFromEnd = 0;
        stepsTaken = 0;
    }

    public override void CollectObservations()
    {
        // change to cell distance from goal
        //AddVectorObs(distanceFromEnd);
        AddVectorObs(NextCellToEnd(currentCell, FromCell.player).cellParentAndLocation.transform.position);
        AddVectorObs(currentCell.cellParentAndLocation.transform.position);

        float rayDistance = 3f;
        float[] rayAngles = { 0f, 90f, 180f, 270f };
        string[] detectableObjects = { "wall" };
        AddVectorObs(RayPerception3D.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));

    }

    private void Start()
    {
        MazePlayerArea = GetComponentInParent<MazePlayerArea>();
        PlayerGoal = MazePlayerArea.PlayerGoal;
        RayPerception3D = GetComponent<RayPerception3D>();
    }

    private void FixedUpdate()
    {
        // Change to cell distance from goal
        if (Vector3.Distance(transform.position, PlayerGoal.transform.position) < MazePlayerArea.minimumDistanceFromPlayerGoal)
        {
            float score = MazePlayerArea.scoreTotal + (MazePlayerArea.penaltyThreshold - stepsTaken);
            float percentage = score / MazePlayerArea.scoreTotal;
            int finalScore = Mathf.RoundToInt(percentage * 100f);
            MazePlayerArea.score = (finalScore < 0 ? 0 : finalScore > 100 ? 100 : finalScore);
            Done();            
            AddReward(1f);
            MazePlayerArea.levelCompleted++;
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
