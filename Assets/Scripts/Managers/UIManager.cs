using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Panels")]
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("HUD References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hudHighScoreText;

    [Header("Home References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private TextMeshProUGUI difficultyLabel;

    [Header("Game Over References")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;
    
    [Header("Leaderboard References")]
    [SerializeField] private TextMeshProUGUI difficultyModeText;
    [SerializeField] private TextMeshProUGUI todayBestText;
    [SerializeField] private TextMeshProUGUI weekBestText;
    [SerializeField] private TextMeshProUGUI allTimeBestText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ShowHome();

        // Listen for Game Over
        if (GameManager.instance != null)
        {
            GameManager.instance.OnGameOver += ShowGameOver;
        }

        // Setup buttons
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (homeButton != null)
            homeButton.onClick.AddListener(OnHomeClicked);

        // Setup difficulty slider
        if (difficultySlider != null)
        {
            difficultySlider.onValueChanged.AddListener(OnDifficultyChanged);
            // Initialize label
            if (DifficultyManager.Instance != null)
            {
                difficultySlider.value = (int)DifficultyManager.Instance.currentLevel;
                UpdateDifficultyLabel();
            }
        }
    }

    private void OnDifficultyChanged(float value)
    {
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetDifficulty((int)value);
            UpdateDifficultyLabel();
        }
    }

    private void UpdateDifficultyLabel()
    {
        if (difficultyLabel != null && DifficultyManager.Instance != null)
        {
            difficultyLabel.text = "Difficulty: " + DifficultyManager.Instance.GetDifficultyName();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnGameOver -= ShowGameOver;
        }

        if (playButton != null) playButton.onClick.RemoveListener(OnPlayClicked);
        if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartClicked);
        if (homeButton != null) homeButton.onClick.RemoveListener(OnHomeClicked);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private void ShowHome()
    {
        if (homePanel != null) homePanel.SetActive(true);
        if (hudPanel != null) hudPanel.SetActive(false); // Hide HUD during home
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void OnPlayClicked()
    {
        if (homePanel != null) homePanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
        
        if (GameManager.instance != null && ScoreManager.Instance != null && DifficultyManager.Instance != null)
        {
            GameManager.instance.StartGame();
            if (hudHighScoreText != null)
            {
                ScoreStats stats = ScoreManager.Instance.GetStats(DifficultyManager.Instance.currentLevel);
                hudHighScoreText.text = "All Time: " + stats.allTimeBest.ToString();
            }
        }
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        if (hudPanel != null)
        {
             hudPanel.SetActive(false);
        }

        if (finalScoreText != null && GameManager.instance != null)
        {
            finalScoreText.text = "Score: " + GameManager.instance.GetScore().ToString();
        }

        // Update Leaderboard
        if (ScoreManager.Instance != null && DifficultyManager.Instance != null)
        {
             ScoreStats stats = ScoreManager.Instance.GetStats(DifficultyManager.Instance.currentLevel);
             
             if (difficultyModeText != null) difficultyModeText.text = DifficultyManager.Instance.GetDifficultyName().ToUpper() + " MODE";
             if (todayBestText != null) todayBestText.text = "Today's Best: " + stats.dailyBest.ToString();
             if (weekBestText != null) weekBestText.text = "Week's Best: " + stats.weeklyBest.ToString();
             if (allTimeBestText != null) allTimeBestText.text = "All-Time Best: " + stats.allTimeBest.ToString();
        }
    }

    private void OnRestartClicked()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RestartGame();
        }
    }

    private void OnHomeClicked()
    {
        // Reloading the scene basically resets everything, 
        // and since Start logic defaults to Home, this works.
        if (GameManager.instance != null)
        {
            GameManager.instance.RestartGame();
        }
    }
}
