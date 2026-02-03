using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSpawner : MonoBehaviour
{
    [SerializeField]
    private CollisionDetector rimLeft;
    [SerializeField]
    private CollisionDetector rimRight;
    [SerializeField]
    private CollisionDetector centerTrigger;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float missedRange = 4f;
    [SerializeField]
    private float spawnDistance = 5f;
    [SerializeField]
    private ParticleSystem perfectCombo1;
    [SerializeField]
    private ParticleSystem perfectCombo2;
    [SerializeField]
    private ParticleSystem perfectCombo3;

    [HideInInspector]
    public int ringIndex;
    [HideInInspector]
    public int consecutiveCombo = 1;

    private bool hasSpawned = false;
    private RingSpawner nextRing;

    void Update()
    {
        if (player == null) return;

        // Game over if player passed ring without going through center
        if (player.position.x > transform.position.x + missedRange)
        {
            GameManager.instance.SetGameOver();
        }

        // Spawn next ring when player approaches
        if (!hasSpawned && player.position.x > transform.position.x + spawnDistance)
        {
            SpawnNextRing();
        }
    }

    private void SpawnNextRing()
    {
        GameObject newRing = Instantiate(gameObject);
        ScreenPositioner positioner = newRing.GetComponent<ScreenPositioner>();
        
        if (positioner != null)
        {
            positioner.enabled = false;
        }

        newRing.transform.position = new Vector3(transform.position.x + 5f, Random.Range(-2f, 2f), 0f);
        nextRing = newRing.GetComponent<RingSpawner>();
        
        if (nextRing != null)
        {
            nextRing.ringIndex = ringIndex + 1;
            newRing.name = "Ring_" + nextRing.ringIndex;
        }
        
        hasSpawned = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if we successfully went through center
        if (centerTrigger != null && centerTrigger.GetHasCollided() && nextRing != null)
        {
            bool hitLeftRim = (rimLeft != null && rimLeft.GetHasCollided());
            bool hitRightRim = (rimRight != null && rimRight.GetHasCollided());

            if (!hitLeftRim && !hitRightRim)
            {
                // Perfect pass - no rim collision
                nextRing.consecutiveCombo = consecutiveCombo + 1;

                if (nextRing.consecutiveCombo == 2)
                {
                    GameManager.instance.ChangeScore(4);
                    PlayParticleEffect(perfectCombo1);
                }
                else if (nextRing.consecutiveCombo == 3)
                {
                    GameManager.instance.ChangeScore(6);
                    PlayParticleEffect(perfectCombo2);
                }
                else if (nextRing.consecutiveCombo > 3)
                {
                    GameManager.instance.ChangeScore(8);
                    PlayParticleEffect(perfectCombo3);
                }
            }
            else
            {
                // Hit rim - reset combo to 1
                nextRing.consecutiveCombo = 1;
                GameManager.instance.ChangeScore(2);
                StopAllParticles();
            }
        }
        else
        {
            // Missed the ring entirely
            GameManager.instance.SetGameOver();
        }

        Destroy(gameObject);
    }

    private void PlayParticleEffect(ParticleSystem particle)
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