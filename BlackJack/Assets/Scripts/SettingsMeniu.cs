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
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (AudioManager.Instance != null)
        {
            if (musicSlider != null)
            {
                musicSlider.SetValueWithoutNotify(AudioManager.Instance.masterMusicVolume);
                musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            }
            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(AudioManager.Instance.masterSFXVolume);
                sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
            }
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
    }

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