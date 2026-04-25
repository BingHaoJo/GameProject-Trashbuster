using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


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
    private float jumpForce = 13f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private VacuumGunController vacuumGunController;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool ControlsDisabled = false;
    [SerializeField] private InputActionAsset playerInput;
    private float groundCheckAngle = 0f;
    private Vector2 groundCheckSize = new Vector2(0.12f, 0.05f);

    private InputAction moveAction;
    private InputAction jumpAction;
    private PlayerStates currentState = PlayerStates.Idle;
    private Vector2 moveInput;
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
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
        if (ControlsDisabled)
        {
            playerInput.FindActionMap("Player").Disable();
        }
        else
        {
            playerInput.FindActionMap("Player").Enable();
        }

    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        // rb.linearVelocityX = moveInput.x * walkSpeed;

        if (jumpAction.IsPressed() && IsGrounded())
        {
            currentState = PlayerStates.Jumping;
            // rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StateFunction();
        }

        if (rb.linearVelocityY < 0f && !IsGrounded())
        {
            currentState = PlayerStates.Falling;
            StateFunction();
        }

        if (currentState == PlayerStates.ForcePushed && IsGrounded())
        {
            StartCoroutine(AllowIdle());
            StateFunction();
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

        Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.right * groundCheckSize.x / 2f, Color.red);

        
    }

    private void FixedUpdate()
    {
        // rb.linearVelocityX = moveInput.x * walkSpeed;
        if (moveInput.x != 0f)
        {
            currentState = PlayerStates.Walking;
            StateFunction();
        }

        if(moveInput.x == 0f && IsIdle())
        {
            currentState = PlayerStates.Idle;
            StateFunction();
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, groundCheckAngle, groundLayer);
    }

    private bool IsIdle()
    {
        return (IsGrounded() && currentState != PlayerStates.Jumping && currentState != PlayerStates.ForcePushed) ? true : false;
    }

    private IEnumerator AllowIdle() // to transition to idle state after being force pushed whilst grounded
    {
        yield return new WaitForSeconds(0.2f);
        currentState = PlayerStates.Idle;
    }

    private void StateFunction()
    {
        switch (currentState)
        {
            case PlayerStates.Idle:
                rb.linearVelocity = Vector2.zero;
                break;
            case PlayerStates.Walking:
                rb.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb.linearVelocityY);
                break;
            case PlayerStates.Jumping:
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1" || scene.name == "Level2" || scene.name == "Level3_Vertical")
        {
            ControlsDisabled = false; // enable player controls when scene is loaded
        }
        else if (scene.name == "MainMenu" || scene.name == "LevelSelect")
        {
            ControlsDisabled = true; // disable player controls for other scenes
        }

        SceneStateManager.CheckInLevelScene();
    }

    void OnMousePositionUpdated(Vector2 mousePos)
    {
        // Flip player sprite
        // Adding a small threshold to prevent jitter when mouse is near the player's center
        if (mousePos.x > transform.position.x + 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (mousePos.x < transform.position.x - 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public void ApplyPushForce(float pushForce, Vector2 pushDir)
    {
        currentState = PlayerStates.ForcePushed;
        rb.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (vacuumGunController != null)
        {
            vacuumGunController.mousePositionUpdated -= OnMousePositionUpdated; // signal disconnected
        }
    }
}
