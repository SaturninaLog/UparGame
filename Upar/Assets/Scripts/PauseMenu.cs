using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel; // Panel del menú de pausa
    private bool isPaused = false;

    void Update()
    {
        // Detecta si se presiona ESC o P
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        AudioManager.instance.PlayFX(AudioManager.instance.pauseButtonFX);
        pausePanel.SetActive(true);  // Activa el panel
        Time.timeScale = 0f;         // Pausa el juego
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false); // Oculta el panel
        Time.timeScale = 1f;         // Reanuda el juego
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Asegura que vuelva a la normalidad
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f; // Restablece el tiempo
        SceneManager.LoadScene("MenuInicio"); // 👈 Cambia "Menu" por el nombre real de tu escena de inicio
    }
}
