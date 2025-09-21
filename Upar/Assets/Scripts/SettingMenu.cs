using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider fxSlider;

    private void OnEnable()
    {
        // 🔹 Sincronizamos sliders con el AudioManager actual
        if (AudioManager.instance != null)
        {
            if (musicSlider != null)
                musicSlider.value = AudioManager.instance.musicVolume;

            if (fxSlider != null)
                fxSlider.value = AudioManager.instance.fxVolume;
        }

        // 🔹 Suscribimos listeners
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        fxSlider.onValueChanged.AddListener(SetFXVolume);
    }

    private void OnDisable()
    {
        // 🔹 Para evitar suscripciones duplicadas cada vez que abras el panel
        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        fxSlider.onValueChanged.RemoveListener(SetFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetMusicVolume(value);
    }

    public void SetFXVolume(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetFXVolume(value);
    }
}
