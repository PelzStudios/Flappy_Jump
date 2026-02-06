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

    private void Jump()
    {
        if (m_Animator != null)
        {
            m_Animator.Play("FlapWings");
        }
        
        m_RigidBody.gravityScale = m_GravityScale;
        m_RigidBody.linearVelocity = Vector2.zero;
        m_RigidBody.AddForce((Vector3.up * m_VerticalBumpAmount + Vector3.right * m_HorizontalBumpAmount), ForceMode2D.Impulse);
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