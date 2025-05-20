using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToTargetAgent : Agent
{
    //venv\scripts\activate
    //mlagents-learn --run--id=""
    [SerializeField] private Transform target;
    [SerializeField] private GameObject[] targetPos;

    GameObject Pos;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(10f, 4f);
        Pos = targetPos[Random.Range(0, targetPos.Length-1)];
        target.localPosition = Pos.transform.localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.localPosition);
        sensor.AddObservation((Vector2)target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float moveSpeed = 10f;

        transform.localPosition += new Vector3(moveX, moveY) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Target target))
        {
            AddReward(5f);
            EndEpisode();
        }
        else if (collision.TryGetComponent(out Walls walls))
        {
            AddReward(-10f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Walls walls))
        {
            AddReward(-20f);
            EndEpisode();
        }
    }
}
