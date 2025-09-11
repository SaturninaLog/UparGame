using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;     // La imagen del fade
    public float fadeDuration = 0.2f; // Tiempo del fade

    public static ScreenFader Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public IEnumerator FadeOutIn(System.Action onMidFade)
    {
        // Fade Out
        yield return StartCoroutine(Fade(0f, 1f));

        // Acción intermedia (cambiar de nivel, mover jugador, etc.)
        onMidFade?.Invoke();

        // Fade In
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float start, float end)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(start, end, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = end;
        fadeImage.color = c;
    }
}
