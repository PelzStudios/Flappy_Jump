using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderHandleAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image handleImage;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 2.0f;
    [SerializeField] private float pulseScaleAmount = 1.2f;

    [Header("Colors")]
    [SerializeField] private Color easyColor = new Color(0.4f, 0.8f, 0.4f);
    [SerializeField] private Color mediumColor = new Color(1f, 0.6f, 0.2f);
    [SerializeField] private Color hardColor = new Color(1f, 0.3f, 0.3f);

    private Vector3 originalScale;

    private void Start()
    {
        if (handleImage == null)
        {
            handleImage = GetComponent<Image>();
        }
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // Pulse Animation
        float scale = 1.0f + Mathf.PingPong(Time.time * pulseSpeed, pulseScaleAmount - 1.0f);
        transform.localScale = originalScale * scale;
    }

    public void UpdateColor(float value)
    {
        if (handleImage != null)
        {
             Color targetColor;
             if (value <= 1.0f)
             {
                 targetColor = Color.Lerp(easyColor, mediumColor, value);
             }
             else
             {
                 targetColor = Color.Lerp(mediumColor, hardColor, value - 1.0f);
             }
             handleImage.color = targetColor;
        }
    }
}
