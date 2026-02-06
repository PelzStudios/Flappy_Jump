using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float missedRange = 4f;
    [SerializeField] private float spawnDistance = 5f;

    // Rim collision detection
    [SerializeField] private Collider2D rimLeftCollider;
    [SerializeField] private Collider2D rimRightCollider;

    // Particles for combos
    [SerializeField] private ParticleSystem perfectCombo1;
    [SerializeField] private ParticleSystem perfectCombo2;
    [SerializeField] private ParticleSystem perfectCombo3;

    [HideInInspector] public int ringIndex;
    [HideInInspector] public int consecutiveCombo = 0;

    private bool hasSpawned = false;
    private bool hasBeenScored = false;
    private RingSpawner nextRing;
    private bool rimLeftTouched = false;
    private bool rimRightTouched = false;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("RingSpawner [" + ringIndex + "]: Player not assigned!");
        }

        // Make rims slippery so player doesn't stick
        PhysicsMaterial2D slipperyMat = new PhysicsMaterial2D("Slippery");
        slipperyMat.friction = 0f;
        slipperyMat.bounciness = 0.2f; // Slight bounce to help roll off

        if (rimLeftCollider != null) rimLeftCollider.sharedMaterial = slipperyMat;
        if (rimRightCollider != null) rimRightCollider.sharedMaterial = slipperyMat;
    }

    void Update()
    {
        if (player == null) return;

        // Game over if player passed ring without scoring
        if (player.position.x > transform.position.x + missedRange && !hasBeenScored)
        {
            Debug.Log("Ring [" + ringIndex + "]: Player MISSED - Game Over!");
            GameManager.instance.SetGameOver();
            Destroy(gameObject);
            return;
        }

        // Spawn next ring when player approaches
        if (!hasSpawned && player.position.x > transform.position.x + spawnDistance)
        {
            SpawnNextRing();
        }
    }

    private void SpawnNextRing()
    {
        float distance = 5f;
        float variance = 2f;

        if (DifficultyManager.Instance != null)
        {
            DifficultyProfile profile = DifficultyManager.Instance.GetCurrentProfile();
            distance = profile.ringSpawnDistance;
            variance = profile.ringHeightVariance;
        }

        GameObject newRing = Instantiate(gameObject);
        newRing.transform.position = new Vector3(transform.position.x + distance, Random.Range(-variance, variance), 0f);

        ScreenPositioner positioner = newRing.GetComponent<ScreenPositioner>();
        if (positioner != null)
        {
            positioner.enabled = false;
        }

        nextRing = newRing.GetComponent<RingSpawner>();

        if (nextRing != null)
        {
            nextRing.ringIndex = ringIndex + 1;
            nextRing.SetupRingType(); // Setup functionality for the NEW ring
            newRing.name = "Ring_" + nextRing.ringIndex;
        }

        hasSpawned = true;
    }

    public enum RingType
    {
        Normal,
        ColorChange,
        Slanted,
        Shield
    }

    public RingType ringType = RingType.Normal;

    private void SetupRingType()
    {
        // Reset rotation first
        transform.rotation = Quaternion.identity;
        
        // Visual Reset (White)
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach(var s in srs) s.color = Color.white;

        // Randomize Type (20% chance for special rings)
        float rand = Random.value;
        if (rand < 0.7f) ringType = RingType.Normal;
        else if (rand < 0.8f) ringType = RingType.ColorChange;
        else if (rand < 0.9f) ringType = RingType.Slanted;
        else ringType = RingType.Shield;

        // Apply Visuals
        switch (ringType)
        {
            case RingType.ColorChange:
                foreach(var s in srs) s.color = new Color(1f, 0.5f, 1f); // Pinkish
                break;
            case RingType.Slanted:
                float tilt = Random.Range(15f, 25f);
                if (Random.value > 0.5f) tilt = -tilt;
                transform.rotation = Quaternion.Euler(0, 0, tilt);
                break;
            case RingType.Shield:
                foreach(var s in srs) s.color = new Color(0.4f, 0.8f, 1f); // Cyan
                break;
        }
    }

    // Called when player enters center trigger
    public void OnCenterTriggerEnter(Rigidbody2D playerRb)
    {
        if (hasBeenScored) return;

        hasBeenScored = true;

        // Check velocity - if moving UP (from bottom) = game over
        if (playerRb.linearVelocity.y > 0.5f)
        {
            Debug.Log("Ring [" + ringIndex + "]: Entered from BOTTOM (moving up) - Game Over!");
            GameManager.instance.SetGameOver();
            Destroy(gameObject);
            return;
        }

        // Apply Special Ring Effects
        if (ringType == RingType.Shield)
        {
             GameManager.instance.ActivateShield();
             if (UIManager.instance != null) UIManager.instance.ShowComboPopup("SHIELD!", nextRing != null ? nextRing.consecutiveCombo : 0);
        }

        // Player entered from top (moving down) = SCORE
        Debug.Log("Ring [" + ringIndex + "]: Entered from TOP (moving down) - SCORING!");

        if (nextRing != null)
        {
            // Check if rims were touched
            bool touchedLeftRim = rimLeftTouched;
            bool touchedRightRim = rimRightTouched;
            
            // STRICTER CHECK: Ensure player is close to center x-axis (within 0.5f)
            float xOffset = Mathf.Abs(playerRb.transform.position.x - transform.position.x);
            bool isCentered = xOffset < 0.5f;

            if (!touchedLeftRim && !touchedRightRim && isCentered)
            {
                // Perfect pass
                nextRing.consecutiveCombo = consecutiveCombo + 1;
                
                // Color Change Ring Logic (Only on Perfect)
                if (ringType == RingType.ColorChange)
                {
                    PlayerController pc = playerRb.GetComponent<PlayerController>();
                    if (pc != null) pc.ChangeColor();
                }

                string comboText = "PERFECT";
                string multiplierText = ""; 

                // Show multiplier starting from x1
                if (nextRing.consecutiveCombo >= 1)
                {
                    multiplierText = " x" + nextRing.consecutiveCombo.ToString();
                }

                comboText += multiplierText;
                
                int scoreMultiplier = Mathf.Min(nextRing.consecutiveCombo, 4); 
                // To prevent Perfect x1 being same as Rim Hit, maybe boost base momentarily?
                // For now, adhere to multiplier logic: 2 * 1 = 2. 
                int baseScore = 2;
                int points = baseScore * scoreMultiplier;
                
                GameManager.instance.ChangeScore(points);

                // FX Logic
                if (nextRing.consecutiveCombo >= 1)
                {
                     if (nextRing.consecutiveCombo < 4) 
                     {
                         PlayParticle(perfectCombo1);
                         if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.1f, 0.1f);
                     }
                     else
                     {
                         PlayParticle(perfectCombo2);
                         if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.2f, 0.2f);
                     }
                }
                
                if (UIManager.instance != null)
                {
                    // Always show popup for any perfect
                    UIManager.instance.ShowComboPopup(comboText, nextRing.consecutiveCombo);
                }
                Debug.Log("Ring [" + ringIndex + "]: " + comboText + " (" + points + " pts)");
                

            }
            else
            {
                // Hit rim or not centered - reset combo to 0
                nextRing.consecutiveCombo = 0;
                GameManager.instance.ChangeScore(2);
                StopAllParticles();
                Debug.Log("Ring [" + ringIndex + "]: Clean but not perfect. Rim touched: " + (touchedLeftRim || touchedRightRim) + ", Centered: " + isCentered);
            }
        }

        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        // Disable all colliders immediately so player doesn't hit them while fading
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            
            foreach (var sr in renderers)
            {
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = alpha;
                    sr.color = c;
                }
            }
            yield return null;
        }
        
        Destroy(gameObject);
    }

    // Called when player touches left rim
    public void OnRimLeftEnter()
    {
        rimLeftTouched = true;
        Debug.Log("Ring [" + ringIndex + "]: Player touched LEFT rim");
    }

    // Called when player touches right rim
    public void OnRimRightEnter()
    {
        rimRightTouched = true;
        Debug.Log("Ring [" + ringIndex + "]: Player touched RIGHT rim");
    }

    private void PlayParticle(ParticleSystem particle)
    {
        if (particle != null)
        {
            particle.Play();
        }
        StopAllParticles(particle);
    }

    private void StopAllParticles(ParticleSystem except = null)
    {
        if (perfectCombo1 != null && perfectCombo1 != except) perfectCombo1.Stop();
        if (perfectCombo2 != null && perfectCombo2 != except) perfectCombo2.Stop();
        if (perfectCombo3 != null && perfectCombo3 != except) perfectCombo3.Stop();
    }
}