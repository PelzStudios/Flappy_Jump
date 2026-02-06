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
        Shield,
        GravityFlip
    }

    public RingType ringType = RingType.Normal;

    private void SetupRingType()
    {
        // Reset rotation first
        transform.rotation = Quaternion.identity;
        
        // Visual Reset (White)
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach(var s in srs) s.color = Color.white;

        // Randomize Type
        float rand = Random.value;
        
        bool isGravityInverted = false;
        if (player != null)
        {
             PlayerController pc = player.GetComponent<PlayerController>();
             if (pc != null) isGravityInverted = pc.IsGravityInverted();
        }

        if (isGravityInverted)
        {
            // HIGH chance to spawn a Flip Ring to let player escape
            // 40% Chance to Flip Back
            if (rand < 0.4f) ringType = RingType.GravityFlip;
            else if (rand < 0.7f) ringType = RingType.Normal;
            else if (rand < 0.8f) ringType = RingType.ColorChange;
            else if (rand < 0.9f) ringType = RingType.Slanted;
            else ringType = RingType.Shield;
        }
        else
        {
            // Normal Logic (Rare Flip)
            // 10% Chance to Flip
            if (rand < 0.65f) ringType = RingType.Normal;
            else if (rand < 0.75f) ringType = RingType.ColorChange;
            else if (rand < 0.85f) ringType = RingType.Slanted;
            else if (rand < 0.90f) ringType = RingType.Shield;
            else ringType = RingType.GravityFlip; 
        }

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
            case RingType.GravityFlip:
                foreach(var s in srs) s.color = new Color(0.6f, 0.2f, 1f); // Purple
                break;
        }
    }

    // Called when player enters center trigger
    public void OnCenterTriggerEnter(Rigidbody2D playerRb)
    {
        if (hasBeenScored) return;

        hasBeenScored = true;

        // Check velocity
        bool isMovingUp = playerRb.linearVelocity.y > 0.5f;
        bool isMovingDown = playerRb.linearVelocity.y < -0.5f;

        // If Inverted: Moving DOWN is bad (because you "fall" UP).
        // Wait... If gravity is flipped, Gravity is UP.
        // So default state is falling UP. You tap to go DOWN.
        // So you approach the ring from the BOTTOM always? No.
        // In Normal: You bounce up, then fall DOWN through the ring.
        // In Inverted: You bounce down, then fall UP through the ring.
        // So in Inverted: You must be moving UP (Positive Y) to score.
        // So if Inverted: Moving DOWN (Negative Y) is the "bounce" phase, which is OK?
        // No, you shouldn't enter the trigger during the bounce phase.
        
        PlayerController pc = playerRb.GetComponent<PlayerController>();
        bool isInverted = (pc != null && pc.IsGravityInverted());

        // Condition for ending Gravity Flip:
        // Passing through ANY GravityFlip ring toggles it. 
        // So if you are currently Inverted, and you hit a GravityFlip ring (Purple), it will toggle back to Normal.
        // This is handled in the Logic Block below by pc.ToggleGravity() which flips the boolean.
        
        // Strict Direction Check:
        // Normal (Gravity Down) -> Must approach from TOP (falling down, velocity < 0)
        // Inverted (Gravity Up) -> Must approach from BOTTOM (falling up?, velocity > 0)
        
        // Wait, standard Flappy Bird mechanics:
        // Normal: You Tap (Velocity > 0). You Fall (Velocity < 0). Score happens when falling through ring.
        // Inverted: You Tap (Velocity < 0). You Fall Up (Velocity > 0). Score happens when falling UP through ring.
        
        bool isFallingDown = playerRb.linearVelocity.y < 0;
        bool isFallingUp = playerRb.linearVelocity.y > 0;
        
        bool wrongDirection;
        if (!isInverted)
        {
            // Normal: Must be falling down (Velocity < 0)
            wrongDirection = !isFallingDown;
        }
        else
        {
            // Inverted: Must be falling UP (Velocity > 0)
            wrongDirection = !isFallingUp;
        }

        if (wrongDirection)
        {
            Debug.Log("Ring [" + ringIndex + "]: Wrong Direction Entry (Inverted: " + isInverted + ") - Game Over!");
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
        else if (ringType == RingType.GravityFlip)
        {
            if (UIManager.instance != null) UIManager.instance.ShowComboPopup("GRAVITY FLIP!", 0);
             PlayerController playerController = playerRb.GetComponent<PlayerController>();
             if (playerController != null) playerController.ToggleGravity();
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
                    PlayerController playerController = playerRb.GetComponent<PlayerController>();
                    if (playerController != null) playerController.ChangeColor();
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