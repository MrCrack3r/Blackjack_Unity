using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Duomenų bazė")]
    public List<PowerUpData> allAvailablePowerUps;

    public static List<PowerUpData> powerUps = new List<PowerUpData>();
    public static bool hasReceivedStartCard = false;
    public static bool isRunStarted = false; // Saugiklis, kad kortos neužkrautų kiekvieną raundą

    private int maxPowerUps = 4;

    [Header("UI Elementai ekrane")]
    public Image[] inventorySlots;

    [Header("Tooltip Nustatymai")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);

        // Tikriname, ar tai visiškai naujas žaidimo startas / užkrovimas
        if (!isRunStarted)
        {
            isRunStarted = true; // Užfiksuojame, kad žaidimas jau prasidėjo

            if (PlayerPrefs.HasKey("HasStartCard"))
            {
                hasReceivedStartCard = PlayerPrefs.GetInt("HasStartCard", 0) == 1;
                string savedInv = PlayerPrefs.GetString("SavedInventory", "");

                if (!string.IsNullOrEmpty(savedInv))
                {
                    string[] cardNames = savedInv.Split(',');
                    foreach (string cName in cardNames)
                    {
                        foreach (PowerUpData data in allAvailablePowerUps)
                        {
                            if (data.name == cName)
                            {
                                powerUps.Add(data);
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Pradinę kortą duodame TIK jei tai naujas žaidimas ir jos dar negavome
        if (!hasReceivedStartCard && powerUps.Count == 0)
        {
            hasReceivedStartCard = true;
            GiveRandomStartPowerUp();
        }

        UpdateInventoryUI();
    }

    void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            if (Mouse.current != null)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();

                RectTransform rect = tooltipPanel.GetComponent<RectTransform>();

                float pivotX = mousePos.x / Screen.width > 0.5f ? 1f : 0f;
                float pivotY = mousePos.y / Screen.height > 0.5f ? 1f : 0f;

                rect.pivot = new Vector2(pivotX, pivotY);

                float offsetX = pivotX == 0 ? 15f : -15f;
                float offsetY = pivotY == 1 ? -15f : 15f;

                tooltipPanel.transform.position = new Vector3(mousePos.x + offsetX, mousePos.y + offsetY, 0f);
            }
        }
    }

    public static void ClearInventory()
    {
        powerUps.Clear();
        hasReceivedStartCard = false;
        isRunStarted = false; 
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

    public void RemovePowerUpAt(int index)
    {
        if (index >= 0 && index < powerUps.Count)
        {
            powerUps.RemoveAt(index);
            HideTooltip();
            UpdateInventoryUI();
        }
    }

    public void UpdateInventoryUI()
    {
        if (inventorySlots == null) return;

        HideTooltip();

        bool canUse = (GameManager.Instance != null && GameManager.Instance.currentState == GameState.PlayerTurn);

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null) continue;

            if (i < powerUps.Count && powerUps[i] != null && powerUps[i].icon != null)
            {
                inventorySlots[i].sprite = powerUps[i].icon;
                inventorySlots[i].gameObject.SetActive(true);
                inventorySlots[i].enabled = true;

                inventorySlots[i].color = canUse ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f);
            }
            else
            {
                inventorySlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowTooltip(int index)
    {
        if (index >= 0 && index < powerUps.Count && tooltipPanel != null && tooltipText != null)
        {
            tooltipText.text = powerUps[index].powerUpName + "\n\n" + powerUps[index].description;
            tooltipPanel.SetActive(true);
        }
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }
}