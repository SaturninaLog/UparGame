using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PersistentSettingsManager : MonoBehaviour
{
    public static PersistentSettingsManager instance;

    [Header("Nombres de los sliders en la escena (gameObject names)")]
    public string musicSliderName = "MusicSlider";
    public string fxSliderName = "FXSlider";

    private Slider currentMusicSlider;
    private Slider currentFxSlider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // cada vez que cargue escena intentamos encontrar los sliders
        FindAndAssignSliders();
    }

    /// <summary>
    /// Busca y asigna sliders en la escena actual.
    /// Se puede llamar manualmente si prefieres (por ejemplo desde el panel al abrir).
    /// </summary>
    public void FindAndAssignSliders()
    {
        Slider foundMusic = FindSliderByName(musicSliderName);
        Slider foundFx = FindSliderByName(fxSliderName);

        if (foundMusic != null) AssignMusicSlider(foundMusic);
        else UnassignMusicSlider();

        if (foundFx != null) AssignFXSlider(foundFx);
        else UnassignFxSlider();
    }

    private Slider FindSliderByName(string sliderName)
    {
        var scene = SceneManager.GetActiveScene();
        var roots = scene.GetRootGameObjects();

        foreach (var root in roots)
        {
            // Include inactive children
            var sliders = root.GetComponentsInChildren<Slider>(true);
            foreach (var s in sliders)
            {
                if (s.gameObject.name == sliderName)
                    return s;
            }
        }
        return null;
    }

    private void AssignMusicSlider(Slider s)
    {
        if (s == null) return;
        if (currentMusicSlider == s) return;

        // quitamos la suscripción anterior (si había)
        if (currentMusicSlider != null)
            currentMusicSlider.onValueChanged.RemoveListener(SetMusicVolume);

        currentMusicSlider = s;

        // inicializamos el valor al que tiene AudioManager
        if (AudioManager.instance != null)
            currentMusicSlider.value = AudioManager.instance.musicVolume;

        // añadimos listener
        currentMusicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    private void AssignFXSlider(Slider s)
    {
        if (s == null) return;
        if (currentFxSlider == s) return;

        if (currentFxSlider != null)
            currentFxSlider.onValueChanged.RemoveListener(SetFXVolume);

        currentFxSlider = s;

        if (AudioManager.instance != null)
            currentFxSlider.value = AudioManager.instance.fxVolume;

        currentFxSlider.onValueChanged.AddListener(SetFXVolume);
    }

    private void UnassignMusicSlider()
    {
        if (currentMusicSlider != null)
        {
            currentMusicSlider.onValueChanged.RemoveListener(SetMusicVolume);
            currentMusicSlider = null;
        }
    }

    private void UnassignFxSlider()
    {
        if (currentFxSlider != null)
        {
            currentFxSlider.onValueChanged.RemoveListener(SetFXVolume);
            currentFxSlider = null;
        }
    }

    // Métodos que se llaman cuando los sliders cambian
    private void SetMusicVolume(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetMusicVolume(value);
    }

    private void SetFXVolume(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetFXVolume(value);
    }

    /// <summary>
    /// Llama a esto si quieres forzar la búsqueda (por ejemplo desde el panel al abrir).
    /// </summary>
    public void OnSettingsPanelOpened()
    {
        FindAndAssignSliders();
    }
}
