using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private RingManager ringManager;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    
    private int score = 0;
    private bool isGameOver = false;
    private int scoreThreshold = 3; // Difficulty increases every 3 rings
    
    private void Start()
    {
        // Validate all references
        if (player == null)
        {
            Debug.LogError("GameManager: Player not assigned!");
        }
        if (ringManager == null)
        {
            Debug.LogError("GameManager: RingManager not assigned!");
        }
        if (scoreText == null)
        {
            Debug.LogError("GameManager: Score Text not assigned!");
        }
        if (gameOverText == null)
        {
            Debug.LogError("GameManager: Game Over Text not assigned!");
        }
        
        StartGame();
    }
    
    private void Update()
    {
        if (isGameOver && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            Restart();
        }
    }
    
    public void StartGame()
    {
        score = 0;
        isGameOver = false;
        
        if (ringManager != null) ringManager.ResetRings();
        if (player != null) player.ResetPlayer();
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);
        
        UpdateScoreUI();
        Debug.Log("Game started!");
    }
    
    public void AddScore(int points)
    {
        if (isGameOver) return;
        
        score += points;
        UpdateScoreUI();
        
        Debug.Log($"Score: {score}");
        
        // Increase difficulty every X rings
        if (score % scoreThreshold == 0)
        {
            if (ringManager != null)
            {
                ringManager.IncreaseDifficulty();
            }
        }
    }
    
    public void GameOver()
    {
        isGameOver = true;
        Debug.Log($"GAME OVER! Final Score: {score}");
        
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = $"Game Over!\nScore: {score}\n\nPress SPACE to Restart";
        }
    }
    
    private void Restart()
    {
        StartGame();
    }
    
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}