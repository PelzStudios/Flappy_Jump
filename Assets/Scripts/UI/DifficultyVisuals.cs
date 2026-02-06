using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyVisuals : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private Image faceBackground;
    [SerializeField] private Color easyColor = new Color(0.4f, 0.8f, 0.4f); // Greenish
    [SerializeField] private Color mediumColor = new Color(1f, 0.6f, 0.2f); // Orangish
    [SerializeField] private Color hardColor = new Color(1f, 0.3f, 0.3f);   // Reddish

    [Header("Features (Canvas Groups)")]
    [SerializeField] private CanvasGroup easyFeatures;
    [SerializeField] private CanvasGroup mediumFeatures;
    [SerializeField] private CanvasGroup hardFeatures;

    [Header("Label")]
    [SerializeField] private TMPro.TextMeshProUGUI difficultyLabel;
    private readonly string[] difficultyNames = { "EASY", "MEDIUM", "HARD" };

    [Header("Accessories")]
    [SerializeField] private Transform hornsTransform;
    [SerializeField] private Vector3 hornsHiddenScale = Vector3.zero;
    [SerializeField] private Vector3 hornsShownScale = Vector3.one;

    public void UpdateVisuals(float value)
    {
        // Value ranges from 0 (Easy) to 2 (Hard)
        
        Color currentColor = Color.white;

        // 1. Color Interpolation
        if (value <= 1.0f)
        {
            currentColor = Color.Lerp(easyColor, mediumColor, value);
        }
        else
        {
            currentColor = Color.Lerp(mediumColor, hardColor, value - 1.0f);
        }

        if (faceBackground != null)
        {
            faceBackground.color = currentColor;
        }

        // Update Label
        if (difficultyLabel != null)
        {
            difficultyLabel.color = currentColor;
            int roundedIndex = Mathf.RoundToInt(value);
            roundedIndex = Mathf.Clamp(roundedIndex, 0, 2);
            difficultyLabel.text = difficultyNames[roundedIndex];
        }

        // 2. Feature Morph (Scaling instead of Fading)
        // Easy: 1 at 0, 0 at 1
        float easyScale = Mathf.Clamp01(1.0f - value);
        
        // Medium: 0 at 0, 1 at 1, 0 at 2
        float distFromMed = Mathf.Abs(value - 1.0f);
        float medScale = Mathf.Clamp01(1.0f - distFromMed);

        // Hard: 0 at 1, 1 at 2
        float hardScale = Mathf.Clamp01(value - 1.0f);

        // Apply Scaling for "Pop/Morph" feel
        SetScale(easyFeatures, easyScale);
        SetScale(mediumFeatures, medScale);
        SetScale(hardFeatures, hardScale);

        // Also control alpha for cleanliness (so 0 scale is also invisible)
        SetAlpha(easyFeatures, easyScale > 0.1f ? 1 : 0);
        SetAlpha(mediumFeatures, medScale > 0.1f ? 1 : 0);
        SetAlpha(hardFeatures, hardScale > 0.1f ? 1 : 0);

        if (hornsTransform != null)
        {
            // Horns only start appearing AFTER Medium (1.0).
            // So range 1.0 -> 2.0 maps to 0.0 -> 1.0 scale
            float hornsT = 0f;
            if (value > 1.0f)
            {
                hornsT = (value - 1.0f); // 0 at 1.0, 1 at 2.0
            }
            
            hornsTransform.localScale = Vector3.Lerp(hornsHiddenScale, hornsShownScale, hornsT);
        }
    }

    private void SetScale(CanvasGroup group, float scale)
    {
        if (group != null)
        {
            group.transform.localScale = Vector3.one * scale;
        }
    }

    private void SetAlpha(CanvasGroup group, float alpha)
    {
        if (group != null)
        {
            group.alpha = alpha;
        }
    }
}
