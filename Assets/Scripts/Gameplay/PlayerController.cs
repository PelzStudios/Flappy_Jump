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
    }

    void Update()
    {
        if (Input.GetButtonDown(m_Input) && !GameManager.instance.GetGameOver())
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
}