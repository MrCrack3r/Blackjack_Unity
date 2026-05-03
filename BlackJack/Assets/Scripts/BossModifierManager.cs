using UnityEngine;
using TMPro;
using System.Collections;

public class BossModifierManager : MonoBehaviour
{
    public static BossModifierManager Instance { get; private set; }

    public enum ModifierType { None, DoubleDamage, StealPowerUp }
    public ModifierType currentModifier = ModifierType.None;

    [Header("UI Elementai")]
    public GameObject modifierPanel;
    public TextMeshProUGUI modifierText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (RunManager.instance != null && RunManager.instance.isBossRound)
        {
            ApplyRandomModifier();
        }
        else
        {
            ClearModifier();
        }
    }

    public void ApplyRandomModifier()
    {
        bool hasCards = (InventoryManager.instance != null && InventoryManager.powerUps.Count > 0);
        int randomMod;

        if (hasCards)
        {
            randomMod = Random.Range(1, 3);
        }
        else
        {
            randomMod = 1;
        }

        currentModifier = (ModifierType)randomMod;

        if (modifierPanel != null) modifierPanel.SetActive(true);

        switch (currentModifier)
        {
            case ModifierType.DoubleDamage:
                modifierText.text = "BOSS HAZARD:\nDviguba žala!";
                modifierText.color = Color.red;
                break;

            case ModifierType.StealPowerUp:
                modifierText.text = "BOSS HAZARD:\nPavogta korta!";
                modifierText.color = new Color(0.8f, 0.2f, 0.8f);
                ExecuteStealPowerUp();
                break;
        }

        StartCoroutine(HidePanelRoutine());
    }

    private IEnumerator HidePanelRoutine()
    {
        yield return new WaitForSeconds(3f);
        if (modifierPanel != null) modifierPanel.SetActive(false);
    }

    private void ExecuteStealPowerUp()
    {
        int randomIndex = Random.Range(0, InventoryManager.powerUps.Count);
        InventoryManager.instance.RemovePowerUpAt(randomIndex);
    }

    public void ClearModifier()
    {
        currentModifier = ModifierType.None;
        if (modifierPanel != null) modifierPanel.SetActive(false);
    }
}