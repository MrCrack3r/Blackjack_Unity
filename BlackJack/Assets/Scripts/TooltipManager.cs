using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // BŪTINA naujai Input sistemai!

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    [Header("Tooltip UI Elementai")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    [Header("Langelio Pozicija")]
    // Šituos skaičius galėsi keisti per Unity! 
    // X: teigiamas skaičius stumia į dešinę. Y: neigiamas stumia į apačią.
    public Vector2 offset = new Vector2(50f, -30f);

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            // NAUDOJAME NAUJĄ INPUT SISTEMĄ PELYTĖS POZICIJAI GAUTI
            if (Mouse.current != null)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();

                // Pridedame poslinkį, kad langelis neuždengtų prekės
                tooltipPanel.transform.position = mousePos + offset;
            }
        }
    }

    public void ShowTooltip(string message)
    {
        if (tooltipPanel == null || tooltipText == null) return;

        tooltipText.text = message;
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }
}