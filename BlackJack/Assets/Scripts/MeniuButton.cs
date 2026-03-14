using UnityEngine;
using UnityEngine.SceneManagement; // BŪTINA scenų keitimui!

public class GameMenuController : MonoBehaviour
{
    // Ši funkcija grąžins mus į pagrindinį meniu
    public void GoToMainMenu()
    {
        Debug.Log("Grįžtama į pagrindinį meniu...");
        // Įrašykite TIKSLŲ savo pagrindinio meniu scenos pavadinimą
        SceneManager.LoadScene("Main_menu_scene");
    }
}