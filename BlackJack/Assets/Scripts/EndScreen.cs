using UnityEngine;
using UnityEngine.SceneManagement; // Būtina eilutė, kad galėtume keisti scenas!

// Klasės pavadinimas (EndScreen) privalo tiksliai atitikti failo pavadinimą (EndScreen.cs)
public class EndScreen : MonoBehaviour
{
    // Funkcija skirta "MAIN MENU" mygtukui
    public void GoToMainMenu()
    {
        Debug.Log("Grįžtama į pagrindinį meniu...");
        // Įkeliame meniu sceną. Įsitikinkite, kad pavadinimas tiksliai toks!
        SceneManager.LoadScene("Main_menu_scene");
    }

    // Funkcija skirta "DEAL AGAIN" mygtukui
    public void DealAgain()
    {
        Debug.Log("Kraunamas žaidimas iš naujo...");
        // Įkeliame žaidimo stalą. Įsitikinkite, kad pavadinimas tiksliai toks!
        SceneManager.LoadScene("Blackjack_table_scene");
    }
}
