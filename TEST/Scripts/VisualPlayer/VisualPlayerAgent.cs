using UnityEngine;
using MLAgents;
using System.Collections.Generic;

public class VisualPlayerAgent : Agent
{
    [Header("Specific to VisualPlayerAgent")]
    public int steps = 0;
    public Rigidbody Player;
    public float PlayerMoveSpeed;
    public GameObject Goal;
    public float DistanceThreshold = 0f;
    public float SpawnAreaOffset = 4.5f;
    public Vector2 SpawnAreaSize = new Vector2(9f, 4.5f);
    [SerializeField]
    private float Distance = 0f;
    private float OriginalDistance = 0f;

    private DataRecorder dataRecorder;
    private int screenshotCounter = 0;

    public override void InitializeAgent()
    {
        //dataRecorder = new DataRecorder("D:\\", "newOutput.csv");
        //dataRecorder.AddRecord(new List<string> { "TEST 1", "TEST 2" });
        SetResetParameters();
    }

    public override void CollectObservations()
    {
        //dataRecorder.AddRecord(new List<string> { "DATA 1", "DATA 2" });
        //dataRecorder.Screenshot("D:\\Screenshots\\", $"newOutput{screenshotCounter++}.png");
        //AddVectorObs(Vector3.Distance(Player.transform.position, Goal.transform.position));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        Distance = Vector3.Distance(Player.transform.position, Goal.transform.position);
        var value = (int)vectorAction[0];

        switch (value)
        {
            case 4: // FORWARD
                Player.AddForce(PlayerMoveSpeed * Vector3.right);
                break;
            case 3: // BACKWARD
                Player.AddForce(PlayerMoveSpeed * Vector3.left);
                break;
            case 1: // UP
                Player.AddForce(PlayerMoveSpeed * Vector3.up);
                break;
            case 2: // DOWN
                Player.AddForce(PlayerMoveSpeed * Vector3.down);
                break;
            case 0: // DO NOTHING
                break;
            default: // ERROR
                Debug.LogError("INVALID OUTPUT");
                break;
        }

        if (Distance <= DistanceThreshold)
        {
            Done();
            SetReward(1f);
        }

        var punishment = (Distance / OriginalDistance) * -1;

        AddReward(punishment / steps);
    }

    public override void AgentReset()
    {
        SetResetParameters();
    }

    public void SetResetParameters()
    {
        RandomSpawn(Player.gameObject, -SpawnAreaOffset);
        RandomSpawn(Goal, SpawnAreaOffset);
        Player.velocity = Vector3.zero;
        OriginalDistance = Vector3.Distance(Player.transform.position, Goal.transform.position);
    }

    private void RandomSpawn(GameObject gameObject, float yOffset)
    {
        var basePos = transform.position;
        basePos.y += yOffset / 2;
        var spawnLocation = basePos + new Vector3(Random.Range(-SpawnAreaSize.x / 2, SpawnAreaSize.x / 2), Random.Range(-SpawnAreaSize.y / 2, SpawnAreaSize.y / 2), gameObject.transform.position.z);

        gameObject.transform.position = spawnLocation;
    }
}
