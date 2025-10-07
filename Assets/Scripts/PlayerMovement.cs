using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Salto y Gravedad")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float coyoteTimeDuration = 0.2f; // Duración del coyote time
    private int jumpsRemaining;
    private float coyoteTimeCounter; // Contador actual del coyote time
    
    [Header("Dash")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashDuration = 0.2f;
    
    [Header("Ground")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;
    private bool isGrounded;
    private bool wasGrounded; // Para detectar cuando acaba de dejar el suelo

    private Animator animator;
    
    private Vector3 dashDirection;
    private bool isDashing = false;
    private float dashTimer;
    private float cooldownTimer;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;
    //private bool isSprinting;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        jumpsRemaining = maxJumps;
        coyoteTimeCounter = 0f;
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
        // Puede saltar si tiene saltos restantes O si está en coyote time
        if (context.performed && (jumpsRemaining > 0 || coyoteTimeCounter > 0f))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
            // Si usamos coyote time (primer salto desde el aire), resetear contador
            if (coyoteTimeCounter > 0f)
            {
                coyoteTimeCounter = 0f;
                jumpsRemaining = maxJumps - 1; // Gastamos el primer salto
            }
            else
            {
                jumpsRemaining--;
            }
            
            animator.SetTrigger("Jump"); // Opcional: añadir trigger de salto
        }
    }

   // public void Sprint(InputAction.CallbackContext context)
    //{
        //if (context.performed && isSprinting)
            //isSprinting = true;
       
        //else if (context.canceled)
            //isSprinting = false;
   //}
    
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && cooldownTimer <= 0f)
        {
            // Dirección del dash basada en el input actual
            if (moveInput.sqrMagnitude > 0.1f)
            {
                Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
                dashDirection = Camera.main.transform.TransformDirection(inputDirection);
                dashDirection.y = 0f;
                dashDirection.Normalize();
            }
            else
            {
                dashDirection = transform.forward;
            }

            isDashing = true;
            dashTimer = dashDuration;
            cooldownTimer = dashCooldown;
            
            animator.SetTrigger("Dash"); // Opcional: añadir trigger de dash
        }
    }
    
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing)
        {
            // Usar SetTrigger en lugar de SetBool para animaciones de una sola vez
            animator.SetTrigger("Attack");
        }   
    }

    void Update()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        // Lógica de Coyote Time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTimeDuration;
            jumpsRemaining = maxJumps;
        }
        else
        {
            // Decrementar coyote time solo cuando NO está en el suelo
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Movimiento
        if (!isDashing)
        {
           float currentSpeed = speed;
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
            move = Camera.main.transform.TransformDirection(move);
            move.y = 0f;
            
            // Rotar el personaje hacia la dirección del movimiento
            if (move.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            
           controller.Move(move * currentSpeed * Time.deltaTime);
        }

        // Gravedad
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Pequeña fuerza hacia abajo para mantener grounded
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
        
        // Actualizar parámetros del animator
        UpdateAnimator();
    }
    
    private void UpdateAnimator()
    {
        // Parámetros opcionales que puedes añadir a tu Animator
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", velocity.y);
      //  animator.SetBool("IsSprinting", isSprinting && moveInput.sqrMagnitude > 0.1f);
    }
    
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}