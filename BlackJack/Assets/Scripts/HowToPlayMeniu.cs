using UnityEngine;

public class HowToPlayMenu : MonoBehaviour
{
    [Header("UI Elementai")]
    public GameObject howToPlayPanel;

    void Start()
    {
        // Žaidimo pradžioje užtikriname, kad langas būtų paslėptas
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    // Funkcija atidaryti langą
    public void OpenHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    // Funkcija uždaryti langą
    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }
}
