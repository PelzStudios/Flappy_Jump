using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;
    
    public delegate void OnJumpInputDelegate();
    public event OnJumpInputDelegate JumpPressed;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // PC Input: Spacebar or Left Mouse Click
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            TriggerJumpInput();
            Debug.Log("Jump input detected (PC)");
        }

        // Mobile Input: Screen touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                TriggerJumpInput();
                Debug.Log("Jump input detected (Mobile)");
            }
        }
    }

    private void TriggerJumpInput()
    {
        if (JumpPressed != null)
        {
            JumpPressed.Invoke();
        }
    }
}