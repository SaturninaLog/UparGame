using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float forwardSpeed = 10f;        // Velocidad inicial
    public float maxSpeed = 30f;            // Velocidad máxima
    public float speedIncreaseRate = 0.5f;  // Qué tan rápido acelera
    public float laneDistance = 3f;         // Distancia entre carriles
    private int currentLane = 1; // 0 = izquierda, 1 = centro, 2 = derecha

    private CharacterController controller;
    private Vector3 moveDirection;
    public float jumpForce = 8f;
    public float gravity = 20f;
    private float verticalVelocity;

    private bool isDead = false;

    [Header("UI Game Over")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public Button restartButton;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);
    }

    // 💡 NUEVO: Función para reiniciar el jugador
    public void ResetPlayer()
    {
        isDead = false;
        forwardSpeed = 10f;
        currentLane = 1;
        transform.position = Vector3.zero; // O la posición inicial que prefieras
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        // 🔥 Aumento progresivo de velocidad (limitado a maxSpeed)
        if (forwardSpeed < maxSpeed)
            forwardSpeed += speedIncreaseRate * Time.deltaTime;

        // Movimiento hacia adelante
        moveDirection = Vector3.forward * forwardSpeed;

        // Movimiento entre carriles
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentLane = Mathf.Max(0, currentLane - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            currentLane = Mathf.Min(2, currentLane + 1);

        // Posición horizontal (suavizado)
        float targetX = (currentLane - 1) * laneDistance;
        moveDirection.x = (targetX - transform.position.x) * 10f;

        // Salto
        if (controller.isGrounded)
        {
            verticalVelocity = -1;
            if (Input.GetKeyDown(KeyCode.Space))
                verticalVelocity = jumpForce;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;

        controller.Move(moveDirection * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
        // 💡 CAMBIO: Llama a GameManager para mostrar la imagen del souvenir
        if (hit.gameObject.CompareTag("Souvenir"))
        {
            // 💡 CAMBIO: Ya no pasamos el índice. El GameManager lo gestiona.
            GameManager.instance.ShowSouvenirImage();
            Destroy(hit.gameObject);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("☠️ Game Over");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverText != null)
                gameOverText.text = "☠️ Game Over";
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
