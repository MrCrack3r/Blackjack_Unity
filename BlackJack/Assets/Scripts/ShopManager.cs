using UnityEngine;
using TMPro;
using System.Collections;

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
}