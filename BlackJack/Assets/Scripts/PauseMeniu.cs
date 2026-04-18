using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMeniu : MonoBehaviour
{
    [Header("UI Elementai")]
    public GameObject pauseMenuPanel;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Garso Ikonos")]
    public Image musicIconImage; // Pakeista iš soundIconImage
    public Image sfxIconImage;   // Pridėta SFX ikonai
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private bool isPaused = false;

    // Kintamieji garsumo atsiminimui po nutildymo
    private float savedMusicVolume = 0.5f;
    private float savedSFXVolume = 0.7f;

    void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.7f);

        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterMusicVolume(savedMusic);
            AudioManager.Instance.SetMasterSFXVolume(savedSFX);
        }

        UpdateIcons();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PauseMusic();
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        if (AudioManager.Instance != null) AudioManager.Instance.UnPauseMusic();
    }

    public void RestartGame() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void LoadMainMenu() { Time.timeScale = 1f; SceneManager.LoadScene("Main_menu_scene"); }

    // --- MUZIKOS VALDYMAS ---
    public void SetMusicVolume(float volume)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMasterMusicVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        UpdateIcons();
    }

    public void ToggleMusicMute()
    {
        if (musicSlider.value > 0.01f)
        {
            savedMusicVolume = musicSlider.value;
            musicSlider.value = 0f;
        }
        else
        {
            musicSlider.value = (savedMusicVolume > 0.01f) ? savedMusicVolume : 0.5f;
        }
    }

    // --- SFX VALDYMAS ---
    public void SetSFXVolume(float volume)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMasterSFXVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        UpdateIcons();
    }

    public void ToggleSFXMute()
    {
        if (sfxSlider.value > 0.01f)
        {
            savedSFXVolume = sfxSlider.value;
            sfxSlider.value = 0f;
        }
        else
        {
            sfxSlider.value = (savedSFXVolume > 0.01f) ? savedSFXVolume : 0.7f;
        }
    }

    private void UpdateIcons()
    {
        if (musicIconImage != null && musicSlider != null)
            musicIconImage.sprite = (musicSlider.value > 0.01f) ? soundOnSprite : soundOffSprite;

        if (sfxIconImage != null && sfxSlider != null)
            sfxIconImage.sprite = (sfxSlider.value > 0.01f) ? soundOnSprite : soundOffSprite;
    }
}