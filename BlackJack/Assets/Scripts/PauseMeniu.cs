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
    public Image musicIconImage;
    public Image sfxIconImage;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private bool isPaused = false;

    private float savedMusicVolume = 0.5f;
    private float savedSFXVolume = 1f;

    void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(savedMusic);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(savedSFX);
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

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (RunManager.instance != null)
        {
            RunManager.instance.ResetRun();
        }

        InventoryManager.ClearInventory();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        if (RunManager.instance != null)
        {
            RunManager.instance.SaveGame();
        }
        SceneManager.LoadScene("Main_menu_scene");
        InventoryManager.ClearInventory();
    }

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
            sfxSlider.value = (savedSFXVolume > 0.01f) ? savedSFXVolume : 1f;
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