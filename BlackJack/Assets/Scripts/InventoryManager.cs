using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // BŪTINA prirašyti, kad veiktų Image komponentai ekrane

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Duomenų bazė")]
    // Čia per Unity įmesite VISAS žaidime egzistuojančias galias
    public List<PowerUpData> allAvailablePowerUps;

    [Header("Žaidėjo Inventorius")]
    // Tai yra jūsų sąrašas, kuriame laikomos DABAR turimos galios
    public List<PowerUpData> powerUps = new List<PowerUpData>();
    private int maxPowerUps = 5;

    [Header("UI Elementai ekrane")]
    // Čia per Unity įmesite tuščius "Image" objektus iš savo Canvas
    public Image[] inventorySlots;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // ==========================================
    // TAVO ORIGINALŪS METODAI (su UI atnaujinimu)
    // ==========================================

    public bool AddPowerUp(PowerUpData newPowerUp)
    {
        if (powerUps.Count >= maxPowerUps)
        {
            Debug.Log("Inventory full!");
            return false;
        }

        powerUps.Add(newPowerUp);
        UpdateInventoryUI(); // Pridėjau UI atnaujinimą
        return true;
    }

    public void RemovePowerUp(PowerUpData powerUp)
    {
        powerUps.Remove(powerUp);
        UpdateInventoryUI(); // Pridėjau UI atnaujinimą
    }


    // ==========================================
    // UI ATNAUJINIMO METODAS
    // ==========================================

    public void UpdateInventoryUI()
    {
        // Apsauga, jei nesukūrėte jokių UI laukelių
        if (inventorySlots == null || inventorySlots.Length == 0) return;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // PAKEISTA IŠ playerInventory Į powerUps
            if (i < powerUps.Count)
            {
                // Parodome ikoną
                inventorySlots[i].sprite = powerUps[i].icon; // PASTABA: Patikrink, ar tavo PowerUpData.cs turi kintamąjį 'icon'. Jei jis vadinasi 'powerUpIcon', pakeisk čia!
                inventorySlots[i].enabled = true;
            }
            else
            {
                // Paslepiame ikoną, jei laukelis tuščias
                inventorySlots[i].sprite = null;
                inventorySlots[i].enabled = false;
            }
        }
    }
}