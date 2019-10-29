using UnityEngine;
using MLAgents;

public class MazeGeneratorAcademy : Academy
{
    private MazeGeneratorArea[] mazeGeneratorAreas;

    public override void AcademyReset()
    {
        if (mazeGeneratorAreas == null)
        {
            mazeGeneratorAreas = FindObjectsOfType<MazeGeneratorArea>();
        }

        foreach (MazeGeneratorArea mazeGeneratorArea in mazeGeneratorAreas)
        {
            mazeGeneratorArea.ResetArea();
        }
    }
}
