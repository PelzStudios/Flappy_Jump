using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public struct ScoreStats
{
    public int dailyBest;
    public int weeklyBest;
    public int allTimeBest;
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SubmitScore(int score, DifficultyLevel difficulty)
    {
        string diffKey = difficulty.ToString();
        DateTime now = DateTime.Now;

        // --- All Time ---
        string keyAllTime = $"Score_{diffKey}_AllTime";
        int currentAllTime = PlayerPrefs.GetInt(keyAllTime, 0);
        if (score > currentAllTime)
        {
            PlayerPrefs.SetInt(keyAllTime, score);
            Debug.Log($"[ScoreManager] New All-Time Best ({diffKey}): {score}");
        }

        // --- Daily ---
        string keyDailyScore = $"Score_{diffKey}_Daily_Score";
        string keyDailyDate = $"Score_{diffKey}_Daily_Date";
        string todayStr = now.ToString("yyyy-MM-dd");

        string savedDailyDate = PlayerPrefs.GetString(keyDailyDate, "");
        int dailyBest = PlayerPrefs.GetInt(keyDailyScore, 0);

        if (savedDailyDate != todayStr)
        {
            // New day, reset
            dailyBest = 0;
            PlayerPrefs.SetString(keyDailyDate, todayStr);
        }

        if (score > dailyBest)
        {
            PlayerPrefs.SetInt(keyDailyScore, score);
            Debug.Log($"[ScoreManager] New Daily Best ({diffKey}): {score}");
        }

        // --- Weekly ---
        string keyWeeklyScore = $"Score_{diffKey}_Weekly_Score";
        string keyWeeklyID = $"Score_{diffKey}_Weekly_ID";
        string thisWeekID = GetWeekID(now);

        string savedWeeklyID = PlayerPrefs.GetString(keyWeeklyID, "");
        int weeklyBest = PlayerPrefs.GetInt(keyWeeklyScore, 0);

        if (savedWeeklyID != thisWeekID)
        {
            // New week, reset
            weeklyBest = 0;
            PlayerPrefs.SetString(keyWeeklyID, thisWeekID);
        }

        if (score > weeklyBest)
        {
            PlayerPrefs.SetInt(keyWeeklyScore, score);
            Debug.Log($"[ScoreManager] New Weekly Best ({diffKey}): {score}");
        }

        PlayerPrefs.Save();
        Debug.Log($"[ScoreManager] Score Submitted: {score} for {diffKey}. Saved successfully.");
    }

    public ScoreStats GetStats(DifficultyLevel difficulty)
    {
        string diffKey = difficulty.ToString();
        DateTime now = DateTime.Now;
        ScoreStats stats = new ScoreStats();

        // All Time
        stats.allTimeBest = PlayerPrefs.GetInt($"Score_{diffKey}_AllTime", 0);

        // Daily
        string keyDailyDate = $"Score_{diffKey}_Daily_Date";
        string todayStr = now.ToString("yyyy-MM-dd");
        if (PlayerPrefs.GetString(keyDailyDate, "") == todayStr)
        {
            stats.dailyBest = PlayerPrefs.GetInt($"Score_{diffKey}_Daily_Score", 0);
        }
        else
        {
            stats.dailyBest = 0;
        }

        // Weekly
        string keyWeeklyID = $"Score_{diffKey}_Weekly_ID";
        string thisWeekID = GetWeekID(now);
        if (PlayerPrefs.GetString(keyWeeklyID, "") == thisWeekID)
        {
            stats.weeklyBest = PlayerPrefs.GetInt($"Score_{diffKey}_Weekly_Score", 0);
        }
        else
        {
            stats.weeklyBest = 0;
        }

        return stats;
    }

    private string GetWeekID(DateTime date)
    {
        // Use ISO 8601 week number
        return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString() + "_" + date.Year;
    }
}
