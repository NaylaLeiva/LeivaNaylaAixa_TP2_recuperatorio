using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float mouseSensitivity = 8f;
    private float speedMultiplier = 1f; // Modifica MudZone

    [Header("Detección de Suelo (Raycast)")]
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Interacción (Raycast)")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform cameraHolder;

    [Header("Animaciones")]
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private HealthSystem healthSystem;
    private float horizontalInput;
    private float verticalInput;
    private bool isGrounded;
    private bool isRunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        healthSystem = GetComponent<HealthSystem>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Bloquea el control si el juego no está en Playing o si el jugador murió
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        if (healthSystem != null && healthSystem.IsDead)
            return;

        // 1. Lectura de ejes de movimiento
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isRunning = Input.GetKey(KeyCode.LeftShift) && verticalInput > 0f;

        // 2. Rotación horizontal del cuerpo del jugador (Corregido con *)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        // 3. Detección de suelo con Raycast
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);

        // 4. Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // 5. Interacción con objetos (tecla E) mediante Raycast
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        // 6. Actualización de parámetros del Animator
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        if (healthSystem != null && healthSystem.IsDead)
            return;

        MovePlayer();
    }

    private void MovePlayer()
    {
        // Corregido con el operador de multiplicación (*)
        float currentSpeed = (isRunning ? runSpeed : walkSpeed) * speedMultiplier;
        Vector3 moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        Vector3 targetVelocity = moveDirection * currentSpeed;

        // Preserva la velocidad vertical (gravedad/salto) del Rigidbody
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (animator != null)
            animator.SetTrigger("Jump");
    }

    private void TryInteract()
    {
        Vector3 origin = cameraHolder != null ? cameraHolder.position : transform.position + Vector3.up;
        Vector3 direction = cameraHolder != null ? cameraHolder.forward : transform.forward;

        Debug.DrawRay(origin, direction * interactDistance, Color.green, 1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, interactDistance, interactableLayer))
        {
            Debug.Log($"[PlayerController] Interactuando con: {hit.collider.name}");
            hit.collider.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        float moveAmount = new Vector2(horizontalInput, verticalInput).magnitude;
        animator.SetFloat("Speed", isRunning ? moveAmount * 2f : moveAmount);
        animator.SetBool("IsGrounded", isGrounded);
    }

    // Llamado por MudZone para reducir o restaurar velocidad
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
}