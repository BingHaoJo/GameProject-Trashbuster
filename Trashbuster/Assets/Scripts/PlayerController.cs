using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


enum PlayerStates
{
    Idle,
    Walking,
    Jumping,
    Falling,
    ForcePushed
}

public class PlayerController : MonoBehaviour
{
    private float walkSpeed = 13f;
    private float jumpForce = 12f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private VacuumGunController vacuumGunController;
    [SerializeField] private LayerMask groundLayer;
    private float groundCheckAngle = 0f;
    private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);

    private InputAction moveAction;
    private InputAction jumpAction;
    private PlayerStates currentState = PlayerStates.Idle;

    private Vector2 moveInput;

    void OnEnable()
    {
        if (vacuumGunController != null)
        {
            vacuumGunController.mousePositionUpdated += OnMousePositionUpdated; // signal connected
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Input actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        rb.linearVelocityX = moveInput.x * walkSpeed;
        // if (moveInput.x != 0f && isIdle)
        // {
        //     currentState = PlayerStates.Walking;
        //     rb.linearVelocityX = moveInput.x * walkSpeed;
        // }
        // else if(moveInput.x == 0f && isIdle)
        // {
        //     currentState = PlayerStates.Idle;
        //     rb.linearVelocityX = 0f;
            
        // }

        if (jumpAction.IsPressed() && IsGrounded())
        {
            currentState = PlayerStates.Jumping;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (rb.linearVelocityY < 0f && !IsGrounded())
        {
            currentState = PlayerStates.Falling;
        }

        if (IsGrounded() && currentState == PlayerStates.Falling)
        {
            currentState = PlayerStates.Idle;
        }

        // Air horizontal control
        // if (currentState == PlayerStates.Jumping || currentState == PlayerStates.Falling || currentState == PlayerStates.ForcePushed)
        // {
        //     if (moveInput.x != 0f)
        //     {
        //         rb.AddForce(new Vector2(moveInput.x * 3f, 0f), ForceMode2D.Force);;
        //     }
        // }

        // print("Current State: " + currentState);

        // StateFunction();
    }

    private void FixedUpdate()
    {
        // rb.linearVelocityX = moveInput.x * walkSpeed;
        rb.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb.linearVelocityY);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, groundCheckAngle, groundLayer);
    }

    private bool IsIdle()
    {
        return (currentState != PlayerStates.ForcePushed && currentState != PlayerStates.Jumping && currentState != PlayerStates.Falling) ? true : false;
    }

    private void StateFunction()
    {
        switch (currentState)
        {
            case PlayerStates.Idle:
                rb.linearVelocityX = 0f;
                break;
            case PlayerStates.Walking:
                // Vector2 moveInput = moveAction.ReadValue<Vector2>();
                // rb.linearVelocityX = moveInput.x * walkSpeed;
                break;
            case PlayerStates.Jumping:
                // rb.linearVelocityY = jumpForce;
                // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                break;
            case PlayerStates.Falling:
                // Gravity will handle falling
                break;
            case PlayerStates.ForcePushed:
                // Force push logic handled in VacuumGunController
                break;
        }
    }

    void OnMousePositionUpdated(Vector2 mousePos)
    {
        // Adding a small threshold to prevent jitter when mouse is near the player's center
        if (mousePos.x > transform.position.x + 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            // print("Facing Right" + mousePos.x + " /" + transform.position.x);
        }
        else if (mousePos.x < transform.position.x - 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            // print("Facing Left" + mousePos.x + " /" + transform.position.x);
        }
    }

    public void ApplyPushForce(float pushForce, Vector2 pushDir)
    {
        currentState = PlayerStates.ForcePushed;
        rb.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
        // rb.linearVelocity = pushDir * pushForce;
    }

    void OnDisable()
    {
        if (vacuumGunController != null)
        {
            vacuumGunController.mousePositionUpdated -= OnMousePositionUpdated; // signal disconnected
        }
    }
}
