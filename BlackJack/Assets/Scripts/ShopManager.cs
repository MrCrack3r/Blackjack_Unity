using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("UI Elementai")]
    public TextMeshProUGUI notificationText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShopMusic();
        }

        if (notificationText != null) notificationText.gameObject.SetActive(false);
    }

    // ==========================================
    // PIRKIMO FUNKCIJA
    // ==========================================
    public void TryBuyPowerUp(PowerUpData data)
    {
        if (data == null) return;

        // 1. Patikriname ar žaidėjas turi pakankamai pinigų
        if (RunManager.instance != null && RunManager.instance.playerMoney >= data.baseCost)
        {
            // 2. Bandome pridėti į inventorių
            if (InventoryManager.instance != null)
            {
                bool success = InventoryManager.instance.AddPowerUp(data);

                if (success)
                {
                    // Atimame pinigus
                    RunManager.instance.playerMoney -= data.baseCost;

                    // Parodome sėkmės pranešimą
                    ShowNotification("Purchased: " + data.powerUpName, Color.green);
                    Debug.Log("Bought powerup: " + data.powerUpName);
                }
                else
                {
                    // Inventorius pilnas (jau turi 4 kortas)
                    ShowNotification("Inventory Full!", Color.red);
                }
            }
        }
        else
        {
            // Nepakanka pinigų
            ShowNotification("Not enough money!", Color.red);
        }
    }

    public void ShowNotification(string message, Color color)
    {
        if (notificationText != null)
        {
            StopAllCoroutines();
            StartCoroutine(NotificationRoutine(message, color));
        }
    }

    private IEnumerator NotificationRoutine(string message, Color color)
    {
        notificationText.text = message;
        notificationText.color = color;
        notificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        notificationText.gameObject.SetActive(false);
    }

    public void ContinueToNextRound()
    {
        Debug.Log("Parduotuvė uždaroma. Pradedamas kitas raundas!");

        if (RunManager.instance != null)
        {
            RunManager.instance.NextRound();
        }

        SceneManager.LoadScene("Backjack_table_scene");
    }
}