using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float turnSmoothSpeed = 10f;

    [Header("Modelo y animaciones")]
    public Transform model;
    public Animator animator;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.15f;
    public LayerMask groundMask;
    public float coyoteTime = 0.12f;

    [Header("Plataformas")]
    public float plataformaStickTimeout = 0.2f;

    [Header("Mobile Controls")]
    public Joystick joystick;       // 🎮 Asigna aquí tu Fixed Joystick del Canvas
    public bool useMobileControls = true;
    private bool jumpButtonPressed;

    private CharacterController controller;
    private Vector3 velocity;
    private float lastGroundedTime = -999f;

    private Transform plataformaActual;
    private Vector3 ultimaPosicionPlataforma;
    private float lastPlatformHitTime = -999f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null && model != null)
            animator = model.GetComponent<Animator>();

        if (groundCheck == null)
        {
            GameObject go = new GameObject("GroundCheck");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0, -controller.height * 0.5f + 0.1f, 0);
            groundCheck = go.transform;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // --- Ground check
        bool groundedCheck = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (groundedCheck) lastGroundedTime = Time.time;
        if (plataformaActual != null) lastGroundedTime = Time.time;

        bool isGrounded = (Time.time - lastGroundedTime) <= coyoteTime;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // --- Inputs
        float x, z;

        if (useMobileControls && joystick != null)
        {
            x = joystick.Horizontal;
            z = joystick.Vertical;
        }
        else
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }

        Vector3 move = new Vector3(x, 0f, z).normalized;
        Vector3 moveDir = move * moveSpeed;

        // --- Rotación
        if (move.magnitude >= 0.1f && model != null)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(model.eulerAngles.y, targetAngle, turnSmoothSpeed * dt);
            model.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        // --- Salto
        bool jumpPressed = useMobileControls ? jumpButtonPressed : Input.GetButtonDown("Jump");
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            plataformaActual = null;
        }
        jumpButtonPressed = false; // reset

        // --- Gravedad
        velocity.y += gravity * dt;

        // --- Plataforma delta
        Vector3 platformDelta = Vector3.zero;
        if (plataformaActual != null)
        {
            if (Time.time - lastPlatformHitTime > plataformaStickTimeout)
                plataformaActual = null;
            else
            {
                platformDelta = plataformaActual.position - ultimaPosicionPlataforma;
                ultimaPosicionPlataforma = plataformaActual.position;
            }
        }

        // --- Movimiento
        Vector3 totalMove = (moveDir * dt) + (velocity * dt) + platformDelta;
        controller.Move(totalMove);

        // --- Animaciones
        if (animator != null)
        {
            animator.SetBool("IsJumping", !isGrounded);
            animator.SetFloat("Speed", move.magnitude);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0.5f && hit.collider.CompareTag("Plataforma"))
        {
            plataformaActual = hit.collider.transform;
            ultimaPosicionPlataforma = plataformaActual.position;
            lastPlatformHitTime = Time.time;
            lastGroundedTime = Time.time;
        }
    }

    // 🔹 Método que llamará tu botón de salto en UI
    public void OnJumpButton()
    {
        jumpButtonPressed = true;
    }
}
