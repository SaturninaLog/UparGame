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
    public float fastFallMultiplier = 2.5f;
    public float verticalVelocity;

    private bool isDead = false;

    [Header("Slide / Agacharse")]
    public float slideDuration = 1f;
    public float slideHeight = 0.5f;
    private float originalHeight;
    private Vector3 originalCenter;
    private bool isSliding = false;
    private float slideTimer = 0f;
    [SerializeField] private float slideYOffset = 0f;

    [Header("UI Game Over")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public Button restartButton;

    [Header("Animaciones")]
    public Animator animator;

    [Header("Mobile Controls")]
    public bool useMobileControls = true;
    private Vector2 startTouch;
    private bool isSwiping = false;
    public float minSwipeDistance = 100f; // px

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        originalCenter = controller.center;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartScene);
        }

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isDead || (GameManager.instance != null && GameManager.instance.playerFrozen))
        {
            if (animator != null) animator.SetBool("Running", false);
            return;
        }

        // Aumentar velocidad con el tiempo
        if (forwardSpeed < maxSpeed)
            forwardSpeed += speedIncreaseRate * Time.deltaTime;

        moveDirection = Vector3.forward * forwardSpeed;

        // ✅ Entradas
        HandleInput();

        // Slide activo
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
                EndSlide();
        }

        // Movimiento vertical
        moveDirection.y = verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void HandleInput()
    {
        // --- Cambiar carril (PC o móvil)
        if (!useMobileControls)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) currentLane = Mathf.Max(0, currentLane - 1);
            if (Input.GetKeyDown(KeyCode.RightArrow)) currentLane = Mathf.Min(2, currentLane + 1);
        }
        else
        {
            DetectSwipe();
        }

        // --- Movimiento en X
        float targetX = (currentLane - 1) * laneDistance;
        moveDirection.x = (targetX - transform.position.x) * 10f;

        // --- Saltar / Slide
        if (controller.isGrounded)
        {
            // 👇 Solo resetear si no se está aplicando salto
            if (verticalVelocity < 0f)
                verticalVelocity = -1;

            if (!useMobileControls)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
                    Jump();

                if (Input.GetKeyDown(KeyCode.DownArrow))
                    StartSlide();
            }

            if (!isSliding && animator != null)
                animator.SetBool("Running", true);
        }
        else
        {
            // Gravedad
            if (Input.GetKey(KeyCode.DownArrow))
                verticalVelocity -= gravity * fastFallMultiplier * Time.deltaTime;
            else
                verticalVelocity -= gravity * Time.deltaTime;

            if (animator != null)
                animator.SetBool("Running", false);
        }
    }

    private void DetectSwipe()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
        {
            startTouch = t.position;
            isSwiping = true;
        }
        else if (t.phase == TouchPhase.Ended && isSwiping)
        {
            Vector2 swipeDelta = t.position - startTouch;
            isSwiping = false;

            if (swipeDelta.magnitude < minSwipeDistance) return;

            swipeDelta.Normalize();

            // Determinar dirección del swipe
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                // Horizontal
                if (swipeDelta.x > 0) currentLane = Mathf.Min(2, currentLane + 1); // derecha
                else currentLane = Mathf.Max(0, currentLane - 1); // izquierda
            }
            else
            {
                // Vertical
                if (swipeDelta.y > 0)
                {
                    Debug.Log("📱 Swipe detectado: ARRIBA -> Saltar");
                    Jump();
                }
                else
                {
                    Debug.Log("📱 Swipe detectado: ABAJO -> Slide");
                    StartSlide();
                }
            }
        }
    }

    private void Jump()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = jumpForce;
            Debug.Log("🚀 Jump ejecutado con fuerza: " + jumpForce);

            if (animator != null) animator.SetTrigger("Jump");
        }
    }

    private void StartSlide()
    {
        if (isSliding || !controller.isGrounded) return;

        isSliding = true;
        slideTimer = slideDuration;

        float bottomBefore = controller.center.y - (controller.height * 0.5f);
        controller.height = slideHeight;
        controller.center = new Vector3(
            controller.center.x,
            bottomBefore + (controller.height * 0.5f) + slideYOffset,
            controller.center.z
        );

        if (animator != null)
        {
            animator.SetTrigger("Slide");
            animator.SetBool("Running", false);
        }
    }

    private void EndSlide()
    {
        isSliding = false;
        controller.height = originalHeight;
        controller.center = originalCenter;

        if (animator != null) animator.SetBool("Running", true);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle"))
            Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null) animator.SetTrigger("Die");

        AudioManager.instance.PlayFX(AudioManager.instance.playerDeathFX);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverText != null)
                gameOverText.text = "Has Perdido";
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

        if (animator != null)
        {
            animator.SetBool("Running", true);
            animator.ResetTrigger("Die");
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        GroundManager groundManager = FindObjectOfType<GroundManager>();
        if (groundManager != null)
            groundManager.ResetGround();

        Time.timeScale = 1f;
    }
}
