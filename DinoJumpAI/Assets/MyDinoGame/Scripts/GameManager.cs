using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  //need for UI
using UnityEngine.UI;
using System.IO.Enumeration;
using System.IO;

public class GameManager : MonoBehaviour
{
   //this game manager will be a simple singleton, it's not a true singleton
   //anyone can access this instacnce but only this class can manage the private instance
   public static GameManager Instance {  get; private set; }

    //store a list of all spawned objects
    public List<GameObject> obstacles = new List<GameObject>(); 

    //starting game speed
    public float initialGameSpeed = 5f;
    //current game speed
    public float gameSpeed {  get; private set; }
    //how fast the game speed increases in seconds
    public float gameSpeedIncrease = 0.1f;

    //references just so we can deactivate them
    private DinoRunnerAgent dino;    //this is a reference to the agent script that is attached to the dinoAi
    private Spawner spawner;

    //get reference to UI components 
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hightScoreText;
    public Button retryButton;

    //the score which will increase by time
    private float score;

    //stuff for collection data
    //string to store filename
    string filename = "";
    //create list to store average scores
    public List<float> results;

    
 
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
        //assign references 
        dino = FindObjectOfType<DinoRunnerAgent>();
        spawner = FindObjectOfType<Spawner>();

        //call new game as soon as we run 
        //no longer need this as dino calls this in on episode begin 
        // NewGame();
        //store file in correct location
        filename = Application.dataPath + "/test.csv";

    }

    public void NewGame()
    {
        //just doing this becuase it was in game over before we removed that 
        enabled= false;
        
        //destory any obstacles left over 
        foreach(var obstacle in obstacles)
        {
            //if we did not have .gameobject it would just destory the script not the game object
            Destroy(obstacle.gameObject);
        }
        

        //rest the game speed
        gameSpeed = initialGameSpeed;
        //set score back to 0
        UpdateHighScore();
        score = 0f;
        //enable the game manager
        enabled = true;
       //get palyer and spawner active for new game
        dino.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);

        //set game over ui to false, it should never appear 
        gameOverText.gameObject.SetActive(false); 
        retryButton.gameObject.SetActive(false);
        

        
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

        //update average scores list:
        results.Add(score);

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

    private void OnApplicationQuit()
    {
        //create a new text writer to write to the test file, overwrite the previous one
        TextWriter textWriter = new StreamWriter(filename, false);

        //loop for all socres recorded
        for (int i = 0; i < results.Count;++i)
        {
            textWriter.WriteLine(i / 100 + "," + results[i]);
        }


        textWriter.Close();
    }

}
