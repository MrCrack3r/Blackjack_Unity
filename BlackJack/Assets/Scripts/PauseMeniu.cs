using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem; // <-- PRIDĖTA: Būtina naujajai įvesties sistemai

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Elementai")]
    public GameObject pauseMenuPanel;
    public Slider soundSlider;

    private bool isPaused = false;
    private float previousVolume = 1f;

    void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (soundSlider != null)
        {
            soundSlider.value = AudioListener.volume;
        }
    }

    void Update()
    {
        // <-- PAKEISTA: Naudojame naująją Input System, kad patikrintume ESC mygtuką
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // --- MYGTUKŲ FUNKCIJOS ---

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_menu_scene");
    }

    // --- GARSO FUNKCIJOS ---

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;

        if (volume > 0)
        {
            previousVolume = volume;
        }
    }

    public void ToggleMute()
    {
        if (AudioListener.volume > 0)
        {
            AudioListener.volume = 0f;
            if (soundSlider != null) soundSlider.value = 0f;
        }
        else
        {
            AudioListener.volume = previousVolume;
            if (soundSlider != null) soundSlider.value = previousVolume;
        }
    }
}