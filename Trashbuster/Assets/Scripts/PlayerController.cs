using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private VacuumGunController vacuumGunController;

    private bool isGrounded;
    private Rigidbody2D rb;
    private InputAction moveAction;
    private InputAction jumpAction;

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
        rb.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb.linearVelocityY);


        if (jumpAction.IsPressed() && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnMousePositionUpdated(Vector2 mousePos)
    {
        if (mousePos.x > transform.position.x + 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            // print("Facing Right" + mousePos.x + " /" + transform.position.x);
        }
        else if (mousePos.x < transform.position.x - 0.1f) // Adding a small threshold to prevent jitter when mouse is near the player's center
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            // print("Facing Left" + mousePos.x + " /" + transform.position.x);
        }
    }

    void OnDisable()
    {
        if (vacuumGunController != null)
        {
            vacuumGunController.mousePositionUpdated -= OnMousePositionUpdated; // signal disconnected
        }
    }
}
