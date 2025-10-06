using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float rotationSpeed = 10f; // Velocidad de rotación del personaje

    [Header("Salto y Gravedad")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private int maxJumps = 2;
    private int jumpsRemainig;
    
    [Header("Dash")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashDuration = 0.2f;
    
    [Header("Ground")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;
    private bool isGrounded;

    private Animator animator;
    
    private Vector3 dashDirection;
    private bool isDashing = false;
    private float dashTimer;
    private float cooldownTimer;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;
    private bool isSprinting;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        jumpsRemainig = maxJumps;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (context.performed && moveInput != Vector2.zero)
        {
            animator.SetBool("Walk", true);
        }
        else if (context.canceled)
        {
            animator.SetBool("Walk", false);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed &&jumpsRemainig>0)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpsRemainig--;
            
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
            isSprinting = true;
        else if (context.canceled)
            isSprinting = false;
    }
    
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && cooldownTimer <= 0f)
        {
            // Dirección del dash basada en el input actual
            if (moveInput.sqrMagnitude > 0.1f)
            {
                // Si hay input de movimiento, dash en esa dirección
                Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
                dashDirection = Camera.main.transform.TransformDirection(inputDirection);
                dashDirection.y = 0f;
                dashDirection.Normalize();
            }
            else
            {
                // Si no hay input, dash hacia donde mira el personaje
                dashDirection = transform.forward;
            }

            isDashing = true;
            dashTimer = dashDuration;
            cooldownTimer = dashCooldown;
        }
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetBool("Attack",true);
        }   
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // Animaciones
        
        // Movimiento
        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0f;
        
        // Rotar el personaje hacia la dirección del movimiento
        if (move.sqrMagnitude > 0.1f && !isDashing)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Gravedad
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpsRemainig = maxJumps;
            Debug.Log("Esta en el piso");
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        // Dash
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            controller.Move(dashDirection * (dashDistance / dashDuration) * Time.deltaTime);

            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }
}