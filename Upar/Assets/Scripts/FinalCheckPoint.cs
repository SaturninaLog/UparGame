using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalCheckpoint : MonoBehaviour
{
    [Header("Nombre de la escena a cargar")]
    public string sceneToLoad = "EscenarioPlazaAlfonsoLopez";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.PlayFX(AudioManager.instance.coinCollectedFX);
            Debug.Log("¡Has llegado al final del nivel!");

            // 🔹 Llamamos al Fade antes de cargar la escena
            DeathScreenManager.instance.PlayDeathFade(() =>
            {
                SceneManager.LoadScene(sceneToLoad);
            });
        }
    }
}
