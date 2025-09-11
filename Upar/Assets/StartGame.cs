using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public Animator childAnimator;   // El Animator del niño
    public Transform childTransform; // Transform del niño
    public float runSpeed = 2f;      // Velocidad de avance
    public string nextScene = "InGame"; // Nombre de la escena a cargar
    public Image fadeImage;          // Imagen negra para el fade (UI)
    public float fadeDuration = 1f;  // Tiempo del fade

    private bool isRunning = false;
    private bool startFade = false;
    private float fadeTimer = 0f;

    void Update()
    {
        if (isRunning)
        {
            // 🔹 Avanzar en +Z
            childTransform.Translate(Vector3.forward * runSpeed * Time.deltaTime);

            // Iniciar fade si no empezó aún
            if (!startFade)
            {
                startFade = true;
                fadeTimer = 0f;
            }
        }

        if (startFade)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);

            if (alpha >= 1)
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    public void OnStartButtonPressed()
    {
        Debug.Log("Botón presionado → Activando animación Run");
        if (childAnimator != null)
        {
            childAnimator.SetTrigger("Run");
        }
        else
        {
            Debug.LogWarning("⚠ No hay Animator asignado al script");
        }
        isRunning = true;
    }
}
