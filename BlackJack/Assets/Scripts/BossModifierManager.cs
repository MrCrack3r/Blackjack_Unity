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
        // Kai tik užsikrauna Blackjack stalo scena, patikriname:
        // Ar RunManager egzistuoja ir ar dabar yra boso raundas?
        if (RunManager.instance != null && RunManager.instance.isBossRound)
        {
            ApplyRandomModifier(); // Jei taip, iškart parodome boso UI ir pritaikome efektą
        }
        else
        {
            ClearModifier(); // Jei ne, paslepiame UI
        }
    }

    public void ApplyRandomModifier()
    {
        Debug.Log("UI ATNAUJINIMAS: Bandoma rodyti Boso skydelį!");

        // Pirmiausia patikriname, ar žaidėjas apskritai turi Power-up kortų
        bool hasCards = (InventoryManager.instance != null && InventoryManager.powerUps.Count > 0);

        int randomMod;

        if (hasCards)
        {
            // Jei žaidėjas TURI kortų, bosas gali rinktis iš abiejų atakų:
            // 1 (DoubleDamage) arba 2 (StealPowerUp)
            randomMod = Random.Range(1, 3);
        }
        else
        {
            // Jei žaidėjas NETURI kortų, kortos vagystės efektas nebegalimas.
            // Priverstinai parenkame 1 (DoubleDamage).
            randomMod = 1;
            Debug.Log("Žaidėjas neturi kortų, todėl bosas priverstinai naudoja Dvigubą Žalą!");
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
                modifierText.color = new Color(0.8f, 0.2f, 0.8f); // Violetinė spalva

                ExecuteStealPowerUp();
                break;
        }
    }

    private void ExecuteStealPowerUp()
    {
        // Kadangi šią funkciją iškviesime tik žinodami, kad kortų tikrai yra, 
        // galime iškart vogti atsitiktinę kortą.
        int randomIndex = Random.Range(0, InventoryManager.powerUps.Count);

        InventoryManager.instance.RemovePowerUpAt(randomIndex);

        Debug.Log("Bosas pavogė kortą indeksu: " + randomIndex);
    }

    public void ClearModifier()
    {
        currentModifier = ModifierType.None;
        if (modifierPanel != null) modifierPanel.SetActive(false);
    }
}