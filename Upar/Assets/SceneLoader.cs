using UnityEngine;
using UnityEngine.SceneManagement; // Para cargar escenas

public class SceneLoader : MonoBehaviour
{
    // --- Para cargar escenas ---
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // --- Para abrir/cerrar paneles ---
    // Recibe un panel (GameObject) y lo activa
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    // Recibe un panel (GameObject) y lo desactiva
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    // Alterna el estado del panel (si está activo lo cierra, si no lo abre)
    public void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
}
