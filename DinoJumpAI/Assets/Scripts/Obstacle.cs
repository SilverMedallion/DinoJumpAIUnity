using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    //left edge of screenw ould be 0 and the right edge would we the screen resolutio
    private float leftEdgeOfScreen;

    private void Start()
    {
        //calculate the edge of screen this returns a reference to camera tagged as main 
        leftEdgeOfScreen = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 2f;   //subtracted by 1 to make sure the object is fully off the screen before being destroyed
    }

    private void Update()
    {
        //have objects move to the left based on game speed
        transform.position += Vector3.left * GameManager.Instance.gameSpeed * Time.deltaTime;

        //once object is past the edge of the screen destory it
        if(transform.position.x < leftEdgeOfScreen)
        {
            Destroy(gameObject);
        }
    }
}
