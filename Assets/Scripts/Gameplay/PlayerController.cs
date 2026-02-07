using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_VerticalBumpAmount = 8.5f;
    [SerializeField] private float m_HorizontalBumpAmount = 2f;
    [SerializeField] private float m_MaxSpeed = 3f;
    [SerializeField] private float m_GravityScale = 3f;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private ParticleSystem colorChangeVFX;

    private Rigidbody2D m_RigidBody;
    private bool isGravityInverted = false;

    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_RigidBody.gravityScale = 0;
        isGravityInverted = false;
        
        ApplyDifficultySettings();

        // Subscribe to InputHandler for both PC and Mobile
        if (InputHandler.instance != null)
        {
            InputHandler.instance.JumpPressed += Jump;
        }
        else
        {
            Debug.LogWarning("InputHandler not found in scene!");
        }
    }

    private void ApplyDifficultySettings()
    {
        if (DifficultyManager.Instance != null)
        {
            DifficultyProfile profile = DifficultyManager.Instance.GetCurrentProfile();
            m_GravityScale = profile.gravityScale;
            m_VerticalBumpAmount = profile.verticalJumpForce;
            m_HorizontalBumpAmount = profile.horizontalJumpForce;
            m_MaxSpeed = profile.maxSpeed;
        }
    }

    void Update()
    {
        if (m_RigidBody == null) return;

        // InputHandler handles all input now (PC + Mobile)
        // Just limit horizontal speed here
        if (m_RigidBody.linearVelocity.x > m_MaxSpeed)
        {
            Vector2 velocity = m_RigidBody.linearVelocity;
            velocity.x = m_MaxSpeed;
            m_RigidBody.linearVelocity = velocity;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from InputHandler
        if (InputHandler.instance != null)
        {
            InputHandler.instance.JumpPressed -= Jump;
        }
    }

    private void Jump()
    {
        // Only jump if game is active and not game over
        if (!GameManager.instance.IsGameActive() || GameManager.instance.GetGameOver())
        {
            return;
        }

        if (m_Animator != null)
        {
            m_Animator.Play("FlapWings");
        }
        
        // Play jump sound - SIMPLIFIED AUDIO
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayJumpSound();
        }
        
        // Ensure gravity is active (started)
        float gravityDir = isGravityInverted ? -1f : 1f;
        m_RigidBody.gravityScale = m_GravityScale * gravityDir;
        
        m_RigidBody.linearVelocity = Vector2.zero;
        
        // Flip Jump Direction
        Vector3 jumpForce = Vector3.up * m_VerticalBumpAmount * gravityDir + Vector3.right * m_HorizontalBumpAmount;
        m_RigidBody.AddForce(jumpForce, ForceMode2D.Impulse);
    }

    public void ToggleGravity()
    {
        isGravityInverted = !isGravityInverted;
        
        // Flip Sprite
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y) * (isGravityInverted ? -1f : 1f);
        transform.localScale = scale;

        // Apply immediate gravity change if mid-air
        if (m_RigidBody.gravityScale != 0)
        {
            float gravityDir = isGravityInverted ? -1f : 1f;
            m_RigidBody.gravityScale = m_GravityScale * gravityDir;
        }

        // Add short immunity to prevent immediate death from floor/ceiling impact
        if (GameManager.instance != null)
        {
            GameManager.instance.ActivateImmunity(0.5f);
        }
        
        Debug.Log("Gravity Inverted: " + isGravityInverted);
    }

    public bool IsGravityInverted()
    {
        return isGravityInverted;
    }

    public void ChangeColor()
    {
        // Use the referenced sprite renderer instead of GetComponent
        if (playerSprite != null)
        {
            // Random vibrant color
            playerSprite.color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
            
            // Play color change VFX
            if (colorChangeVFX != null)
            {
                var main = colorChangeVFX.main;
                main.startColor = playerSprite.color;
                colorChangeVFX.Play();
            }
        }
    }

    public void ResetPlayer()
    {
        if (m_RigidBody == null) return;

        // Reset velocity
        m_RigidBody.linearVelocity = Vector2.zero;
        
        // Reset gravity scale to 0
        m_RigidBody.gravityScale = 0;
        
        // Reset gravity direction
        isGravityInverted = false;
        
        // Reset position
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;
        
        // Reset sprite scale
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;
        
        Debug.Log("Player reset!");
    }
}