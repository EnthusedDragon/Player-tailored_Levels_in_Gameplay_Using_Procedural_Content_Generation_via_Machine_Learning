using UnityEngine;
using MLAgents;

public class MazePlayerAcademy : Academy
{
    private MazePlayerArea[] mazePlayerAreas;

    public override void AcademyReset()
    {
        if (mazePlayerAreas == null)
        {
            mazePlayerAreas = FindObjectsOfType<MazePlayerArea>();
        }

        foreach (MazePlayerArea mazePlayerArea in mazePlayerAreas)
        {
            mazePlayerArea.minimumDistanceFromPlayerGoal = 0.1f;
            mazePlayerArea.size = (int)resetParameters["size"];
            mazePlayerArea.ResetArea();
        }
    }
}
