using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float forwardSpeed = 10f;
    public float maxSpeed = 30f;
    public float speedIncreaseRate = 0.5f;
    public float laneDistance = 3f;
    private int currentLane = 1;

    private CharacterController controller;
    private Vector3 moveDirection;
    public float jumpForce = 8f;
    public float gravity = 20f;
    public float fastFallMultiplier = 2.5f; // 👈 Velocidad extra al caer
    private float verticalVelocity;

    private bool isDead = false;

    [Header("Slide / Agacharse")]
    public float slideDuration = 1f;             // Cuánto dura agachado
    public float slideHeight = 0.5f;             // Altura mientras se agacha
    private float originalHeight;
    private Vector3 originalCenter;
    private bool isSliding = false;
    private float slideTimer = 0f;

    [Header("UI Game Over")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public Button restartButton;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Guardar tamaño original
        originalHeight = controller.height;
        originalCenter = controller.center;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartScene);
        }
    }

    void Update()
    {
        if (isDead) return;

        if (forwardSpeed < maxSpeed)
            forwardSpeed += speedIncreaseRate * Time.deltaTime;

        moveDirection = Vector3.forward * forwardSpeed;

        // Cambio de carril
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentLane = Mathf.Max(0, currentLane - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            currentLane = Mathf.Min(2, currentLane + 1);

        float targetX = (currentLane - 1) * laneDistance;
        moveDirection.x = (targetX - transform.position.x) * 10f;

        // Saltar
        if (controller.isGrounded)
        {
            verticalVelocity = -1;

            if (Input.GetKeyDown(KeyCode.Space))
                verticalVelocity = jumpForce;

            // Slide: presiona flecha abajo estando en el suelo
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartSlide();
            }
        }
        else
        {
            // Caer más rápido en el aire
            if (Input.GetKey(KeyCode.DownArrow))
                verticalVelocity -= gravity * fastFallMultiplier * Time.deltaTime;
            else
                verticalVelocity -= gravity * Time.deltaTime;
        }

        // Si está deslizando, contar tiempo
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
            {
                EndSlide();
            }
        }

        moveDirection.y = verticalVelocity;

        controller.Move(moveDirection * Time.deltaTime);
    }

    private void StartSlide()
    {
        if (isSliding) return;

        isSliding = true;
        slideTimer = slideDuration;

        controller.height = slideHeight;
        controller.center = new Vector3(controller.center.x, slideHeight / 2f, controller.center.z);
    }

    private void EndSlide()
    {
        isSliding = false;
        controller.height = originalHeight;
        controller.center = originalCenter;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverText != null)
                gameOverText.text = "☠️ Game Over";
        }

        Time.timeScale = 0f;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Restart()
    {
        forwardSpeed = 10f;
        currentLane = 1;
        transform.position = Vector3.zero;
        verticalVelocity = 0f;
        isDead = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        GroundManager groundManager = FindObjectOfType<GroundManager>();
        if (groundManager != null)
            groundManager.ResetGround();

        Time.timeScale = 1f;
    }
}
