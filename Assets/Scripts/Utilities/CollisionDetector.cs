using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    private bool hasCollided = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only detect if colliding with player
        if (collision.gameObject.CompareTag("Player"))
        {
            hasCollided = true;
            Debug.Log($"Collision detected on {gameObject.name}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only detect if colliding with player
        if (collision.gameObject.CompareTag("Player"))
        {
            hasCollided = true;
            Debug.Log($"Trigger detected on {gameObject.name}");
        }
    }

    public bool GetHasCollided()
    {
        return hasCollided;
    }

    public void ResetCollisionState()
    {
        hasCollided = false;
    }
}