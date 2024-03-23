using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   //this game manager will be a simple singleton, it's not a true singleton
   //anyone can access this instacnce but only this class can manage the private instance
   public static GameManager Instance {  get; private set; }

    //starting game speed
    public float initialGameSpeed = 5f;
    //current game speed
    public float gameSpeed {  get; private set; }
    //how fast the game speed increases in seconds
    public float gameSpeedIncrease = 0.1f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }else
        {
            //if for whatever reason a second instacne is created immediatlry destroy
            DestroyImmediate(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        gameSpeed = initialGameSpeed;
    }

    private void Update()
    {
        //gradually increase game speed 
        gameSpeed += gameSpeedIncrease * Time.deltaTime;
    }


}
