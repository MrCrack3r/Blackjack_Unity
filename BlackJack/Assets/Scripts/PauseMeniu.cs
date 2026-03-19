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
    private bool ignoreSliderChange = false; // Saugiklis nuo kodo sukelto judėjimo
    private float savedVolumeBeforeMute = 1f; // Prisimena garsą PRIEŠ paspaudžiant Mute

    void Start()
    {
        // Paslepiame meniu
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        // Teisingai nustatome pradinę būseną
        float initialVolume = AudioListener.volume;
        if (soundSlider != null) soundSlider.value = initialVolume;

        UpdateIconBasedOnVolume(initialVolume);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // --- Pauzės valdymas ---
    public void PauseGame() { pauseMenuPanel.SetActive(true); Time.timeScale = 0f; isPaused = true; }
    public void ResumeGame() { pauseMenuPanel.SetActive(false); Time.timeScale = 1f; isPaused = false; }
    public void RestartGame() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void LoadMainMenu() { Time.timeScale = 1f; SceneManager.LoadScene("Main_menu_scene"); }

    // --- Garso valdymas ---

    // Iškviečiama, kai ŽAIDĖJAS JUDA SLANKIKLĮ
    public void OnSliderValueChanged(float volume)
    {
        if (ignoreSliderChange) return;

        AudioListener.volume = volume;
        UpdateIconBasedOnVolume(volume);
    }

    // Iškviečiama, kai paspaudžiamas MUTE mygtukas
    public void ToggleMute()
    {
        bool isCurrentlyMuted = AudioListener.volume <= 0.01f;

        ignoreSliderChange = true; // ĮJUNGTI SAUGIKLĮ

        if (isCurrentlyMuted)
        {
            // --- GRĄŽINA GARSĄ ---
            // Apsauga, jei savedVolume netyčia 0
            if (savedVolumeBeforeMute < 0.01f) savedVolumeBeforeMute = 0.5f;

            AudioListener.volume = savedVolumeBeforeMute;
            if (soundSlider != null) soundSlider.value = savedVolumeBeforeMute;
        }
        else
        {
            // --- UŽTILDO ---
            savedVolumeBeforeMute = AudioListener.volume; // Išsaugome dabartinį garsą
            AudioListener.volume = 0f;
            if (soundSlider != null) soundSlider.value = 0f;
        }

        ignoreSliderChange = false; // IŠJUNGTI SAUGIKLĮ

        UpdateIconBasedOnVolume(AudioListener.volume);
    }

    // Pagalbinė funkcija, kuri visada teisingai atnaujina ikoną
    private void UpdateIconBasedOnVolume(float volume)
    {
        if (soundIconImage != null)
        {
            if (volume <= 0.01f)
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