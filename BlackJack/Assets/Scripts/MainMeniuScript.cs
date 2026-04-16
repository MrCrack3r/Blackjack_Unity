using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Backjack_table_scene");
    }

    public void ExitGame()
    {
        Debug.Log("Išeinama iš žaidimo...");
        Application.Quit();
    }
}