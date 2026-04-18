using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [Header("Prekės Nustatymai")]
    public PowerUpData itemData; // Jūsų sukurtas ScriptableObject

    [Header("Prekės UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI priceText;
    public Image iconImage;

    void Start()
    {
        // Užpildome UI pagal priskirtą PowerUpData failą
        if (itemData != null)
        {
            if (nameText != null) nameText.text = itemData.powerUpName;
            if (descText != null) descText.text = itemData.description;
            if (priceText != null) priceText.text = "$" + itemData.baseCost;

            // PATAISYMAS 1: Naudojame tavo kintamąjį "icon" (vietoj powerUpIcon)
            if (iconImage != null) iconImage.sprite = itemData.icon;
        }
    }

    public void BuyItem()
    {
        if (itemData == null) return;

        // 1. Patikriname pinigus
        if (RunManager.instance.playerMoney < itemData.baseCost)
        {
            Debug.Log("Nepakanka pinigų!");
            if (ShopManager.instance != null)
                ShopManager.instance.ShowNotification("Nepakanka pinigų!", Color.red);
            return;
        }

        // 2. Patikriname inventorių
        if (InventoryManager.instance != null)
        {
            bool success = InventoryManager.instance.AddPowerUp(itemData);

            if (success)
            {
                // 3. Nuskaičiuojame pinigus
                RunManager.instance.playerMoney -= itemData.baseCost;

                // PATAISYMAS 2: Ištryniau RunManager.instance.UpdateUI(), nes tokios funkcijos ten nebėra.
                // (Jei turi MoneyUI skriptą, pinigų atnaujinimą ekrane turėsi padaryti ten)

                if (ShopManager.instance != null)
                    ShopManager.instance.ShowNotification("Sėkmingai nupirkta!", Color.green);

                // 4. Paslepiame šią prekę, nes ji jau nupirkta
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Inventorius pilnas!");
                if (ShopManager.instance != null)
                    ShopManager.instance.ShowNotification("Inventorius pilnas!", Color.yellow);
            }
        }
        else
        {
            Debug.LogError("Nerastas InventoryManager!");
        }
    }
}