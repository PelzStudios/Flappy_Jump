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

    [Header("Home References")]
    [SerializeField] private Button playButton;

    [Header("Game Over References")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;

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
            scoreText.text = score.ToString();
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
        
        if (GameManager.instance != null)
        {
            GameManager.instance.StartGame();
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
            finalScoreText.text = GameManager.instance.GetScore().ToString();
        }

        if (highScoreText != null && GameManager.instance != null)
        {
            highScoreText.text = "BEST: " + GameManager.instance.GetHighScore().ToString();
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
