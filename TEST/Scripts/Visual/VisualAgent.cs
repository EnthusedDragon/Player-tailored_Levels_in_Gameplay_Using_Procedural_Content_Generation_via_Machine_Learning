using UnityEngine;
using MLAgents;

public class VisualAgent : Agent
{
    [Header("Specific to VisualAgent")]
    public GameObject Platform;
    [SerializeField]
    private bool inControll = false;
    [SerializeField]
    private bool show = false;
    [SerializeField]
    private string AiCorrect = "";

    public override void InitializeAgent()
    {
        SetResetParameters();
    }

    public override void CollectObservations()
    {

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {        
        var value = (int)vectorAction[0];

        if (Platform.activeSelf && value == 1)
        {
            SetReward(1f);
            AiCorrect = "ON";
            AgentReset();
            return;
        }

        if (!Platform.activeSelf && value == 0)
        {
            SetReward(1f);
            AiCorrect = "OFF";
            AgentReset();
            return;
        }

        if((Platform.activeSelf && value == 0) || (!Platform.activeSelf && value == 1))
        {
            SetReward(-1f);
            AiCorrect = "WRONG";
            AgentReset();
        }
    }

    public override void AgentReset()
    {
        SetResetParameters();
    }

    public void SetResetParameters()
    {
        if (!inControll)
        {
            var rand = Random.Range(0, 2);

            if (rand == 1)
                Platform.SetActive(true);
            else
                Platform.SetActive(false);
        }else
        {
            Platform.SetActive(show);
        }
    }
}
