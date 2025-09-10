using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class SceneLoader : MonoBehaviour
{
    // Esta funci�n la puedes llamar desde el bot�n
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
