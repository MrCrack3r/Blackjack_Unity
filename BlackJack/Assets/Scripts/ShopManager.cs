using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    public Button continueButton;

    void Start()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
        else
        {
            Debug.LogError("Continue Button nėra priskirtas!");
        }
    }

    public void OnContinueClicked()
    {
        Debug.Log("Continue paspaustas!");
        StartCoroutine(TransitionToGame());
    }

    private IEnumerator TransitionToGame()
    {
        yield return new WaitForSeconds(0.5f);
        RunManager.instance.NextRound();
        SceneManager.LoadScene("Backjack_table_scene");
    }
}