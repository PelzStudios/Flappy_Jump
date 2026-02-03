using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [SerializeField]
    private Vector2 explosionForce;

    private void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnGameOver += PlayDeathEffect;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnGameOver -= PlayDeathEffect;
        }
    }

    private void PlayDeathEffect()
    {
        // Add physics to create explosion effect
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        transform.parent = null;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(explosionForce, ForceMode2D.Impulse);
        
        Debug.Log("Death effect triggered!");
    }
}