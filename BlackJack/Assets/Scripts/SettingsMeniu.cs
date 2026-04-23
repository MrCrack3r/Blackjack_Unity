using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Elementai")]
    public GameObject settingsPanel;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // 1. Žaidimo pradžioje paslepiame nustatymų langą
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // 2. Nustatome slankiklių pozicijas pagal išsaugotas AudioManager reikšmes
        if (AudioManager.Instance != null)
        {
            if (musicSlider != null) musicSlider.value = AudioManager.Instance.masterMusicVolume;
            if (sfxSlider != null) sfxSlider.value = AudioManager.Instance.masterSFXVolume;
        }
    }

    // --- Mygtukų funkcijos ---
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        // Sugrojame mygtuko garsą atidarant
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
    }

    // --- Slankiklių (Slider) funkcijos ---
    public void OnMusicSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterMusicVolume(value);
        }
    }

    public void OnSFXSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterSFXVolume(value);
        }
    }
}