using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToTargetAgent : Agent
{
    //must be set in editor
    [SerializeField]
    Transform env;

    //reference to target position, set in editor
    [SerializeField]
    Transform target;

    //set in editor 
    [SerializeField]
    SpriteRenderer backgroundSpriteRenderer;

    //works similar to a start functino
    public override void OnEpisodeBegin()
    {
        //reset the transforms for the agent and the target
        //set the starting values to be random each time
        transform.localPosition = new Vector3(Random.Range(-3.5f, -1.5f), Random.Range(-3.5f, 3.5f));
        target.localPosition = new Vector3(Random.Range(1.5f, 3.5f), Random.Range(-3.5f, 3.5f));

        //rotate randomly between 0 and 360 on the z this is used so we can train the agent to work no matter where the target is
        env.rotation= Quaternion.Euler(0, 0, Random.Range(0f, 360f  )   );
        //don't use local here as, need to look at difference between LocalRotation and Roation using heuristic
        transform.rotation = Quaternion.identity;
    }

    //function is used to tell the agent what it can see. essentially giving it values and what these values do 

    //note that it needs 4 vector observations. 2 for the agents x and y. 2 for the targets x and y 
   
    public override void CollectObservations(VectorSensor sensor)
    {
        //give the agent it's own position
        sensor.AddObservation((Vector2)transform.localPosition);

        //give the agent the target position
        sensor.AddObservation((Vector2)target.localPosition);

    }


    //whenever the agent takes an action this function is called
    public override void OnActionReceived(ActionBuffers actions)
    {
        //code to move the agent
        //define continous actions for the agent
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float movementSpeed = 5;

        //NOTE: transform position has a problem with MLAgent
        //if we have multiple envirmoments then when one agent finds out how to reach it's goal in it's enviromnet then other agnets
        //in other environments will move towards the goal that is not in their environment
        //using transform.localPosition will tell the agent to move in local space instead of global stopping this issue
        transform.localPosition += new Vector3(moveX, moveY) * movementSpeed * Time.deltaTime;
    }

    //need to set the behaviour type in behaviour parametrs to heuristic only for user to controller the agent. set to default for ai
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // when pressing a and d or w and s move side to side and up and down
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //code if the agent passes the episode
        if(collision.TryGetComponent(out Target target))
        {
            //add reward when reaching target
            AddReward(10);
            //used to show easily when we pass
            backgroundSpriteRenderer.color = Color.green; 
            //training is multiple episodes, each episode he either fails or suceeeds. here he succeeding
            EndEpisode();
        }//code if the agent fails the episode
        else if(collision.TryGetComponent(out Wall wall))
        {
            AddReward(-2);
            //used ot show easily when we fail
            backgroundSpriteRenderer.color = Color.red;
            EndEpisode();
        }
    }
}

