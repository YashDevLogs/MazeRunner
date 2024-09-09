using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIService : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private GameObject gameWonPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Slider fuelSliderUI;

    private float timer;

    public void Initialize(System.Action onRetry, System.Action onNextLevel)
    {
        retryButton.onClick.AddListener(() => onRetry.Invoke());
        nextLevelButton.onClick.AddListener(() => onNextLevel.Invoke());
        gameWonPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void StartTimer()
    {
        timer = 0;
    }

    public void UpdateTimer(float deltaTime)
    {
        timer += deltaTime;
        DisplayTime(timerText, timer);
    }

    public void ShowFinalTime()
    {
        DisplayTime(finalTimeText, timer);
    }

    private void DisplayTime(TextMeshProUGUI displayText, float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        displayText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void ShowGameWon()
    {
        gameWonPanel.SetActive(true);
        ShowFinalTime();
    }

    public void UpdateFuelSlider(float fuelPercentage)
    {
        fuelSliderUI.value = fuelPercentage;
    }
}
