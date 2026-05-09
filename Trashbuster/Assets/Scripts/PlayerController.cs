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
    ForcePushedUp
}

public class PlayerController : MonoBehaviour
{
    private float walkSpeed = 13f;
    private float jumpForce = 14f;
    [Header("Referencing")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private VacuumGunController vacuumGunController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private InputActionAsset playerInput;
    [SerializeField] private AudioClip footStepsAudio;
    [SerializeField] private AudioClip jumpAudio;
    [SerializeField] private ParticleSystem stepDust;
    [SerializeField] private bool ControlsDisabled = false;
    private AudioSource walkAUSource;
    private float groundCheckAngle = 0f;
    private Vector2 groundCheckSize = new Vector2(0.12f, 0.05f);
    private bool keepMomentum = false;
    private InputAction moveAction;
    private InputAction jumpAction;
    private PlayerStates currentState = PlayerStates.Idle;
    private Vector2 moveInput;
    private bool isDusting = false;
    
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
        walkAUSource = gameObject.GetComponent<AudioSource>();

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
        // // Changing from falling state
        // if (currentState == PlayerStates.Falling && IsGrounded())
        // {
        //     AudioManager.Instance.PlaySfx(footStepsAudio, 0.3f);
        // }

        moveInput = moveAction.ReadValue<Vector2>();

        if (jumpAction.IsPressed() && IsGrounded())
        {
            Jump();
        }

        // Need pre-frame checks
        if (rb.linearVelocityY < 0f && !IsGrounded())
        {
            currentState = PlayerStates.Falling;
        }

        if (currentState == PlayerStates.ForcePushedUp && IsGrounded())
        {
            StartCoroutine(AllowIdle());
        }

        // print("Current State: " + currentState);

        Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.right * groundCheckSize.x / 2f, Color.red);
        
    }

    private void FixedUpdate()
    {
        StateFunction();
        StateTransistion();

        // Disable stopping momentum mid air
        if (InputSystem.actions.FindAction("Push").IsPressed())
        {
            keepMomentum = true;
        }

        if (stepDust.isPlaying && currentState != PlayerStates.Walking && isDusting)
        {
            stepDust.Stop();
            isDusting = false;
        }

        // print(stepDust.isPlaying);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, groundCheckAngle, groundLayer);
    }

    private bool IsIdle()
    {
        return (IsGrounded() && currentState != PlayerStates.Jumping && currentState != PlayerStates.ForcePushedUp) ? true : false;
    }

    private IEnumerator AllowIdle() // to transition to idle state after being force pushed whilst grounded
    {
        yield return new WaitForSeconds(0.2f);
        currentState = PlayerStates.Idle;
    }

    private void Jump()
    {
        currentState = PlayerStates.Jumping;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        AudioManager.Instance.PlaySfx(jumpAudio, 0.3f);
    }

    private void StateFunction()
    {
        switch (currentState)
        {
            case PlayerStates.Idle:
                animator.SetBool("isWalking", false);
                animator.SetBool("isFalling", false);
                break;
            case PlayerStates.Walking:
                if (IsGrounded())
                {
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isFalling", false);
                    if (!isDusting)
                    {
                        stepDust.Play();
                        isDusting = true;
                    }

                    // Playing walking sound
                    if (!walkAUSource.isPlaying)
                    {
                        walkAUSource.Play();
                    }
                    else if(walkAUSource.isPlaying)
                    {
                        walkAUSource.Stop();
                    }
                }
                break;
            case PlayerStates.Jumping:
                animator.SetBool("isWalking", false);
                animator.SetBool("isJumping", true);
                animator.SetBool("isFalling", false);
                break;
            case PlayerStates.Falling:
                animator.SetBool("isFalling", true);
                animator.SetBool("isJumping", false);
                animator.SetBool("isPushUp", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isPushSides", false);
                break;
            case PlayerStates.ForcePushedUp:
                animator.SetBool("isPushUp", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isFalling", false);
                animator.SetBool("isJumping", false);
                break;
        }
    }

    private void StateTransistion()
    {


        // Moving on X axis
        if (moveInput.x != 0f && IsGrounded())
        {
            rb.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb.linearVelocityY);
            currentState = PlayerStates.Walking;
            keepMomentum = false;
        }
        else if (moveInput.x != 0f && !IsGrounded())
        {
            rb.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb.linearVelocityY);
            keepMomentum = false;
        }

        // Moving on Y axis
        if(moveInput.x == 0f && IsIdle())
        {
            rb.linearVelocity = Vector2.zero;
            currentState = PlayerStates.Idle;
            keepMomentum = false;            
        }
        else if (moveInput.x == 0f && !IsIdle() && !keepMomentum)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocityY);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1" || scene.name == "Level2" || scene.name == "Level3")
        {
            ControlsDisabled = false; // enable player controls when scene is loaded
        }
        else if (scene.name == "MainMenu" || scene.name == "LevelSelect")
        {
            ControlsDisabled = true; // disable player controls for other scenes
        }
        SceneStateManager.CheckInLevelScene(scene);
    }

    void OnMousePositionUpdated(Vector2 mousePos)
    {
        // Flip player sprite
        // Adding a small threshold to prevent jitter when mouse is near the player's center
        if (mousePos.x > transform.position.x + 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            vacuumGunController.transform.Find("VacuumGunSprite").GetComponent<SpriteRenderer>().flipY = false; // flip vacuum gun sprite to match player direction
        }
        else if (mousePos.x < transform.position.x - 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            vacuumGunController.transform.Find("VacuumGunSprite").GetComponent<SpriteRenderer>().flipY = true; // flip vacuum gun sprite to match player direction
        }
    }

    public void ApplyPushForce(float pushForce, Vector2 pushDir)
    {
        currentState = PlayerStates.ForcePushedUp;
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