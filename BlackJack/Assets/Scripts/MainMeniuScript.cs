using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	public GameObject continueButton;

	void Start()
	{
		

		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayMenuMusic();
		}
		else
		{
			Debug.LogWarning("AudioManager nerastas scenoje!");
		}

		
		Button btn = continueButton.GetComponent<Button>();

		if (!PlayerPrefs.HasKey("Money"))
		{
			btn.interactable = false; 
		}
		else
		{
			btn.interactable = true;
		}
	}

	public void StartGame()
    {
		RunManager.instance.ResetRun();

		if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }
        SceneManager.LoadScene("Backjack_table_scene");


    }

	public void ContinueGame()
	{
		Debug.Log("Tęsiamas žaidimas...");

		RunManager.instance.LoadGame(); 

		SceneManager.LoadScene("Backjack_table_scene");
	}

	public void ExitGame()
    {
        Debug.Log("Išeinama iš žaidimo...");
        Application.Quit();
    }



}