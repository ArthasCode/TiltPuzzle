using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using TreeEditor;
using System;

public class AgentTable : Agent
{
    [SerializeField] private HoleFall currentHoleFall;
    private GameObject levelNow;
    [SerializeField]private LevelManager levelManager;
    Vector3 rotacaoCalc;
    Vector3 inclinacaoMin;
    Vector3 inclinacaoMax;

    private int difficultSet;

    public override void CollectObservations(VectorSensor sensor)
    {
        Transform ball = FindChildWithTag("Ball");
        float dist = Vector3.Distance(currentHoleFall.transform.position, ball.position) / 28.284271247f;
        sensor.AddObservation(dist);

        float groundAngleX = levelNow.transform.eulerAngles.x;
        if (groundAngleX > 180f) 
            groundAngleX -= 360f;
        float normalizedAngleX = Math.Clamp(groundAngleX / inclinacaoMax.x, -1f, 1f);

        float groundAngleZ = levelNow.transform.eulerAngles.z;
        if (groundAngleZ > 180f) 
            groundAngleZ -= 360f;
        float normalizedAngleZ = Math.Clamp(groundAngleZ / inclinacaoMax.z, -1f, 1f);

        sensor.AddObservation(normalizedAngleX);
        sensor.AddObservation(normalizedAngleZ);

        // Debug.Log($"[ML-Agents Obs] Distance: {dist:F2}" +
        //     $"| AngleX: {normalizedAngleX:F2}" + 
        //     $"| AngleZ: {normalizedAngleZ:F2}");
    }

    private Transform FindChildWithTag(string tag)
    {
    foreach (Transform child in transform)
    {
        if (child.CompareTag(tag))
        {
            return child; // Retorna o primeiro filho encontrado
        }
    }
    return null;
    }
    void FixedUpdate()
    {
        if(currentHoleFall.BallEntered){
            if(!levelManager.NextLevel(BallPositionOnEnvironment())){
                levelManager.levelNow = 0;
                levelManager.SetInitialLevel(BallPositionOnEnvironment());
            }
            AddReward(1f);
            EndEpisode();

            currentHoleFall.ResetTrigger();
        }
    }
    public override void OnEpisodeBegin()
    {
        levelNow = levelManager.levels[levelManager.levelNow]
            .transform.GetChild(0).gameObject;
        currentHoleFall = levelNow.GetComponentInChildren<HoleFall>();

        // For later possibly curriculum
        difficultSet = (int) Academy.Instance.EnvironmentParameters.GetWithDefault("ball_position", 3);

        TableController levelNowGroundScript = levelNow.GetComponent<TableController>();
        if (StepCount != 0)
            levelManager.RespawnBall(BallPositionOnEnvironment());
        else
            levelManager.SetInitialLevel(BallPositionOnEnvironment());
        rotacaoCalc = levelNowGroundScript.rotacaoCalc;
        inclinacaoMin = levelNowGroundScript.inclinacaoMin;
        inclinacaoMax = levelNowGroundScript.inclinacaoMax;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int actX = actions.DiscreteActions[0];
        int actZ = actions.DiscreteActions[1];

        float taxaInclinacao = levelNow.GetComponent<TableController>().taxaInclinacao;
        
        
        Vector3 alterInclinacao = new Vector3();
        switch (actX)
        {
            case 1:
                alterInclinacao.x += taxaInclinacao * Time.fixedDeltaTime;
                break;
            case 2:
                alterInclinacao.x -= taxaInclinacao * Time.fixedDeltaTime;
                break;
        }
        
        switch (actZ)
        {
            case 1:
                alterInclinacao.z += taxaInclinacao * Time.fixedDeltaTime;
                break;
            case 2:
                alterInclinacao.z -= taxaInclinacao * Time.fixedDeltaTime;
                break;
        }

        rotacaoCalc += alterInclinacao;

        rotacaoCalc = Vector3.Min(Vector3.Max(rotacaoCalc, inclinacaoMin),inclinacaoMax);

        levelNow.transform.localEulerAngles = rotacaoCalc;

        // penalty for step
        AddReward(-1f/ MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
        ActionSegment<int> action = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            action[0] = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            action[0] = 2;
        }
        else
        {
            action[0] = 0;
        }


        if (Input.GetKey(KeyCode.LeftArrow))
        {
            action[1] = 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            action[1] = 2;
        }
        else
        {
            action[1] = 0;
        }
    }

    private Vector3 BallPositionOnEnvironment()
    {
        Vector3 holePosition = currentHoleFall.transform.position;
        Vector3 newPos;
        Vector3 positionChange;
        switch (difficultSet)
        {
            case 1:
                positionChange = new Vector3(0, 0, 5f);
                newPos = holePosition + positionChange;

                break;
            case 2:
                positionChange = new Vector3(3f, 0, 5f);
                newPos = holePosition + positionChange;

                break;
            case 3:
                positionChange = new Vector3(5f, 0, 10f);
                newPos = holePosition + positionChange;

                break;
            default:
                positionChange = new Vector3(5f, 0, 10f);
                newPos = holePosition + positionChange;
                break;
        }

        return newPos;
    }
}
