using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("UI Elementai")]
    public TextMeshProUGUI notificationText;

    [Header("Visos Galimos Kortos Žaidime")]
    public List<PowerUpData> allAvailablePowerUps;

    [Header("Parduotuvės Prekės (3 Slotai)")]
    public ShopItem[] shopSlots;

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

        GenerateRandomShop();
    }

    private void GenerateRandomShop()
    {

        if (allAvailablePowerUps == null || allAvailablePowerUps.Count == 0) return;

        List<PowerUpData> availableCards = new List<PowerUpData>(allAvailablePowerUps);

        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (shopSlots[i] == null) continue;
            if (availableCards.Count == 0) break;

            int randomIndex = Random.Range(0, availableCards.Count);
            PowerUpData randomCard = availableCards[randomIndex];

            availableCards.RemoveAt(randomIndex);

            shopSlots[i].Setup(randomCard);
            shopSlots[i].gameObject.SetActive(true);
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
        if (RunManager.instance != null)
        {
            RunManager.instance.NextRound();
        }

        SceneManager.LoadScene("Backjack_table_scene");
    }
}