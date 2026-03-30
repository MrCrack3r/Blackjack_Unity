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
        // Šis pranešimas pasirodys tik Unity redaktoriaus konsolėje,
        // kad žinotumėte, jog mygtukas veikia testuojant.
        Debug.Log("Išeinama iš žaidimo...");

        // Ši komanda IŠJUNGS patį žaidimą, kai jis bus sukompiliuotas (Build).
        // SVARBU: Ji neveikia pačiame Unity redaktoriuje (paspaudus Play)!
        Application.Quit();
    }
}