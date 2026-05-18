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
    private Rigidbody2D rb2D;
    private Rigidbody rb;
    // [SerializeField] private VacuumGunController vacuumGunController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private InputActionAsset playerInput;
    [SerializeField] private AudioClip jumpAudio;
    [SerializeField] private ParticleSystem stepDust;
    [SerializeField] private bool ControlsDisabled = false;
    [SerializeField] private bool is3D = false;
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
        // if (vacuumGunController != null)
        // {
        //     vacuumGunController.mousePositionUpdated += OnMousePositionUpdated; // signal connected
        // }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Input actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        walkAUSource = gameObject.GetComponent<AudioSource>();

        // Disables player controls
        if (ControlsDisabled)
        {
            playerInput.FindActionMap("Player").Disable();
        }
        else
        {
            playerInput.FindActionMap("Player").Enable();
        }

        // 3D or 2D check
        if (is3D)
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }
        else
        {
            rb2D = gameObject.GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        StateTransistion();

        // If player airboost while on the ground, start the coroutine to Idle state. Needs per-frame check.
        if (currentState == PlayerStates.ForcePushedUp && IsGrounded())
        {
            StartCoroutine(AllowIdle());
        }

        // print("Current State: " + currentState);
    }

    private void FixedUpdate()
    {
        // StateFunction();

        // Disable stopping momentum mid air
        if (InputSystem.actions.FindAction("Push").IsPressed())
        {
            keepMomentum = true;
        }

        // Stop dust particle
        if (stepDust.isPlaying && currentState != PlayerStates.Walking && isDusting)
        {
            stepDust.Stop();
            isDusting = false;
        }

        // Playing walking sound
        if (!walkAUSource.isPlaying && currentState == PlayerStates.Walking)
        {
            walkAUSource.Play();
        }
        else if(walkAUSource.isPlaying && currentState != PlayerStates.Walking)
        {
            walkAUSource.Stop();
        }
    }

    private bool IsGrounded()
    {
        if (is3D)
        {
            return Physics.CheckBox(groundCheck.position, (Vector3)groundCheckSize / 2f, Quaternion.Euler(0f, 0f, groundCheckAngle), groundLayer);
        }
        else
        {
            return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, groundCheckAngle, groundLayer);
        }
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
        if (is3D)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
        else
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
        }
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
            if (is3D)
            {
                rb.linearVelocity = new Vector3(moveInput.x * walkSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
            }
            else
            {
                rb2D.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb2D.linearVelocityY);
            }
            currentState = PlayerStates.Walking;
            keepMomentum = false;
        }
        else if (moveInput.x != 0f && !IsGrounded())
        {
            if (is3D)
            {
                rb.linearVelocity = new Vector3(moveInput.x * walkSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
            }
            else
            {
                rb2D.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb2D.linearVelocityY);
            }
            keepMomentum = false;
        }

        // Stationery
        if(moveInput.x == 0f && IsIdle())
        {
            if (is3D)
            {
                rb.linearVelocity = Vector3.zero;
            }
            else
            {
                rb2D.linearVelocity = Vector2.zero;
            }
            currentState = PlayerStates.Idle;
            keepMomentum = false;            
        }
        else if (moveInput.x == 0f && !IsIdle() && !keepMomentum)
        {
            if (is3D)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, rb.linearVelocity.z);
            }
            else
            {
                rb2D.linearVelocity = new Vector2(0f, rb2D.linearVelocityY);
            }
        }

        // Jumping
        if (jumpAction.IsPressed() && IsGrounded())
        {
            Jump();
        }

        // Falling
        if (is3D)
        {
            if (rb.linearVelocity.y < 0f && !IsGrounded())
            {
                currentState = PlayerStates.Falling;
            }
        }
        else
        {
            if (rb2D.linearVelocityY < 0f && !IsGrounded())
            {
                currentState = PlayerStates.Falling;
            }
        }
    }
    public void ApplyPushForce(float pushForce, Vector2 pushDir)
    {
        currentState = PlayerStates.ForcePushedUp;
        if (is3D)
        {
            rb.AddForce((Vector3)pushDir * pushForce, ForceMode.Impulse);
        }
        else
        {
            rb2D.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
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
            // vacuumGunController.transform.Find("VacuumGunSprite").GetComponent<SpriteRenderer>().flipY = false; // flip vacuum gun sprite to match player direction
        }
        else if (mousePos.x < transform.position.x - 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            // vacuumGunController.transform.Find("VacuumGunSprite").GetComponent<SpriteRenderer>().flipY = true; // flip vacuum gun sprite to match player direction
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // if (vacuumGunController != null)
        // {
        //     vacuumGunController.mousePositionUpdated -= OnMousePositionUpdated; // signal disconnected
        // }
    }
}