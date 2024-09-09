using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;           // Timer shown during the game
    public TextMeshProUGUI finalTimeText;       // Timer shown on the Game Won panel
    public GameObject gameWonPanel;
    public GameObject gameOverPanel;
    public Button retryButton;
    public Button nextLevelButton;

    private float timer;
    private bool isGameActive = false;
    private bool isFuelDepleted = false;

    public VehicleController vehicleController;  // Reference to the vehicle controller for checking fuel
    public float fuelCheckInterval = 0.5f;       // How often to check fuel level

    void Start()
    {
        timer = 0;
        gameWonPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        retryButton.onClick.AddListener(RestartGame);
        nextLevelButton.onClick.AddListener(NextLevel);

        StartCoroutine(CheckFuel());
    }

    void Update()
    {
        if (isGameActive)
        {
            timer += Time.deltaTime;
            DisplayTime(timer, timerText);  // Display time on UI
        }
    }

    // Call this to display the timer
    private void DisplayTime(float timeToDisplay, TextMeshProUGUI displayText)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        displayText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartMaze()
    {
        if (!isGameActive)  // Prevent restarting timer multiple times
        {
            Debug.Log("Maze Started");
            timer = 0;
            isGameActive = true;
        }
    }

    public void EndMaze()
    {
        if (isGameActive)
        {
            isGameActive = false;
            gameWonPanel.SetActive(true);
            DisplayTime(timer, finalTimeText);  // Display time taken on game won screen
        }
    }

    public void GameOver()
    {
        if (isGameActive)
        {
            isGameActive = false;
            gameOverPanel.SetActive(true);
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void NextLevel()
    {
        // Load the next level when implemented, currently reloading the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Coroutine to check the fuel level periodically
    private IEnumerator CheckFuel()
    {
        while (isGameActive && !isFuelDepleted)
        {
            if (vehicleController.fuel <= 0)
            {
                isFuelDepleted = true;
                GameOver();
            }
            yield return new WaitForSeconds(fuelCheckInterval);
        }
    }


}
