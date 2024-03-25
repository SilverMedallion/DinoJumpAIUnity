using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class DinoRunnerAgent : Agent
{


    //Dino Data////////////////////////////////////////////////////
    //reference to character controller. 
    [SerializeField]
    CharacterController character;

   
    //gravity for the dino 
    [SerializeField]
    float gravity = 19.6f;

    //force of dinos jumps
    [SerializeField]
    float jumpForce = 8f;

    //movement vector
    Vector3 direction;

    public override void Initialize()
    {
        //set reference to character controller
        character= GetComponent<CharacterController>();
    }

    //works similar to onEnable, want this stuff to be called every time we start a new attempt
    public override void OnEpisodeBegin()
    {
        //set the direction to zero on spawn
        direction = Vector3.zero;
    }

    //function is used to tell the agent what it can see. essentially giving it values and what these values do 
   //note that this works the same way as update so calculations should be done here and not update as it is synced 
   //to the agents decisions steps
    public override void CollectObservations(VectorSensor sensor)
    {
       if(GameManager.Instance.obstacles.Count > 0)
        {
            //the first array or the list should be the closest obstacle. if add it so bird can overtake will need to sort manually
            GameObject nextObstacle = GameManager.Instance.obstacles[0];

            //set distance from the dino to the next obstacle
            float distanceToNextObstacle = Vector3.Distance(transform.position, nextObstacle.transform.position);

             //give the dino the distance to the next obstacle
             sensor.AddObservation(distanceToNextObstacle);


            //get the height of the obastcle
            BoxCollider nextObstacleCollider = nextObstacle.GetComponent<BoxCollider>();
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

        //give the dino if it is jumping or not
        sensor.AddObservation(character.isGrounded);

       
        

    }


    //called when an action is recieved from the player input or the neural network 
    
    public override void OnActionReceived(ActionBuffers actions)
    {


        //apply gravity every frame
        direction.y += gravity * Time.deltaTime;

        //check if character is gounded using character cotroller butilt in function
        if (character.isGrounded)
        {
            //if the player is grounded stop applying graviy 
            direction.y = 0;

            //check if the action is to jump
            int action = actions.DiscreteActions[0];
            if(action == 1)
            {
                //apply jump force
                direction.y = jumpForce;
                
            }
           

        }
        //actually move the character based on direction
        character.Move(direction * Time.deltaTime);
    }

    //need to set the behaviour type in behaviour parametrs to heuristic only for user to controller the agent. set to default for ai
    public override void Heuristic(in ActionBuffers actionsOut)
    {
       
        //get reference to actions
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        
        if (Input.GetButton("Jump"))  //jump is auto mapped to space in untiy input manager
        {
            //action 1 is mapped to jump
            discreteActions[0] = 1;
        }
        else
        {
            //action 0 is do nothing 
            discreteActions[0] = 0;
        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the dino jumps over the target
        if (collision.CompareTag("Passed"))
        {
            //add reward when reaching target
            AddReward(10);

        }//if the dino hits an obstacle
        else if (collision.CompareTag("Obstacle"))
        {
            //punish dino when hitting obstacle
            AddReward(-2);
           //end episode to start again
            EndEpisode();
        }
    }
}
