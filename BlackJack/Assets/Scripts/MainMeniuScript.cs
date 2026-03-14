// MainMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Funkcija žaidimo pradžiai
    public void StartGame()
    {
        // Naudojame scenos pavadinimą iš jūsų projekto struktūros
        SceneManager.LoadScene("Backjack_table_scene");
    }

    // Funkcija išėjimui iš žaidimo
    public void ExitGame()
    {
        Application.Quit();
    }
}