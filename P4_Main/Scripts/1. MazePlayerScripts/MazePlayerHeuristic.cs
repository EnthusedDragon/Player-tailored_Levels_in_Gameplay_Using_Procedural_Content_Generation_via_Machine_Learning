using UnityEngine;
using MLAgents;
using System.Collections.Generic;

public class MazePlayerHeuristic : Decision
{
    private MazeGeneratorPlayerAgent MazePlayerAgent;
    
    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {        
        if (MazePlayerAgent == null)
        {
            MazePlayerAgent = GameObject.FindGameObjectWithTag("HeuristicAgent").GetComponent<MazeGeneratorPlayerAgent>();
        }

        var nexPos = new Vector3(vectorObs[0], vectorObs[1], vectorObs[2]);
        
        if (MazePlayerAgent.currentCell.topCell != null && MazePlayerAgent.currentCell.topCell.cellParentAndLocation.transform.position == nexPos)
        {
            return new float[1] { 1f };
        }
        else if (MazePlayerAgent.currentCell.bottomCell != null && MazePlayerAgent.currentCell.bottomCell.cellParentAndLocation.transform.position == nexPos)
        {
            return new float[1] { 2f };
        }
        else if (MazePlayerAgent.currentCell.leftCell != null && MazePlayerAgent.currentCell.leftCell.cellParentAndLocation.transform.position == nexPos)
        {
            return new float[1] { 3f };
        }
        else if (MazePlayerAgent.currentCell.rightCell != null && MazePlayerAgent.currentCell.rightCell.cellParentAndLocation.transform.position == nexPos)
        {
            return new float[1] { 4f };
        }

        return new float[1] { 0f };
    }

    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        return new List<float>();
    }
}
