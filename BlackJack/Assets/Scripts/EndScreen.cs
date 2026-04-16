using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Debug.Log("Grįžtama į pagrindinį meniu...");
        SceneManager.LoadScene("Main_menu_scene");
    }

    public void DealAgain()
    {
        Debug.Log("Kraunamas žaidimas iš naujo...");
        SceneManager.LoadScene("Backjack_table_scene");
    }
}
