using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMeniu : MonoBehaviour
{
    [Header("UI Elementai")]
    public GameObject pauseMenuPanel;
    public Slider soundSlider;

    [Header("Garso Ikona")]
    public Image soundIconImage;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private bool isPaused = false;
    private float savedVolumeBeforeMute = 1f;

    void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        float initialVolume = AudioListener.volume;

        if (soundSlider != null)
        {
            soundSlider.SetValueWithoutNotify(initialVolume);
        }

        // Kviečiame be jokių skaičių skliausteliuose!
        UpdateIcon();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame() { pauseMenuPanel.SetActive(true); Time.timeScale = 0f; isPaused = true; }
    public void ResumeGame() { pauseMenuPanel.SetActive(false); Time.timeScale = 1f; isPaused = false; }
    public void RestartGame() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void LoadMainMenu() { Time.timeScale = 1f; SceneManager.LoadScene("Main_menu_scene"); }

    // ==========================================
    // --- GARSO FUNKCIJOS BE JOKIŲ KONFLIKTŲ ---
    // ==========================================

    public void OnSliderValueChanged(float volume)
    {
        AudioListener.volume = volume;

        if (volume > 0.01f)
        {
            savedVolumeBeforeMute = volume;
        }

        // Kviečiame be jokių skaičių!
        UpdateIcon();
    }

    public void ToggleMute()
    {
        bool isCurrentlyMuted = AudioListener.volume <= 0.01f;

        if (isCurrentlyMuted)
        {
            if (savedVolumeBeforeMute < 0.01f) savedVolumeBeforeMute = 0.5f;

            AudioListener.volume = savedVolumeBeforeMute;

            if (soundSlider != null)
            {
                soundSlider.SetValueWithoutNotify(savedVolumeBeforeMute);
            }
        }
        else
        {
            savedVolumeBeforeMute = AudioListener.volume;
            AudioListener.volume = 0f;

            if (soundSlider != null)
            {
                soundSlider.SetValueWithoutNotify(0f);
            }
        }

        // Kviečiame be jokių skaičių!
        UpdateIcon();
    }

    // Funkcija pati žino, kur ieškoti garso lygio
    private void UpdateIcon()
    {
        if (soundIconImage != null)
        {
            // Tikriname patį tikriausią šaltinį - Unity AudioListener
            if (AudioListener.volume <= 0.01f)
            {
                soundIconImage.sprite = soundOffSprite;
            }
            else
            {
                soundIconImage.sprite = soundOnSprite;
            }
        }
    }
}