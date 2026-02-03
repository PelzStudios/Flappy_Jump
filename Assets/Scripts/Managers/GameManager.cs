using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField]
    private TextMeshProUGUI scoreDisplay;

    private int score;
    private bool isGameOver;

    public Action OnGameOver;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            // Destroy duplicate instance
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        score = 0;
        isGameOver = false;
        UpdateScoreDisplay();
    }

    public void ChangeScore(int amount)
    {
        score += amount;
        UpdateScoreDisplay();
        Debug.Log("Score changed by: " + amount + " | Total: " + score);
    }

    public void SetGameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("Game Over! Final Score: " + score);
            
            if (OnGameOver != null)
            {
                OnGameOver.Invoke();
            }
        }
    }

    public bool GetGameOver()
    {
        return isGameOver;
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreDisplay()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.text = score.ToString();
        }
    }
}