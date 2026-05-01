using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RunUI : MonoBehaviour
{
    public static RunUI instance { get; private set; }

    public TextMeshProUGUI roundText;
    public Image[] hearts;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateDisplay();
    }

    void Update()
    {
        if (GameManager.Instance.currentState != GameState.RoundOver)
        {
            UpdateDisplay();
        }
        UpdateHearts();
    }

    public void UpdateDisplay()
    {
        int handNum = RunManager.instance.handsSurvivedThisRound + 1;
        int handReq = RunManager.instance.handsRequiredThisRound;
        int round = RunManager.instance.currentRound;

        if (handNum > handReq)
        {
            handNum = handReq;
        }

        // --- NAUJA BOSO RAUNDO LOGIKA ---
        if (RunManager.instance.isBossRound)
        {
            // Boso raundo metu rodome "BOSS ROUND!" tekstą
            roundText.text = $"BOSS ROUND! | Hand {handNum} / {handReq}";
            roundText.color = Color.red; // Padarome tekstą raudoną
        }
        else
        {
            // Normalaus raundo tekstas
            roundText.text = $"Round {round} | Hand {handNum} / {handReq}";
            roundText.color = Color.white; // Grąžiname atgal į baltą spalvą
        }
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < RunManager.instance.playerLives;
        }
    }
}