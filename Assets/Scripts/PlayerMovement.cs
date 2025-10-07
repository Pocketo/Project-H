using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float airControl = 0.5f; // Control del movimiento en el aire (0-1)

    [Header("Salto y Gravedad")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float coyoteTimeDuration = 0.2f;
    [SerializeField] private float fallThreshold = -0.5f; // Velocidad mínima para activar animación de caída
    private int jumpsRemaining;
    private float coyoteTimeCounter;
    private bool isFalling = false;
    
    [Header("Dash")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashDuration = 0.2f;
    
    [Header("Ground")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;
    private bool isGrounded;
    private bool wasGrounded;

    [Header("Combate")]
    [SerializeField] private float attackCooldown = 0.5f; // Tiempo entre ataques
    [SerializeField] private float comboResetTime = 1.5f; // Tiempo para resetear el combo
    private int currentAttack = 0; // 0 = sin atacar, 1-3 = ataques del combo
    private float lastAttackTime = 0f;
    private float comboTimer = 0f;
    private bool isAttacking = false;

    private Animator animator;
    
    private Vector3 dashDirection;
    private bool isDashing = false;
    private float dashTimer;
    private float cooldownTimer;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;

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

        // Solo activar animación de Walk si está en el suelo Y no está en dash
        if (context.performed && moveInput != Vector2.zero && isGrounded && !isDashing)
        {
            animator.SetBool("Walk", true);
        }
        else if (context.canceled || !isGrounded || isDashing)
        {
            animator.SetBool("Walk", false);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && (jumpsRemaining > 0 || coyoteTimeCounter > 0f))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
            if (coyoteTimeCounter > 0f)
            {
                coyoteTimeCounter = 0f;
                jumpsRemaining = maxJumps - 1;
            }
            else
            {
                jumpsRemaining--;
            }
            
            animator.SetTrigger("Jump");
            isFalling = false;
        }
    }
    
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && cooldownTimer <= 0f && !isAttacking)
        {
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
            
            animator.SetTrigger("Dash");
        }
    }
    
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && !isAttacking)
        {
            // Verificar si el cooldown ha pasado
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // Resetear combo si pasó mucho tiempo
                if (Time.time >= comboTimer + comboResetTime)
                {
                    currentAttack = 0;
                }

                // Incrementar el ataque del combo
                currentAttack++;
                if (currentAttack > 3)
                {
                    currentAttack = 1; // Volver al primer ataque
                }

                // Activar la animación correspondiente
                switch (currentAttack)
                {
                    case 1:
                        animator.SetTrigger("Attack1");
                        break;
                    case 2:
                        animator.SetTrigger("Attack2");
                        break;
                    case 3:
                        animator.SetTrigger("Attack3");
                        break;
                }

                lastAttackTime = Time.time;
                comboTimer = Time.time;
                isAttacking = true;

                // Llamar a la corrutina para resetear isAttacking después de un tiempo
                Invoke(nameof(ResetAttack), attackCooldown);
            }
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
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
            
            // Resetear animación de caída al tocar el suelo
            if (isFalling)
            {
                animator.SetBool("IsFalling", false);
                isFalling = false;
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            
            // Activar animación de caída si está cayendo rápido (y no está en dash)
            if (velocity.y < fallThreshold && !isFalling && !isGrounded && !isDashing)
            {
                animator.SetBool("IsFalling", true);
                isFalling = true;
            }
        }

        // Movimiento normal (bloqueado durante ataque O dash)
        if (!isDashing && !isAttacking)
        {
            // Velocidad diferente según si está en el suelo o en el aire
            float currentSpeed = isGrounded ? speed : speed * airControl;
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
            move = Camera.main.transform.TransformDirection(move);
            move.y = 0f;
            
            // Rotar el personaje hacia la dirección del movimiento (más lento en el aire)
            if (move.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                float currentRotationSpeed = isGrounded ? rotationSpeed : rotationSpeed * airControl;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentRotationSpeed * Time.deltaTime);
            }
            
            controller.Move(move * currentSpeed * Time.deltaTime);
        }

        // Gravedad
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        // Dash (tiene prioridad sobre el movimiento normal)
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            // Durante el dash, solo se mueve en la dirección del dash
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
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", velocity.y);
        animator.SetInteger("AttackCount", currentAttack);
        
        // Asegurar que Walk solo esté activo en el suelo y sin dash/ataque
        if (!isGrounded || isDashing || isAttacking)
        {
            animator.SetBool("Walk", false);
        }
        else if (isGrounded && moveInput.sqrMagnitude > 0.1f && !isAttacking && !isDashing)
        {
            animator.SetBool("Walk", true);
        }
        else if (moveInput.sqrMagnitude <= 0.1f)
        {
            animator.SetBool("Walk", false);
        }
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