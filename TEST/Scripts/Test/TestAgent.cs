using UnityEngine;
using MLAgents;

public class TestAgent : Agent
{
    [Header("Specific to TEST")]
    public TextMesh FirstNumber;
    public TextMesh SecondNumber;
    public TextMesh ActualTotal;
    public TextMesh TestTotal;
    public MeshRenderer Platform;
    private ResetParameters resetParams;
    private int firstNumber;
    private int secondNumber;
    private int actualTotal;
    [SerializeField]
    private Academy academy;

    public override void InitializeAgent()
    {
        resetParams = academy.resetParameters;
        SetResetParameters();
    }

    public override void CollectObservations()
    {
        AddVectorObs(firstNumber);
        AddVectorObs(secondNumber);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (IsDone())
            return;
        
        var value = (int)vectorAction[0]+1;
        TestTotal.text = value.ToString();

        if (actualTotal == value)
        {
            Platform.material.color = Color.green;
            Done();
            AddReward(2f);
        }
        else
        {
            var percentageWrong = (Mathf.Abs(actualTotal - value) / actualTotal) * 100;
            AddReward(-0.02f * percentageWrong);
            Platform.material.color = Color.red;
        }
    }

    public override void AgentReset()
    {
        //Reset the parameters when the Agent is reset.
        SetResetParameters();
    }

    public void SetResetParameters()
    {
        firstNumber = Random.Range(1, 5);
        FirstNumber.text = firstNumber.ToString();

        secondNumber = Random.Range(1, 5);
        SecondNumber.text = secondNumber.ToString();

        actualTotal = firstNumber + secondNumber;
        ActualTotal.text = actualTotal.ToString();


        Platform.material.color = Color.grey;
    }
}
