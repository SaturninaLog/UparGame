using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeathScreenManager : MonoBehaviour
{
    public static DeathScreenManager instance;

    [Header("Imagen negra que cubre la pantalla")]
    public Image deathImage;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void PlayDeathFade(System.Action onFadeComplete)
    {
        StartCoroutine(FadeRoutine(onFadeComplete));
    }

    private IEnumerator FadeRoutine(System.Action onFadeComplete)
    {
        Color color = deathImage.color;

        // 🔹 Fade In rápido al morir
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            deathImage.color = color;
            yield return null;
        }
        color.a = 1;
        deathImage.color = color;

        // 🔹 Respawn mientras pantalla negra está activa
        onFadeComplete?.Invoke();

        // 🔹 Espera un momento para tapar el tirón de cámara
        yield return new WaitForSeconds(0.2f);

        // 🔹 Fade Out para volver a jugar
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            deathImage.color = color;
            yield return null;
        }
        color.a = 0;
        deathImage.color = color;
    }
}
