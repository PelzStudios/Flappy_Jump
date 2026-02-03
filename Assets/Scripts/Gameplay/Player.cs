using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float maxFallSpeed = 15f;
    [SerializeField] private float tiltAngle = 45f;
    
    private Rigidbody2D rb;
    private bool isDead = false;
    
    private void OnEnable()
    {
        // Get Rigidbody2D when script enables
        rb = GetComponent<Rigidbody2D>();
        
        // If still null, this will catch it
        if (rb == null)
        {
            Debug.LogError("Player: Rigidbody2D not found! Add Rigidbody2D to this GameObject!");
            enabled = false;
            return;
        }
    }
    
    private void Update()
    {
        if (isDead) return;
        
        // Jump on Space or Click
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Jump();
        }
    }
    
    private void FixedUpdate()
    {
        if (rb == null) return;
        
        // Clamp fall speed
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(0, -maxFallSpeed);
        }
        
        // Rotate based on velocity
        float rotation = Mathf.Clamp(rb.linearVelocity.y * -2f, -tiltAngle, tiltAngle);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
    
    private void Jump()
    {
        if (rb == null) return;
        
        rb.linearVelocity = new Vector2(0, jumpForce);
        Debug.Log("Player jumped!");
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if hit ring edge
        // if (collision.CompareTag("Ring"))
        // {
        //     Die();
        // }
        
        // Passed through ring safely
        if (collision.CompareTag("RingPass"))
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddScore(1);
            }
        }

        if (collision.CompareTag("Ground"))
        {
            Die();
        }
    }
    
    private void Die()
    {
        isDead = true;
        Debug.Log("HIT A RING! GAME OVER!");
        
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }
    
    public void ResetPlayer()
    {
        isDead = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(0, 0, 0);
    }
}