using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    private int score;
    private int highScore;
    private bool isGameOver;
    private bool isGameActive;

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
        // Game starts in "Menu" state
        isGameActive = false;
        score = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        isGameOver = false;
        
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateScore(score);
        }
    }

    public void StartGame()
    {
        isGameActive = true;
        isGameOver = false;
        score = 0;
        
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateScore(score);
        }
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public void ChangeScore(int amount)
    {
        score += amount;
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateScore(score);
        }
        Debug.Log("Score changed by: " + amount + " | Total: " + score);
    }

    public void SetGameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("Game Over! Final Score: " + score);
            
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
                Debug.Log("New High Score: " + highScore);
            }

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

    public int GetHighScore()
    {
        return highScore;
    }



    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}