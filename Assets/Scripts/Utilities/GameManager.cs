using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private VehicleController vehicleController;  // Reference to the vehicle controller for checking fuel                 
    [SerializeField] private float fuelCheckInterval = 0.5f;      

    private bool isGameActive = false;
    private bool isFuelDepleted = false;

    void Start()
    {

        ServiceLocator.instance.UIService.Initialize(RestartGame, NextLevel);
        StartCoroutine(CheckFuel());
    }

    void Update()
    {
        if (isGameActive)
        {
            ServiceLocator.instance.UIService.UpdateTimer(Time.deltaTime);
        }
    }

    public void StartMaze()
    {
        if (!isGameActive)
        {
            Debug.Log("Maze Started");
            isGameActive = true;
            ServiceLocator.instance.UIService.StartTimer();
        }
    }

    public void EndMaze()
    {
        if (isGameActive)
        {
            isGameActive = false;
            ServiceLocator.instance.UIService.ShowGameWon();
            ServiceLocator.instance.SoundManager.PlaySound(SoundType.GameWon);
        }
    }

    public void GameOver()
    {
        if (isGameActive)
        {
            isGameActive = false;
            ServiceLocator.instance.UIService.ShowGameOver();
            ServiceLocator.instance.SoundManager.PlaySound(SoundType.GameOver);
        }
    }

    private void RestartGame()
    {
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void NextLevel()
    {
        // Load next level or reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Check fuel level periodically
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
