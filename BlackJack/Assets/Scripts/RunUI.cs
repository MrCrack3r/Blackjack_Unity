using UnityEngine;
using TMPro;

public class RunUI : MonoBehaviour
{
    public static RunUI instance { get; private set; }

    public TextMeshProUGUI roundText;
    public TextMeshProUGUI livesText;

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

        if (RunManager.instance.isBossRound)
        {
            roundText.text = $"BOSS ROUND! | Hand {handNum} / {handReq}";
            roundText.color = Color.red;
        }
        else
        {
            roundText.text = $"Round {round} | Hand {handNum} / {handReq}";
            roundText.color = Color.white;
        }
    }

    private void UpdateHearts()
    {
        if (livesText != null)
        {
            livesText.text = "x " + RunManager.instance.playerLives.ToString();
        }
    }
}