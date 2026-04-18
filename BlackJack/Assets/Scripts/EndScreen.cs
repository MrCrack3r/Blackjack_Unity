using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayLoseMusic();
    }

    public void GoToMainMenu()
    {
        Debug.Log("Grįžtama į pagrindinį meniu...");
        RunManager.instance.ResetRun();
        AudioManager.Instance.PlayMenuMusic();
        SceneManager.LoadScene("Main_menu_scene");
    }

    public void DealAgain()
    {
        Debug.Log("Kraunamas žaidimas iš naujo...");
        RunManager.instance.ResetRun();
        AudioManager.Instance.PlayGameplayMusic();
        SceneManager.LoadScene("Backjack_table_scene");
    }
}