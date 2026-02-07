using UnityEngine;

public class ShieldVFX : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private SpriteRenderer shieldSprite;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 0.8f;
    [SerializeField] private Color shieldColor = new Color(0.4f, 0.8f, 1f, 0.5f);

    private bool isActive = false;

    private void Awake()
    {
        if (shieldSprite == null)
        {
            shieldSprite = GetComponent<SpriteRenderer>();
        }

        if (shieldSprite != null)
        {
            shieldSprite.color = shieldColor;
            shieldSprite.enabled = false;
        }
    }

    private void Update()
    {
        if (isActive && shieldSprite != null)
        {
            // Pulse effect
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            Color color = shieldSprite.color;
            color.a = alpha;
            shieldSprite.color = color;
        }
    }

    public void ActivateShield()
    {
        isActive = true;
        if (shieldSprite != null)
        {
            shieldSprite.enabled = true;
        }
    }

    public void DeactivateShield()
    {
        isActive = false;
        if (shieldSprite != null)
        {
            shieldSprite.enabled = false;
        }
    }
}
