using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent2 : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform objectTransform;
    private float moveSpeed = 7f;
    private float jumpForce = 14f;
    private Vector3 startPos;
    private Vector3 objectPos;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private float startDistance;
    private float distanceFromGoal;

    [SerializeField] private LayerMask jumpableGround;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        startPos = transform.localPosition;
        startDistance = Vector3.Distance(transform.position, targetTransform.position);
        if (objectTransform != null)
        {
            objectPos = objectTransform.localPosition;
        }
    }

    private void Update()
    {
        if (StepCount == MaxStep - 1)
        {
            Debug.Log("MaxStep reached");
            RewardDistance();
        }

    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = startPos;
        if (objectTransform != null)
        {
            objectTransform.localPosition = new Vector2(Random.Range(objectPos.x - 5f, objectPos.x + 5f), objectPos.y);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        distanceFromGoal = Vector3.Distance(transform.position, targetTransform.position);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(IsGrounded());
        sensor.AddObservation(distanceFromGoal);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float jump = actions.DiscreteActions[0];

        //transform.localPosition += new Vector3(moveX, 0, 0) * Time.deltaTime * moveSpeed;
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        if (jump == 1 && IsGrounded())
        {
            Jump();
        }
        
    }

    // Allows a user to control the sprite
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        discreteActions[0] = Input.GetButtonDown("Jump") ? 1 : 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Goal"))
        {
            AddReward(3f);
            RewardDistance();
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("Trap"))
        {
            AddReward(-.25f);
            RewardDistance();
            EndEpisode();
        }
    }

    // Reward the agent based on how close they are to the goal
    public void RewardDistance()
    {
        distanceFromGoal = Vector3.Distance(transform.position, targetTransform.position);
        AddReward(1 - distanceFromGoal / startDistance);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
