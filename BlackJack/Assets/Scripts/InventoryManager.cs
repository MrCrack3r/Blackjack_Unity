using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Duomenų bazė")]
    public List<PowerUpData> allAvailablePowerUps;

    public static List<PowerUpData> powerUps = new List<PowerUpData>();
    public static bool hasReceivedStartCard = false;
    private int maxPowerUps = 4;

    [Header("UI Elementai ekrane")]
    public Image[] inventorySlots;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (!hasReceivedStartCard && powerUps.Count == 0)
        {
            hasReceivedStartCard = true;
            GiveRandomStartPowerUp();
        }
        UpdateInventoryUI();
    }

    public static void ClearInventory()
    {
        powerUps.Clear();
        hasReceivedStartCard = false;
    }

    public void GiveRandomStartPowerUp()
    {
        if (allAvailablePowerUps != null && allAvailablePowerUps.Count > 0)
        {
            int randomIndex = Random.Range(0, allAvailablePowerUps.Count);
            PowerUpData randomPowerUp = allAvailablePowerUps[randomIndex];
            AddPowerUp(randomPowerUp);
        }
    }

    public bool AddPowerUp(PowerUpData newPowerUp)
    {
        if (powerUps.Count >= maxPowerUps) return false;
        powerUps.Add(newPowerUp);
        UpdateInventoryUI();
        return true;
    }

    // Šią funkciją naudos BossModifierManager kortos vagystei
    public void RemovePowerUpAt(int index)
    {
        if (index >= 0 && index < powerUps.Count)
        {
            Debug.Log("Inventorius: Ištrinama korta " + powerUps[index].name + " iš indekso " + index);
            powerUps.RemoveAt(index);
            UpdateInventoryUI();
        }
    }

    public void UpdateInventoryUI()
    {
        if (inventorySlots == null) return;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null) continue;

            if (i < powerUps.Count && powerUps[i] != null && powerUps[i].icon != null)
            {
                inventorySlots[i].sprite = powerUps[i].icon;
                inventorySlots[i].gameObject.SetActive(true);
                inventorySlots[i].enabled = true;
                inventorySlots[i].color = Color.white;
            }
            else
            {
                inventorySlots[i].gameObject.SetActive(false);
            }
        }
    }
}