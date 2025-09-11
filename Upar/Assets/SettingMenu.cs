using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider fxSlider;

    void Start()
    {
        musicSlider.value = AudioManager.instance.musicVolume;
        fxSlider.value = AudioManager.instance.fxVolume;

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        fxSlider.onValueChanged.AddListener(SetFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetMusicVolume(value);
    }

    public void SetFXVolume(float value)
    {
        AudioManager.instance.SetFXVolume(value);
    }
}
