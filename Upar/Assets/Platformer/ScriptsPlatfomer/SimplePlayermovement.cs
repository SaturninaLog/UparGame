using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float turnSmoothSpeed = 10f;

    public Transform model;        // tu modelo hijo
    public Animator animator;      // el Animator del modelo hijo

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Si no lo asignaste en el inspector, intenta buscarlo en el hijo
        if (animator == null && model != null)
            animator = model.GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0f, z).normalized;

        // 🔹 Movimiento
        Vector3 moveDir = move * moveSpeed;
        controller.Move(moveDir * Time.deltaTime);

        // 🔹 Rotación del modelo (solo el hijo)
        if (move.magnitude >= 0.1f && model != null)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(model.eulerAngles.y, targetAngle, turnSmoothSpeed * Time.deltaTime);
            model.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        // 🔹 Saltar
        bool jumpPressed = Input.GetButtonDown("Jump");
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 🔹 Animaciones
        if (animator != null)
        {
            animator.SetBool("IsJumping", !isGrounded); // true si está en el aire
            animator.SetFloat("Speed", move.magnitude); // 0 quieto, >0 moviéndose
        }
    }
}
