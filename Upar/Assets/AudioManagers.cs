using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Musica de Fondo")]
    public AudioSource musicSource;      // Música general
    public AudioClip menuMusic;
    public AudioClip centerMusic;
    public AudioClip inGameMusic;
    public AudioClip plazaMusic;

    [Header("FX")]
    public AudioSource fxSource;         // Efectos individuales

    [Header("FX del Juego")]
    public AudioClip selectButtonFX;
    public AudioClip deselectButtonFX;
    public AudioClip jumpFX;
    public AudioClip coinCollectedFX;
    public AudioClip playerDeathFX;
    public AudioClip pauseButtonFX;

    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float fxVolume = 1f;

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
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 🔹 Reasignar musicSource si existe en la nueva escena
        AudioSource newMusicSource = GameObject.Find("MusicSource")?.GetComponent<AudioSource>();
        if (newMusicSource != null)
        {
            musicSource = newMusicSource;
        }
        else if (musicSource == null)
        {
            // Si no hay MusicSource en la escena, crea uno temporal
            GameObject musicObj = new GameObject("MusicSource");
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            DontDestroyOnLoad(musicObj);
        }

        // 🔹 Reasignar fxSource si existe en la nueva escena
        AudioSource newFXSource = GameObject.Find("FXSource")?.GetComponent<AudioSource>();
        if (newFXSource != null)
        {
            fxSource = newFXSource;
        }
        else if (fxSource == null)
        {
            GameObject fxObj = new GameObject("FXSource");
            fxSource = fxObj.AddComponent<AudioSource>();
            DontDestroyOnLoad(fxObj);
        }

        // Reproducir música correspondiente a la escena
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        if (musicSource == null) return;

        AudioClip clipToPlay = null;

        switch (sceneName)
        {
            case "MenuInicio":
                clipToPlay = menuMusic;
                break;
            case "Center":
                clipToPlay = centerMusic;
                break;
            case "InGame":
                clipToPlay = inGameMusic;
                break;
            case "EscenarioPlazaAlfonsoLopez":
                clipToPlay = plazaMusic;
                break;
            default:
                clipToPlay = inGameMusic;
                break;
        }

        if (clipToPlay != null && musicSource.clip != clipToPlay)
        {
            musicSource.clip = clipToPlay;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayFX(AudioClip clip)
    {
        if (fxSource != null && clip != null)
        {
            fxSource.PlayOneShot(clip, fxVolume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    public void SetFXVolume(float volume)
    {
        fxVolume = Mathf.Clamp01(volume);
    }
}
