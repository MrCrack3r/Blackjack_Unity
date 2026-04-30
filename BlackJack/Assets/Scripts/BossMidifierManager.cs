using UnityEngine;
using TMPro;

public class BossModifierManager : MonoBehaviour
{
    public static BossModifierManager Instance { get; private set; }

    // Palikome tik du efektus
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
        ClearModifier();
    }

    public void ApplyRandomModifier()
    {
        // Random.Range(1, 3) grąžins tik 1 arba 2.
        int randomMod = Random.Range(1, 3);
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
                modifierText.color = new Color(0.8f, 0.2f, 0.8f); // Violetinė spalva

                // IŠKART įvykdome kortos atėmimą
                ExecuteStealPowerUp();
                break;
        }
    }

    private void ExecuteStealPowerUp()
    {
        // Naudojame InventoryManager.powerUps vietoje InventoryManager.instance.powerUps
        if (InventoryManager.instance != null && InventoryManager.powerUps.Count > 0)
        {
            // Išrenkame atsitiktinės kortos indeksą
            int randomIndex = Random.Range(0, InventoryManager.powerUps.Count);

            // Pašaliname tą kortą iš sąrašo per instance (nes funkcija nėra static)
            InventoryManager.instance.RemovePowerUpAt(randomIndex);

            Debug.Log("Bosas pavogė kortą indeksu: " + randomIndex);
        }
        else
        {
            // Jei žaidėjas neturėjo jokių kortų
            modifierText.text = "BOSS HAZARD:\nNepavyko pavogti kortos (tuščias inventorius)!";
            Debug.Log("Žaidėjas neturėjo kortų, bosas neturėjo ko vogti.");
        }
    }

    public void ClearModifier()
    {
        currentModifier = ModifierType.None;
        if (modifierPanel != null) modifierPanel.SetActive(false);
    }
}