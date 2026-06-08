using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
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
    // Movement var
    private float walkSpeed = 13f;
    private float jumpForce = 14f;

    // Coyote time var
    private float coyoteTime = 0.1f;
    private float coyoteTimeCounter = 0f;

    // Jump buffer var
    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter = 0f;

    //Rigidbodies
    private Rigidbody2D rb2D;
    private Rigidbody rb;

    // Ground check var
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private float groundCheckAngle = 0f;
    private Vector2 groundCheckSize = new Vector2(0.12f, 0.05f);

    //Audio var
    [SerializeField] private AudioClip jumpAudio;
    private AudioSource walkAUSource;

    // Player input
    [SerializeField] private InputActionAsset playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;
    private bool jumpPressed = false;

    // Referencing var
    [SerializeField] private VacuumGunController vacuumGunController;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem walkDust;
    [SerializeField] private ParticleSystem jumpDust;

    // Boolean var
    [SerializeField] private bool ControlsDisabled = false;
    private bool is3D = false;
    private bool isDusting = false;
    private bool keepMomentum = false;

    // State var
    private PlayerStates currentState = PlayerStates.Idle;
    
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
        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            rb = gameObject.GetComponent<Rigidbody>();
            jumpForce = 10f;
            is3D = true;
        }
        else if (gameObject.GetComponent<Rigidbody2D>() != null)
        {
            rb2D = gameObject.GetComponent<Rigidbody2D>();
            is3D = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        // If player airboost while on the ground, start the coroutine to Idle state. Needs per-frame check.
        // if (currentState == PlayerStates.ForcePushedUp && IsGrounded())
        // {
        //     StartCoroutine(AllowIdle());
        // }

        jumpAction.started += context => // When jump is pressed, let player jump. Jump won't be counted as pressed after pressed.
        {
            if (context.interaction is TapInteraction)
            {
                jumpPressed = true;
            }
        };

        jumpAction.performed += context => // Even if the jump is tapped, the jump won't be counted as pressed.
        {
            if (context.interaction is TapInteraction)
            {
                jumpPressed = false;
            }
        };


        // CoyoteTime
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump buffer
        if (jumpPressed || InputSystem.actions.FindAction("Push").IsPressed())
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        StateTransistion();

        // print("Current State: " + currentState);
    }

    private void FixedUpdate()
    {
        StateFunction();

        // Disable stopping momentum mid air
        if (InputSystem.actions.FindAction("Push").IsPressed())
        {
            keepMomentum = true;
        }

        // Stop dust particle
        if (walkDust.isPlaying && isDusting && currentState != PlayerStates.Walking)
        {
            walkDust.Stop();
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
    private void StateFunction()
    {
        switch (currentState)
        {
            case PlayerStates.Idle:
                animator.SetBool("isWalking", false);
                animator.SetBool("isFalling", false);
                animator.SetBool("isJumping", false);
                animator.SetBool("isPushUp", false);
                break;
            case PlayerStates.Walking:
                if (IsGrounded())
                {
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isFalling", false);
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isPushUp", false);
                    if (!isDusting)
                    {
                        walkDust.Play();
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
        XAxisMove();

        // Stationery
        Idle();

        // Jumping
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && jumpPressed)
        {
            Jump();
        }

        // Falling
        Fall();
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

    private bool TouchingGround() // to make sure that the player won't be set as grounded when jumping or pushed up
    {
        return (IsGrounded() && currentState != PlayerStates.Jumping && currentState != PlayerStates.ForcePushedUp) ? true : false;
    }

    private IEnumerator AllowIdle() // to transition to idle state after being force pushed whilst grounded
    {
        yield return new WaitForSeconds(0.2f);
        currentState = PlayerStates.Idle;
    }

    private void XAxisMove()
    {
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
    }

    private void Idle()
    {
        if(moveInput.x == 0f && TouchingGround())
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
        else if (moveInput.x == 0f && !TouchingGround() && !keepMomentum)
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
    }
    private void Jump()
    {
        currentState = PlayerStates.Jumping;
        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
        jumpPressed = false;
        if (is3D)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
        else
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
        }
        AudioManager.Instance.PlaySfx(jumpAudio, 0.3f);
        jumpDust.Play();
    }

    private void Fall()
    {
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
        jumpBufferCounter = 0f; // reset jump buffer to prevent jump buffering during push up
        if (is3D)
        {
            rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
        }
        else
        {
            rb2D.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1" || scene.name == "Level2" || scene.name == "Level3" || scene.name == "Level4")
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
        if (is3D)
        {
            var VacuumGun3DModel = vacuumGunController.transform.Find("VacuumGun3DModel");
            if (VacuumGun3DModel == null)
            {
                Debug.LogError("VacuumGun3DModel not found as a child of VacuumGunController.");
                return;
            }


            if (mousePos.x > transform.position.x + 0.1f)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                VacuumGun3DModel.transform.localRotation = Quaternion.Euler(0f, VacuumGun3DModel.transform.localRotation.y, VacuumGun3DModel.transform.localRotation.z); // flip vacuum gun sprite to match player direction
                // vacuumGunController.transform.position = new Vector3(transform.position.x + -0.34f, vacuumGunController.transform.position.y, -0.145f); // adjust vacuum gun position to be in front of player when facing right
            }
            else if (mousePos.x < transform.position.x - 0.1f)
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                VacuumGun3DModel.transform.localRotation = Quaternion.Euler(180f, VacuumGun3DModel.transform.localRotation.y, VacuumGun3DModel.transform.localRotation.z); // flip vacuum gun sprite to match player direction
                // vacuumGunController.transform.position = new Vector3(transform.position.x + 0.34f, vacuumGunController.transform.position.y, -0.145f); // adjust vacuum gun position to be in front of player when facing left
            }
        }
        else
        {
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