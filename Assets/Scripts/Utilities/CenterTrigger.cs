using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterTrigger : MonoBehaviour
{
    private RingSpawner parentRing;
    private Rigidbody2D playerRb;

    private void Start()
    {
        parentRing = GetComponentInParent<RingSpawner>();

        if (parentRing == null)
        {
            Debug.LogError("CenterTrigger: Parent RingSpawner not found!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // Get player's rigidbody to check velocity direction
        playerRb = collision.GetComponent<Rigidbody2D>();

        if (playerRb == null)
        {
            Debug.LogWarning("CenterTrigger: Player has no Rigidbody2D!");
            return;
        }

        // Pass to parent ring for handling
        if (parentRing != null)
        {
            parentRing.OnCenterTriggerEnter(playerRb);
        }
    }
}