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
    [Header("HUD References")]
    [SerializeField] private TextMeshProUGUI hudScoreLabel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboPopupText;
    [SerializeField] private TextMeshProUGUI hudHighScoreLabel;
    [SerializeField] private TextMeshProUGUI hudHighScoreText;

    [Header("Home References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private TextMeshProUGUI difficultyLabel;
    [SerializeField] private DifficultyVisuals difficultyVisuals;
    [SerializeField] private SliderHandleAnimator sliderHandleAnimator;

    [Header("Game Over References")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;
    
    [Header("Difficulty Colors")]
    [SerializeField] private Color easyColor = new Color(0.4f, 0.8f, 0.4f);
    [SerializeField] private Color mediumColor = new Color(1f, 0.6f, 0.2f);
    [SerializeField] private Color hardColor = new Color(1f, 0.3f, 0.3f);
    
    [Header("Difficulty Faces")]
    [SerializeField] private Sprite easyFaceSprite;
    [SerializeField] private Sprite mediumFaceSprite;
    [SerializeField] private Sprite hardFaceSprite;
    
    [Header("Leaderboard References")]
    [SerializeField] private Image difficultyFaceImage;
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
                // Initialize visuals
                if (difficultyVisuals != null) difficultyVisuals.UpdateVisuals(difficultySlider.value);
                if (sliderHandleAnimator != null) sliderHandleAnimator.UpdateColor(difficultySlider.value);
            }
        }
    }

    private void OnDifficultyChanged(float value)
    {
        // Update Game Logic immediately for preview? No, let's keep it smooth
        // Update Visuals (Smooth float)
        if (difficultyVisuals != null)
        {
            difficultyVisuals.UpdateVisuals(value);
        }

         if (sliderHandleAnimator != null)
        {
            sliderHandleAnimator.UpdateColor(value);
        }
    }



    private void UpdateDifficultyLabel()
    {
        // Handled by DifficultyVisuals now for smoother animation
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
        // Play button click sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
        if (homePanel != null) homePanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
        
        if (GameManager.instance != null && ScoreManager.Instance != null && DifficultyManager.Instance != null)
        {
            GameManager.instance.StartGame();
            
            // Update HUD Labels and Values
            string difficultyName = DifficultyManager.Instance.GetDifficultyName().ToUpper();
            ScoreStats stats = ScoreManager.Instance.GetStats(DifficultyManager.Instance.currentLevel);

            if (hudScoreLabel != null) hudScoreLabel.text = difficultyName + " MODE:";
            if (scoreText != null) scoreText.text = "0";

            if (hudHighScoreLabel != null) hudHighScoreLabel.text = "ALL TIME BEST:";
            if (hudHighScoreText != null) hudHighScoreText.text = stats.allTimeBest.ToString();
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
            finalScoreText.text = " " + GameManager.instance.GetScore().ToString();
        }

        // Update Leaderboard
        if (ScoreManager.Instance != null && DifficultyManager.Instance != null)
        {
             ScoreStats stats = ScoreManager.Instance.GetStats(DifficultyManager.Instance.currentLevel);
             
             if (difficultyModeText != null) 
             {
                 difficultyModeText.text = DifficultyManager.Instance.GetDifficultyName().ToUpper() + " MODE";
                 
                 // Apply Color
                 switch (DifficultyManager.Instance.currentLevel)
                 {
                     case DifficultyLevel.Easy: 
                        difficultyModeText.color = easyColor;
                        if (difficultyFaceImage != null) difficultyFaceImage.sprite = easyFaceSprite;
                        break;
                     case DifficultyLevel.Medium: 
                        difficultyModeText.color = mediumColor;
                        if (difficultyFaceImage != null) difficultyFaceImage.sprite = mediumFaceSprite;
                        break;
                     case DifficultyLevel.Hard: 
                        difficultyModeText.color = hardColor;
                        if (difficultyFaceImage != null) difficultyFaceImage.sprite = hardFaceSprite;
                        break;
                 }
             }

             if (todayBestText != null) todayBestText.text = "Today's Best: " + stats.dailyBest.ToString();
             if (weekBestText != null) weekBestText.text = "Week's Best: " + stats.weeklyBest.ToString();
             if (allTimeBestText != null) allTimeBestText.text = "All-Time Best: " + stats.allTimeBest.ToString();
        }
    }

    public void ShowComboPopup(string text, int comboLevel)
    {
        // For simplicity, we can clone a template or use a dedicated single text object that animates
        if (comboPopupText != null)
        {
            comboPopupText.gameObject.SetActive(true);
            comboPopupText.text = text;
            
            // Basic Pop Animation
            StartCoroutine(AnimateComboPopup());
        }
    }

    private IEnumerator AnimateComboPopup()
    {
        if (comboPopupText == null) yield break;

        float duration = 0.8f;
        float elapsed = 0f;
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * 1.5f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Pop up scale then shrink
            float scaleCrv = Mathf.Sin(t * Mathf.PI); // 0 -> 1 -> 0 curve-ish
            
            // Actually let's do: Scale Up quickly, then fade out
            float curScale = Mathf.Lerp(1.0f, 1.5f, t);
            comboPopupText.transform.localScale = originalScale * curScale;
            
            // Fade alpha
            comboPopupText.alpha = 1.0f - (t * t); // Non-linear fade

            elapsed += Time.deltaTime;
            yield return null;
        }

        comboPopupText.gameObject.SetActive(false);
        comboPopupText.transform.localScale = originalScale;
    }

    private void OnRestartClicked()
    {
        // Play button click sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
        if (GameManager.instance != null)
        {
            GameManager.instance.RestartGame();
        }
    }

    private void OnHomeClicked()
    {
        // Play button click sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
        // Reloading the scene basically resets everything, 
        // and since Start logic defaults to Home, this works.
        if (GameManager.instance != null)
        {
            GameManager.instance.RestartGame();
        }
    }
}
