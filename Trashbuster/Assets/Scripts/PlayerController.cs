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
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private VacuumGunController vacuumGunController;

    private bool isGrounded;
    private bool isIdle;
    private Rigidbody2D rb;
    private InputAction moveAction;
    private InputAction jumpAction;
    private PlayerStates currentState = PlayerStates.Idle;

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
        rb = GetComponent<Rigidbody2D>();
        // Input actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        if (moveInput.x != 0f && isIdle)
        {
            currentState = PlayerStates.Walking;
            rb.linearVelocityX = moveInput.x * walkSpeed;
        }
        else if(moveInput.x == 0f && isIdle)
        {
            currentState = PlayerStates.Idle;
            rb.linearVelocityX = 0f;
            
        }

        if (jumpAction.IsPressed() && isGrounded)
        {
            StateFunction(PlayerStates.Jumping);
        }

        if (rb.linearVelocityY < 0f && !isGrounded)
        {
            currentState = PlayerStates.Falling;
        }


        // Air horizontal control
        if (currentState == PlayerStates.Jumping || currentState == PlayerStates.Falling || currentState == PlayerStates.ForcePushed)
        {
            if (moveInput.x != 0f)
            {
                rb.AddForce(new Vector2(moveInput.x * 3f, 0f), ForceMode2D.Force);
                print("Air Control: " + moveInput.x);
            }
        }

        // print("Current State: " + currentState);
    }

    private void FixedUpdate()
    {
        isIdle = (currentState != PlayerStates.ForcePushed && currentState != PlayerStates.Jumping && currentState != PlayerStates.Falling) ? true : false;
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && currentState == PlayerStates.Falling)
        {
            currentState = PlayerStates.Idle;
        }
        
    }
    private void StateManager()
    {
        
    }
    private void StateFunction(PlayerStates currentState)
    {
        switch (currentState)
        {
            case PlayerStates.Idle:
                rb.linearVelocityX = 0f;
                break;
            case PlayerStates.Walking:
                Vector2 moveInput = moveAction.ReadValue<Vector2>();
                rb.linearVelocityX = moveInput.x * walkSpeed;
                break;
            case PlayerStates.Jumping:
                rb.linearVelocityY = jumpForce;
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
