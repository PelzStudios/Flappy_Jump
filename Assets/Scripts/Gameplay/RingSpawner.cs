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
    [HideInInspector] public int consecutiveCombo = 1;

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
            newRing.name = "Ring_" + nextRing.ringIndex;
        }

        hasSpawned = true;
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

        // Player entered from top (moving down) = SCORE
        Debug.Log("Ring [" + ringIndex + "]: Entered from TOP (moving down) - SCORING!");

        if (nextRing != null)
        {
            // Check if rims were touched
            bool touchedLeftRim = rimLeftTouched;
            bool touchedRightRim = rimRightTouched;

            if (!touchedLeftRim && !touchedRightRim)
            {
                // Perfect pass - no rim touch
                nextRing.consecutiveCombo = consecutiveCombo + 1;

                if (nextRing.consecutiveCombo == 2)
                {
                    GameManager.instance.ChangeScore(4);
                    PlayParticle(perfectCombo1);
                    if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.1f, 0.1f);
                    Debug.Log("Ring [" + ringIndex + "]: Perfect x2! +4 points");
                }
                else if (nextRing.consecutiveCombo == 3)
                {
                    GameManager.instance.ChangeScore(6);
                    PlayParticle(perfectCombo2);
                    if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.15f, 0.15f);
                    Debug.Log("Ring [" + ringIndex + "]: Perfect x3! +6 points");
                }
                else if (nextRing.consecutiveCombo > 3)
                {
                    GameManager.instance.ChangeScore(8);
                    PlayParticle(perfectCombo3);
                    if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.2f, 0.2f);
                    Debug.Log("Ring [" + ringIndex + "]: Perfect x4+! +8 points");
                }
            }
            else
            {
                // Hit rim - reset combo
                nextRing.consecutiveCombo = 1;
                GameManager.instance.ChangeScore(2);
                StopAllParticles();
                Debug.Log("Ring [" + ringIndex + "]: Hit rim! +2 points");
            }
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