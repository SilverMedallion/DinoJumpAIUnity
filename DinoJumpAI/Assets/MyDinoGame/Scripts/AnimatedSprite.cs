using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{

    //array of sprites our game objects will cycle between
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    
    //used to keep track what frame/sprite we are currently showing. so will be used for the index of the array
    private int frame;

    private void Awake()
    {
        spriteRenderer= GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        //want to start animation 
        //we cannnot just call the animate function as game speed does not get set until start is called which will ALWAYS be called AFTER OnEnable so this does not work 
        //start would not work either as it would only be called once, need this to run even after we restart
        Invoke(nameof(Animate), 0f);   //having 0 here means this will get called the very  next frame so out speed will already be set. could also make it wait for a second by changing 0 to 1 but it's better to just do it next frame
    
    
    }

    private void OnDisable()
    {
        //will stop animating and reinable when we enable again
        CancelInvoke();
    }

    private void Animate()
    {
        //increase current frame each time we animate
        ++frame; 

        //if we cycle through all our sprites go back to original
        if(frame >=sprites.Length)
        {
            //go back to first sprite
            frame= 0;
        }

        //check that the sprites array is not out of bound. stops an error when we forget to assign sprites to the array
        //stops us getting the out of bounds error
        if(frame>= 0 && frame < sprites.Length)
        {
            spriteRenderer.sprite = sprites[frame];

        }

        //want to invoke animated based on game speed
        //so as game speed in creases this function will shorten the time between cycles so the dino runs faster and faster
        Invoke(nameof(Animate), 1f / GameManager.Instance.gameSpeed); 
    }
}
