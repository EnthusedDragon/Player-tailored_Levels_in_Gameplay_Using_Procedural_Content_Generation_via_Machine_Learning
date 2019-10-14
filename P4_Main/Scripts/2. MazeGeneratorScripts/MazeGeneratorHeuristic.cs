using UnityEngine;
using MLAgents;
using System.Collections.Generic;

public class MazeGeneratorHeuristic : Decision
{
    private MazeGeneratorArea MazePlayerArea;
    private MazeGeneratorAgent MazePlayerAgent;

    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        var array = new float[22]
        {
            38,
            38,
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
            Random.Range(0,4),
        };

        return array;
    }

    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        return new List<float>();
    }
}
