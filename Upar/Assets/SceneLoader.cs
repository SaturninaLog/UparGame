using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class SceneLoader : MonoBehaviour
{
    // Esta función la puedes llamar desde el botón
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
