using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float turnSmoothSpeed = 10f; // velocidad de giro suave

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal"); // A/D o ←/→
        float z = Input.GetAxis("Vertical");   // W/S o ↑/↓

        Vector3 move = new Vector3(x, 0f, z).normalized;

        if (move.magnitude >= 0.1f)
        {
            // 🔹 Calcular el ángulo hacia donde mirar
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;

            // 🔹 Girar suavemente al jugador
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, turnSmoothSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        // 🔹 Mover al jugador (en la dirección que está mirando)
        Vector3 moveDir = transform.forward * move.magnitude;
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        // 🔹 Saltar
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
