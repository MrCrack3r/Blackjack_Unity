using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("UI Elementai")]
    public TextMeshProUGUI notificationText; // Tekstas, kuris iššoks perkant

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Paslepiame pranešimą parduotuvės atidarymo metu
        if (notificationText != null) notificationText.gameObject.SetActive(false);
    }

    // Funkcija, kurią iškviesime norėdami parodyti tekstą
    public void ShowNotification(string message, Color color)
    {
        if (notificationText != null)
        {
            StopAllCoroutines(); // Sustabdome senus pranešimus, jei žaidėjas spaudinėja greitai
            StartCoroutine(NotificationRoutine(message, color));
        }
    }

    private IEnumerator NotificationRoutine(string message, Color color)
    {
        notificationText.text = message;
        notificationText.color = color;
        notificationText.gameObject.SetActive(true);

        // Rodome pranešimą 2 sekundes
        yield return new WaitForSeconds(2f);

        notificationText.gameObject.SetActive(false);
    }

    // ==========================================
    // NAUJA FUNKCIJA "CONTINUE" MYGTUKUI
    // ==========================================
    public void ContinueToNextRound()
    {
        Debug.Log("Parduotuvė uždaroma. Pradedamas kitas raundas!");

        // 1. Padidiname raundą ir atstatome partijų skaičių (naudojame jūsų RunManager funkciją)
        if (RunManager.instance != null)
        {
            RunManager.instance.NextRound();
        }

        // 2. Grįžtame atgal į žaidimo stalą. 
        // SVARBU: Pavadinimas kabutėse turi TIKSLIAI atitikti jūsų žaidimo scenos failo pavadinimą!
        SceneManager.LoadScene("Backjack_table_scene");
    }
}