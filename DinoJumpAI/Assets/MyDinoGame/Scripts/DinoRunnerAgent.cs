using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class DinoRunnerAgent : Agent
{


    //Dino Data////////////////////////////////////////////////////
    //reference to rigidbody2d. set in editor
    
    public Rigidbody2D rigidbody2d;

    //reference to box collider2d set in editor
    public BoxCollider2D boxCollider;

    //gravity for the dino 
    //[SerializeField]
   // float gravity = 2f;

    //force of dinos jumps
    [SerializeField]
    float jumpForce = 8f;

    //movement vector
    Vector3 velocity;
    
    //track if dino is in the air or not
    bool isGrounded = false;

   

    //works similar to onEnable, want this stuff to be called every time we start a new attempt
    public override void OnEpisodeBegin()
    {
        //set the dino direction to zero on spawn
        velocity = Vector2.zero;
        //destory all obstacles still active, reset game speed, reset score, set dino and spawnner to active
        GameManager.Instance.NewGame();
    }

    //function is used to tell the agent what it can see. essentially giving it values and what these values do 
    //note that this works the same way as update so calculations should be done here and not update as it is synced 
    //to the agents decisions steps
    /*
     public override void CollectObservations(VectorSensor sensor)
     {
         //check that an obstacle is spawned in and added to the list in game manager
        if(GameManager.Instance.obstacles.Count > 0)
         {
             Debug.LogWarning("There are more than 0 obstacles in the list");
             //the first array or the list should be the closest obstacle. if add it so bird can overtake will need to sort manually
             GameObject nextObstacle = GameManager.Instance.obstacles[0];

             //set distance from the dino to the next obstacle
             float distanceToNextObstacle = Vector3.Distance(transform.position, nextObstacle.transform.position);

              //give the dino the distance to the next obstacle
              sensor.AddObservation(distanceToNextObstacle);


             //get the height of the obastcle
             BoxCollider2D nextObstacleCollider = nextObstacle.GetComponent<BoxCollider2D>();
             //make sure there is a collider before added the observation
             if(nextObstacleCollider != null)
             {
                 //get the height of the collider
                 float nextObstacleHeight = nextObstacleCollider.size.y * nextObstacle.transform.localScale.y;
                 //add the height of the next obstacle as an observation 
                 sensor.AddObservation(nextObstacleHeight);
             }
             else
             {
                 //just incase there is no collider for some reason:
                 sensor.AddObservation(0);
             }



         }else
         {
             //need to still prvoide an oberservation as the neural network expects a fixed number of inputs. different inputs at each step would cause problems 
             //allows the agents to tell when there is no object ahead 

             //no obstacle in the scene so no distance of height
             sensor.AddObservation(0);
             sensor.AddObservation(0);

         }

         //how high the next incoming obstacle is


         //give the dino it's own position
         sensor.AddObservation(transform.position.y);

         //give the dino the speed at which the game is moving at
         sensor.AddObservation(GameManager.Instance.gameSpeed);





     }
    */

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(GameManager.Instance.gameSpeed);
        sensor.AddObservation(rigidbody2d.velocity.y);
        sensor.AddObservation(transform.position.y);
    }


    //called when an action is recieved from the player input or the neural network 

    public override void OnActionReceived(ActionBuffers actions)
    {

        Debug.LogWarning("ON ACTIONS RECEIVED IS AT LEAST BEING CALLED");
        //apply gravity every frame
      

        //check if character is gounded using character cotroller butilt in function
        if (isGrounded)
        {
           

            //check if the action is to jump
            int action = actions.DiscreteActions[0];
            if(action == 1)
            {
                Debug.Log("jump force is being applied in Onaction recieved");
                //apply jump force
                Jump();
                
           }
           

        }

        //actually move the character based on direction
        //rigidbody2d.velocity = velocity;
        Debug.Log("rigidbody2d move position should have been called");
    }

    //need to set the behaviour type in behaviour parametrs to heuristic only for user to controller the agent. set to default for ai
    public override void Heuristic(in ActionBuffers actionsOut)
    {
       
        //get reference to actions
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        
        if (Input.GetButton("Jump") ) //jump is auto mapped to space in untiy input manager
        {
            //action 1 is mapped to jump
            Debug.LogWarning("heuristic is being pick up as space being down");
            discreteActions[0] = 1;
        }
        else
        {
            Debug.LogWarning("heuristic is being picked up as space not being down");
            //action 0 is do nothing 
            discreteActions[0] = 0;
        }


    }


    //check if obstacle is hit or passed
    /*
    private void OnTriggerEnter2D(Collider collision)
    {

        //if the dino jumps over the target
        if (collision.TryGetComponent(out JumpedOver jumpedOver))
        {
            //add reward when reaching target
            AddReward(10);
            Debug.Log("reward should be added as dino hit reward collider");

        }//if the dino hits an obstacle
        else if (collision.TryGetComponent(out Obstacle obstacle))
        {
            Debug.LogWarning("Dino collided with Obstacle");
            //punish dino when hitting obstacle
            AddReward(-2);
           //end episode to start again
            EndEpisode();
        }
    }
    */


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            AddReward(-1f);
            EndEpisode();
        }
        if (other.CompareTag("JumpedOver"))
        {

            Debug.Log("reward");
            AddReward(0.2f);
        }

    }


private void OnCollisionEnter2D(Collision2D collision)
    {
        
        //if dino is colliding with the ground set grounded to true
        if (collision.gameObject.TryGetComponent(out Ground ground))
        {
            Debug.LogWarning("dino has collided with ground and grounded set to true");
            isGrounded = true;
        }
    }

 
    private void OnCollisionExit2D(Collision2D collision)
    {
        //set grounded to false when jumping from the ground 
        if (collision.gameObject.TryGetComponent(out Ground ground))
        {
            Debug.LogWarning("dino is no longer colliding with ground and grounded is false");
            isGrounded = false;
        }
    }

    private void Jump()
    {
        rigidbody2d.velocity = Vector2.up * jumpForce;

    }
}
