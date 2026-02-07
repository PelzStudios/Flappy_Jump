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
    
    // Special ring VFX
    [SerializeField] private ParticleSystem shieldRingVFX;
    [SerializeField] private ParticleSystem colorChangeRingVFX;

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
        slipperyMat.bounciness = 0.2f;

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
            nextRing.SetupRingType();
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
        transform.rotation = Quaternion.identity;
        
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach(var s in srs) s.color = Color.white;

        float rand = Random.value;
        
        bool isGravityInverted = false;
        if (player != null)
        {
             PlayerController pc = player.GetComponent<PlayerController>();
             if (pc != null) isGravityInverted = pc.IsGravityInverted();
        }

        if (isGravityInverted)
        {
            if (rand < 0.4f) ringType = RingType.GravityFlip;
            else if (rand < 0.7f) ringType = RingType.Normal;
            else if (rand < 0.8f) ringType = RingType.ColorChange;
            else if (rand < 0.9f) ringType = RingType.Slanted;
            else ringType = RingType.Shield;
        }
        else
        {
            if (rand < 0.65f) ringType = RingType.Normal;
            else if (rand < 0.75f) ringType = RingType.ColorChange;
            else if (rand < 0.85f) ringType = RingType.Slanted;
            else if (rand < 0.90f) ringType = RingType.Shield;
            else ringType = RingType.GravityFlip; 
        }

        switch (ringType)
        {
            case RingType.ColorChange:
                foreach(var s in srs) s.color = new Color(1f, 0.5f, 1f);
                
                if (colorChangeRingVFX != null)
                {
                    var main = colorChangeRingVFX.main;
                    main.startColor = new Color(1f, 0.5f, 1f, 0.8f);
                    colorChangeRingVFX.Play();
                }
                break;
            case RingType.Slanted:
                float tilt = Random.Range(15f, 25f);
                if (Random.value > 0.5f) tilt = -tilt;
                transform.rotation = Quaternion.Euler(0, 0, tilt);
                break;
            case RingType.Shield:
                foreach(var s in srs) s.color = new Color(0.4f, 0.8f, 1f);
                
                if (shieldRingVFX != null)
                {
                    var main = shieldRingVFX.main;
                    main.startColor = new Color(0.4f, 0.8f, 1f, 0.8f);
                    shieldRingVFX.Play();
                }
                break;
            case RingType.GravityFlip:
                foreach(var s in srs) s.color = new Color(0.6f, 0.2f, 1f);
                break;
        }
    }

    public void OnCenterTriggerEnter(Rigidbody2D playerRb)
    {
        if (hasBeenScored) return;

        hasBeenScored = true;

        PlayerController pc = playerRb.GetComponent<PlayerController>();
        bool isInverted = (pc != null && pc.IsGravityInverted());

        bool isFallingDown = playerRb.linearVelocity.y < 0;
        bool isFallingUp = playerRb.linearVelocity.y > 0;
        
        bool wrongDirection;
        if (!isInverted)
        {
            wrongDirection = !isFallingDown;
        }
        else
        {
            wrongDirection = !isFallingUp;
        }

        if (wrongDirection)
        {
            Debug.Log("Ring [" + ringIndex + "]: Wrong Direction Entry - Game Over!");
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

        Debug.Log("Ring [" + ringIndex + "]: Scoring!");

        if (nextRing != null)
        {
            bool touchedLeftRim = rimLeftTouched;
            bool touchedRightRim = rimRightTouched;
            
            float xOffset = Mathf.Abs(playerRb.transform.position.x - transform.position.x);
            bool isCentered = xOffset < 0.5f;

            if (!touchedLeftRim && !touchedRightRim && isCentered)
            {
                // Perfect pass
                nextRing.consecutiveCombo = consecutiveCombo + 1;
                
                if (ringType == RingType.ColorChange)
                {
                    PlayerController playerController = playerRb.GetComponent<PlayerController>();
                    if (playerController != null) playerController.ChangeColor();
                }

                string comboText = "PERFECT x" + nextRing.consecutiveCombo.ToString();
                
                int scoreMultiplier = Mathf.Min(nextRing.consecutiveCombo, 4); 
                int baseScore = 2;
                int points = baseScore * scoreMultiplier;
                
                GameManager.instance.ChangeScore(points);

                if (nextRing.consecutiveCombo >= 1)
                {
                     if (nextRing.consecutiveCombo < 4) 
                     {
                         PlayParticle(perfectCombo1);
                         if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.1f, 0.1f);
                         
                         // SIMPLE AUDIO - Ring pass sound
                         if (AudioManager.Instance != null)
                         {
                             AudioManager.Instance.PlayRingPassSound();
                         }
                     }
                     else
                     {
                         PlayParticle(perfectCombo2);
                         if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.2f, 0.2f);
                         
                         // SIMPLE AUDIO - Ring pass sound
                         if (AudioManager.Instance != null)
                         {
                             AudioManager.Instance.PlayRingPassSound();
                         }
                     }
                }
                
                if (UIManager.instance != null)
                {
                    UIManager.instance.ShowComboPopup(comboText, nextRing.consecutiveCombo);
                }
                Debug.Log("Ring [" + ringIndex + "]: " + comboText + " (" + points + " pts)");
            }
            else
            {
                // Hit rim or not centered
                nextRing.consecutiveCombo = 0;
                GameManager.instance.ChangeScore(2);
                StopAllParticles();
                
                // SIMPLE AUDIO - Ring pass sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayRingPassSound();
                }
                
                Debug.Log("Ring [" + ringIndex + "]: Regular pass (2 pts)");
            }
        }

        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
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

    public void OnRimLeftEnter()
    {
        rimLeftTouched = true;
        Debug.Log("Ring [" + ringIndex + "]: Player touched LEFT rim");
    }

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