using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField]
    private string m_Input = "Jump";
    [SerializeField]
    private float m_VerticalBumpAmount = 8.5f;
    [SerializeField]
    private float m_HorizontalBumpAmount = 2f;
    [SerializeField]
    private float m_MaxSpeed = 3f;
    [SerializeField]
    private float m_GravityScale = 3f;
    [SerializeField]
    private Animator m_Animator;

    private Rigidbody2D m_RigidBody;

    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_RigidBody.gravityScale = 0;
        isGravityInverted = false;
        
        ApplyDifficultySettings();
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
        if (GameManager.instance.IsGameActive() && Input.GetButtonDown(m_Input) && !GameManager.instance.GetGameOver())
        {
            Jump();
        }
        
        // Limit horizontal speed
        if (m_RigidBody.linearVelocity.x > m_MaxSpeed)
        {
            Vector2 velocity = m_RigidBody.linearVelocity;
            velocity.x = m_MaxSpeed;
            m_RigidBody.linearVelocity = velocity;
        }
    }

    private bool isGravityInverted = false;

    private void Jump()
    {
        if (m_Animator != null)
        {
            m_Animator.Play("FlapWings");
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
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Random vibrant color
            sr.color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
        }
    }
}