using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        // Patikriname, ar Instance egzistuoja prieš jį naudojant
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }
        else
        {
            Debug.LogWarning("AudioManager nerastas scenoje!");
        }
    }

    public void StartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }
        SceneManager.LoadScene("Backjack_table_scene");
    }

    public void ExitGame()
    {
        Debug.Log("Išeinama iš žaidimo...");
        Application.Quit();
    }
}