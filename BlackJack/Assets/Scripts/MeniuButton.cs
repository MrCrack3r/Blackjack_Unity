using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Debug.Log("Grįžtama į pagrindinį meniu...");
		RunManager.instance.SaveGame();
		SceneManager.LoadScene("Main_menu_scene");
    }
}