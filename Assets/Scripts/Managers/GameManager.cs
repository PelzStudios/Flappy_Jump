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
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Game starts in "Menu" state
        isGameActive = false;
        score = 0;
        isGameOver = false;
        
        // Play background music on menu screen
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackgroundMusic();
        }
        
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateScore(score);
        }
    }

    public void StartGame()
    {
        isGameActive = true;
        isGameOver = false;
        hasShield = false;
        score = 0;
        
        // STOP background music when game starts
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }
        
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

    private bool hasShield = false;
    private float immunityTimer = 0f;

    private void Update()
    {
        if (immunityTimer > 0)
        {
            immunityTimer -= Time.deltaTime;
        }
    }

    public void ActivateImmunity(float duration)
    {
        immunityTimer = duration;
        Debug.Log("Immunity Activated for " + duration + "s");
    }

    public void SetGameOver()
    {
        if (immunityTimer > 0)
        {
            Debug.Log("Game Over blocked by Immunity.");
            return;
        }

        if (hasShield)
        {
            hasShield = false;
            DeactivateShieldVFX();
            Debug.Log("Shield Confirmed! Saved from Game Over!");
            if (UIManager.instance != null) UIManager.instance.ShowComboPopup("SHIELD SAVED!", 0);
            if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.3f, 0.3f);
            return;
        }

        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("Game Over! Final Score: " + score);
            
            // Play game over sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGameOverSound();
            }
            
            // Play background music on game over screen
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBackgroundMusic();
            }
            
            if (ScoreManager.Instance != null && DifficultyManager.Instance != null)
            {
                ScoreManager.Instance.SubmitScore(score, DifficultyManager.Instance.currentLevel);
            }

            if (OnGameOver != null)
            {
                OnGameOver.Invoke();
            }
        }
    }

    public void ActivateShield()
    {
        hasShield = true;
        Debug.Log("Shield Activated!");
        
        // Activate player shield VFX
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            ShieldVFX shieldVFX = player.GetComponentInChildren<ShieldVFX>();
            if (shieldVFX != null)
            {
                shieldVFX.ActivateShield();
            }
        }
    }

    private void DeactivateShieldVFX()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            ShieldVFX shieldVFX = player.GetComponentInChildren<ShieldVFX>();
            if (shieldVFX != null)
            {
                shieldVFX.DeactivateShield();
            }
        }
    }

    public bool HasShield()
    {
        return hasShield;
    }

    public bool GetGameOver()
    {
        return isGameOver;
    }

    public int GetScore()
    {
        return score;
    }

    public void RestartGame()
    {
        // Play button click sound on restart
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}