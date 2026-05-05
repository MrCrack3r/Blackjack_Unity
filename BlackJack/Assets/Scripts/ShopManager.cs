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
    public TextMeshProUGUI budgetText;

    [Header("Visos Galimos Kortos Žaidime")]
    public List<PowerUpData> allAvailablePowerUps;

    [Header("Parduotuvės Prekės (3 Slotai)")]
    public ShopItem[] shopSlots;

    [Header("Pirkimo Taisyklės")]
    public int minimumBetRequired = 1;

    [Header("Kainų Infliacija")]
    [Tooltip("Kiek procentų brangsta prekė kiekviename raunde (pvz. 0.2 = 20% brangiau kiekvieną raundą)")]
    public float priceMultiplierPerRound = 0.2f;

    [Header("Life Item (visada rodomas)")]
    public PowerUpData lifeItem;
    public ShopItem lifeSlot;

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

        UpdateBudgetUI();
        GenerateRandomShop();
    }

    public void UpdateBudgetUI()
    {
        if (budgetText != null && RunManager.instance != null)
        {
            budgetText.text = "$" + RunManager.instance.playerMoney;
        }
    }

    private void GenerateRandomShop()
    {
        if (allAvailablePowerUps == null || allAvailablePowerUps.Count == 0) return;

        List<PowerUpData> availableCards = new List<PowerUpData>(allAvailablePowerUps);

        availableCards.Remove(lifeItem);

        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (shopSlots[i] == null) continue;
            if (availableCards.Count == 0) break;

            int randomIndex = Random.Range(0, availableCards.Count);
            PowerUpData randomCard = availableCards[randomIndex];

            availableCards.RemoveAt(randomIndex);

            int dynamicPrice = CalculateDynamicPrice(randomCard.baseCost);
            shopSlots[i].Setup(randomCard, dynamicPrice);
            shopSlots[i].gameObject.SetActive(true);
        }

        if (lifeSlot != null && lifeItem != null)
        {
            int price = CalculateDynamicPrice(lifeItem.baseCost);

            lifeSlot.Setup(lifeItem, price);
            lifeSlot.gameObject.SetActive(true);
        }
    }

    public int CalculateDynamicPrice(int baseCost)
    {
        int round = 1;
        if (RunManager.instance != null)
        {
            round = RunManager.instance.currentRound;
        }

        float inflatedPrice = baseCost + (baseCost * priceMultiplierPerRound * (round - 1));

        return Mathf.RoundToInt(inflatedPrice);
    }

    public bool ValidatePurchase(int itemCost)
    {
        int balanceAfterPurchase = RunManager.instance.playerMoney - itemCost;

        if (balanceAfterPurchase < minimumBetRequired)
        {
            return false;
        }

        return true;
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