using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  //need for UI
using UnityEngine.UI;

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

    //references just so we can deactivate them
    private Player player;    //this is a reference to the player script that is attached to the dinoAi
    private Spawner spawner;

    //get reference to UI components 
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hightScoreText;
    public Button retryButton;

    //the score which will increase by time
    private float score;

  

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
        player = FindObjectOfType<Player>();
        spawner = FindObjectOfType<Spawner>();


        NewGame();
    }

    public void NewGame()
    {
        //clean up all obstacle in the scene
        Obstacle[] obstacles = FindObjectsOfType <Obstacle> ();

        foreach(var obstacle in obstacles)
        {
            //if we did not have .gameobject it would just destory the script not the game object
            Destroy(obstacle.gameObject);
        }

        gameSpeed = initialGameSpeed;
        //set score back to 0
        score = 0f;
        //endable game manger
        enabled = true;

       //get palyer and spawner active for new game
        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);

        //turn game over ui off on new game
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);

        UpdateHighScore();
    }

    public void GameOver()
    {
        //reset game speed
        gameSpeed = 0;
        //disable game manager
        enabled= false;

        //disable the Player/Ai and spawner
        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);

        //enable game over ui 
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);

        UpdateHighScore();
    }

    private void Update()
    {
        //gradually increase game speed 
        gameSpeed += gameSpeedIncrease * Time.deltaTime;

        //update game score based on how long the dino is alive
        //as the game goes on longer increase score faster
        score += gameSpeed * Time.deltaTime;    //could change to 1* time.delta time for constant incrnease in score

        //update score ui. floor to int just means to round down 
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");  //D5 makes it alwasys display 5 digits
    }

    private void UpdateHighScore()
    {
        //palyer prefs will store data between game session
        //get the score 
        float highScore = PlayerPrefs.GetFloat("highScore", 0);  //the second parameter is the deafult if there is no score saved

        //update high score to be new highsocre
        if(score> highScore)
        {
            highScore= score;
            //update saved float to highscore
            PlayerPrefs.SetFloat("highScore", highScore);
        }

        hightScoreText.text = Mathf.FloorToInt(highScore).ToString("D5");
    }

}
