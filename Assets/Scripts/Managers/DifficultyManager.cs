using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DifficultyLevel
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

[System.Serializable]
public class DifficultyProfile
{
    public float gravityScale;
    public float verticalJumpForce;
    public float horizontalJumpForce;
    public float maxSpeed;
    
    public float ringSpawnDistance;
    public float ringHeightVariance; // +/- range
}

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    public DifficultyLevel currentLevel = DifficultyLevel.Medium;

    [Header("Profiles")]
    public DifficultyProfile easyProfile = new DifficultyProfile() { 
        gravityScale = 2.5f, 
        verticalJumpForce = 8.0f, 
        horizontalJumpForce = 2.0f, 
        maxSpeed = 2.8f,
        ringSpawnDistance = 4.5f,
        ringHeightVariance = 1.5f
    };

    public DifficultyProfile mediumProfile = new DifficultyProfile() { 
        gravityScale = 3.0f, 
        verticalJumpForce = 8.5f, 
        horizontalJumpForce = 2.0f, 
        maxSpeed = 3.0f,
        ringSpawnDistance = 5.0f,
        ringHeightVariance = 2.0f
    };

    public DifficultyProfile hardProfile = new DifficultyProfile() { 
        gravityScale = 4.0f, 
        verticalJumpForce = 10.0f, 
        horizontalJumpForce = 2.5f, 
        maxSpeed = 3.5f,
        ringSpawnDistance = 6.0f,
        ringHeightVariance = 3.0f
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across reloads if needed, but safe to persist
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDifficulty(int levelIndex)
    {
        currentLevel = (DifficultyLevel)Mathf.Clamp(levelIndex, 0, 2);
    }

    public DifficultyProfile GetCurrentProfile()
    {
        switch (currentLevel)
        {
            case DifficultyLevel.Easy: return easyProfile;
            case DifficultyLevel.Hard: return hardProfile;
            default: return mediumProfile;
        }
    }

    public string GetDifficultyName()
    {
        return currentLevel.ToString();
    }
}
