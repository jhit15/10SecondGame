using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;  

public class LevelController : MonoBehaviour
{
    public float timeLimit = 10f;  
    public float timeRemaining;
    public Transform goal;  
    public GameObject player; 

    public TMP_Text timerText; 
    public TMP_Text resultText; 
    public TMP_Text readyText;  
    public TMP_Text endGameText;  

    private bool hasPlayerWon = false;
    private bool isGameStarted = false;

    public AudioManager2 audioManager;
    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = timeLimit;
        timerText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
        StartCoroutine(GameStartDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (hasPlayerWon) return;  
        if (!isGameStarted) return;  
        
        timeRemaining -= Time.deltaTime; 
        timerText.text = "Time Left: " + Mathf.Max(timeRemaining, 0).ToString("F2");

        // If time runs out and the player has not won
        if (timeRemaining <= 0f)
        {
            LoseGame();
        }
    }

   
    private IEnumerator GameStartDelay()
    {
       
        Time.timeScale = 0f;
        readyText.text = "Find The Hidden Entrance to the Cave!";  
        yield return new WaitForSecondsRealtime(2f);

        Time.timeScale = 1f;  
        readyText.text = "";  
        timerText.gameObject.SetActive(true);  
        isGameStarted = true;
        audioManager.PlayBackgroundMusic();

    }

    // Player win mechanic
    private void WinGame()
    {
        if (hasPlayerWon) return; 
        hasPlayerWon = true;
        timeRemaining = 0f;
        resultText.text = "You Win!";
        resultText.gameObject.SetActive(true);
        Debug.Log("You Win! triggered");
        StopGame();
        //audioManager.StopCurrentAudio();
        ShowEndGameScreen("You Win!");
        audioManager.PlayWinSound();
    }

    // Lose Mechanic
    private void LoseGame()
    {
        if (hasPlayerWon) return;
        hasPlayerWon = true;
        resultText.text = "You Lose!"; 
        resultText.gameObject.SetActive(true); 
        ShowEndGameScreen("You Lose!");
        StopGame();
        audioManager.PlayLoseSound();
    }

    
    private void ShowEndGameScreen(string message)
    {
         endGameText.text = message;
    }

    
    private void StopGame()
    {
        Time.timeScale = 0f;  
        player.GetComponent<PlayerMovement>().enabled = false; 
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Debug.Log("OnTriggerEnter called with: " + other.gameObject.name);

        
        if (other.gameObject == goal.gameObject) 
        {
            
            Debug.Log("Player reached the goal!");
            WinGame();
        }
    }

}
