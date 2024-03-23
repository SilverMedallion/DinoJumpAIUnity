using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{

    //reference to character controller
    private CharacterController character;

    //movement vector
    private Vector3 direction;

    //gravity for the dino 
    [SerializeField]
    float gravity = 19.6f;

    [SerializeField]
    float jumpForce = 8f;

    private void Awake()
    {
        //assign reference to character controller attached to the dino
        character = GetComponent<CharacterController>();
    }

    //want this stuff to be called each time we enable, so will run again after a disable 
    private void OnEnable()
    {
        //set the direction to zero on spawn
        direction = Vector3.zero;
    }


    private void Update()
    {
        //pull the player down with gravity which will increase over time
        direction += Vector3.down * gravity * Time.deltaTime;

        //check if character is gounded using character cotroller butilt in function
        if (character.isGrounded)
        {
            //apply constant gravity when the player is grounded
            direction = Vector3.down;

            //get if button is pressed for the single frame it was used. Get button allows for hold down
            if (Input.GetButton("Jump"))             //jump is auto mapped to space bar in the unity input manager
            {
                direction = Vector3.up * jumpForce;
            }
            
            
        }
        //actually move the character based on direction
        character.Move(direction * Time.deltaTime);
    }
}
