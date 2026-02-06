using UnityEngine;

public class SpotlightRotator : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("How far the spotlight rotates in degrees (e.g., 15 means it swings from -15 to +15)")]
    [SerializeField] private float angleLimit = 15f;
    
    [Tooltip("How fast the spotlight swings")]
    [SerializeField] private float speed = 2.0f;

    [Tooltip("Starting phase offset")]
    [SerializeField] private float offset = 0f;

    private Quaternion initialRotation;

    private void Awake()
    {
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        // Calculate rotation using Cosine for a slightly different "hang" time feel, or stick to Sin
        float angle = Mathf.Sin((Time.time * speed) + offset) * angleLimit;
        
        // Apply to Z axis specifically
        transform.localRotation = initialRotation * Quaternion.Euler(0, 0, angle);
    }
}
