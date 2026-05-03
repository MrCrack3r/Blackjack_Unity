using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Prekės Nustatymai")]
    public PowerUpData itemData;
    private int currentDynamicPrice;


    [Header("Prekės UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Image iconImage;


    public void Setup(PowerUpData newData, int dynamicPrice)
    {
        itemData = newData;
        currentDynamicPrice = dynamicPrice; 
        if (itemData != null)
        {
            if (nameText != null) nameText.text = itemData.powerUpName;
            if (priceText != null) priceText.text = "$" + currentDynamicPrice;
            if (iconImage != null) iconImage.sprite = itemData.icon;
        }
    }

    public void BuyItem()
    {
        if (itemData == null) return;

        if (ShopManager.instance != null && !ShopManager.instance.ValidatePurchase(currentDynamicPrice))
        {
            int minBet = ShopManager.instance.minimumBetRequired;
            ShopManager.instance.ShowNotification($"Nepakanka lėšų! Turi likti bent ${minBet} sekanciam raundui.", Color.red);
            return;
        }

        if (itemData.isLifeItem)
        {
            if (RunManager.instance.playerLives >= 3)
            {
                ShopManager.instance.ShowNotification("Gyvybės pilnos!", Color.yellow);
                return;
            }

            RunManager.instance.playerMoney -= currentDynamicPrice;
            RunManager.instance.playerLives++;

            if (RunUI.instance != null)
                RunUI.instance.UpdateDisplay();

            ShopManager.instance.ShowNotification("+1 Life!", Color.green);

            gameObject.SetActive(false);
            return;
        }


        if (InventoryManager.powerUps.Count < 4)
        {
            InventoryManager.powerUps.Add(itemData);
            RunManager.instance.playerMoney -= currentDynamicPrice;

            if (AudioManager.Instance != null) AudioManager.Instance.PlayCoinSound();

            if (ShopManager.instance != null)
                ShopManager.instance.ShowNotification("Purchased: " + itemData.powerUpName, Color.green);

            if (InventoryManager.instance != null)
                InventoryManager.instance.UpdateInventoryUI();

            if (TooltipManager.instance != null) TooltipManager.instance.HideTooltip();

            gameObject.SetActive(false);
        }
        else
        {
            if (ShopManager.instance != null)
                ShopManager.instance.ShowNotification("Inventory Full!", Color.yellow);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null && TooltipManager.instance != null)
        {
            TooltipManager.instance.ShowTooltip(itemData.description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.instance != null)
        {
            TooltipManager.instance.HideTooltip();
        }
    }
}