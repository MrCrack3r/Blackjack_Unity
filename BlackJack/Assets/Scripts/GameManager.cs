using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public enum GameState
{
    Betting,
    Dealing,
    PlayerTurn,
    DealerTurn,
    RoundOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentState;

    [Header("Kortų dalinimo nustatymai")]
    public GameObject cardPrefab;
    public Transform playerHandArea;
    public Transform dealerHandArea;
    public Sprite[] testCardSprites;
    public int[] testCardValues; // Turi sutapti su sprite masyvu pagal indeksą

    [Header("UI mygtukai")]
    public Button hitButton;
    public Button standButton;
    public Button doubleButton;

    [Header("Money UI")]
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI currentBetText;

    [Header("Money Settings")]
    public int playerBudget = 200;
    public int currentBet = 20;

    private CardDisplay dealerHiddenCard;

    private int playerScore;
    private int dealerScore;

    private int playerAcesAsEleven;
    private int dealerAcesAsEleven;

    private int playerCardCount;
    private bool doubleUsed;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateMoneyUI();
        ChangeState(GameState.Betting);
    }

    private void Update()
    {
        // Testavimui:
        if (currentState == GameState.PlayerTurn && Keyboard.current.hKey.wasPressedThisFrame)
            Hit();

        if (currentState == GameState.PlayerTurn && Keyboard.current.sKey.wasPressedThisFrame)
            Stand();

        if (currentState == GameState.PlayerTurn && Keyboard.current.dKey.wasPressedThisFrame)
            Double();
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("Žaidimo būsena pasikeitė į: " + newState);

        UpdateButtons();

        switch (currentState)
        {
            case GameState.Betting:

                if (playerBudget < currentBet)
                {
                    Debug.Log("Nebėra pinigų statymui!");
                    return;
                }

                playerBudget -= currentBet;
                UpdateMoneyUI();

                ChangeState(GameState.Dealing);
                break;

            case GameState.Dealing:
                HandleDealing();
                break;

            case GameState.PlayerTurn:
                Debug.Log($"Žaidėjo ėjimas. Taškai: {playerScore}");
                break;

            case GameState.DealerTurn:
                StartCoroutine(DealerPlayRoutine());
                break;

            case GameState.RoundOver:
                HandleRoundOver();
                break;
        }
    }

    private void HandleDealing()
    {
        ClearHand(playerHandArea);
        ClearHand(dealerHandArea);

        playerScore = 0;
        dealerScore = 0;
        playerAcesAsEleven = 0;
        dealerAcesAsEleven = 0;
        playerCardCount = 0;
        doubleUsed = false;
        dealerHiddenCard = null;

        Debug.Log("Dalinamos kortos...");

        // Žaidėjui 2 kortos
        int value;
        CardDisplay player1 = SpawnCard(playerHandArea, true, out value);
        if (player1 != null)
        {
            AddCardToPlayer(value);
        }

        CardDisplay player2 = SpawnCard(playerHandArea, true, out value);
        if (player2 != null)
        {
            AddCardToPlayer(value);
        }

        // Dalintojui 2 kortos
        CardDisplay dealer1 = SpawnCard(dealerHandArea, true, out value);
        if (dealer1 != null)
        {
            AddCardToDealer(value);
        }

        dealerHiddenCard = SpawnCard(dealerHandArea, false, out value);
        AddHiddenCardToDealer(value);

        Debug.Log($"Žaidėjo taškai po dalinimo: {playerScore}");

        ChangeState(GameState.PlayerTurn);
    }

    private CardDisplay SpawnCard(Transform area, bool faceUp, out int cardValue)
    {
        cardValue = 0;

        if (testCardSprites == null || testCardSprites.Length == 0)
        {
            Debug.LogError("testCardSprites masyvas tuščias arba nepriskirtas!");
            return null;
        }

        if (testCardValues == null || testCardValues.Length != testCardSprites.Length)
        {
            Debug.LogError("testCardValues masyvas nepriskirtas arba jo ilgis nesutampa su testCardSprites!");
            return null;
        }

        GameObject newCard = Instantiate(cardPrefab, area);
        CardDisplay display = newCard.GetComponent<CardDisplay>();

        int randomIndex = Random.Range(0, testCardSprites.Length);
        Sprite randomSprite = testCardSprites[randomIndex];
        cardValue = testCardValues[randomIndex];

        display.SetupCard(randomSprite, faceUp);
        return display;
    }

    private void AddCardToPlayer(int value)
    {
        playerScore += value;
        if (value == 11) playerAcesAsEleven++;

        AdjustForAces(ref playerScore, ref playerAcesAsEleven);
        playerCardCount++;

        Debug.Log($"Žaidėjas gavo kortą už {value}. Iš viso: {playerScore}");
    }

    private void AddCardToDealer(int value)
    {
        dealerScore += value;
        if (value == 11) dealerAcesAsEleven++;

        AdjustForAces(ref dealerScore, ref dealerAcesAsEleven);

        Debug.Log($"Dalintojo vidiniai taškai dabar: {dealerScore}");
    }
    private void AddHiddenCardToDealer(int value)
    {
        dealerScore += value;
        if (value == 11) dealerAcesAsEleven++;

        AdjustForAces(ref dealerScore, ref dealerAcesAsEleven);

        Debug.Log($"Dalintojo antroji korta yra paslėpta");
    }


    private void AdjustForAces(ref int score, ref int acesAsEleven)
    {
        while (score > 21 && acesAsEleven > 0)
        {
            score -= 10; // tūzas iš 11 tampa 1
            acesAsEleven--;
        }
    }

    public void Hit()
    {
        if (currentState != GameState.PlayerTurn)
            return;

        int value;
        CardDisplay playerhit = SpawnCard(playerHandArea, true, out value);
        if (playerhit != null)
        {
            AddCardToPlayer(value);
        }

        if (playerScore > 21)
        {
            Debug.Log("Žaidėjas bust!");
            ChangeState(GameState.RoundOver);
        }
    }

    public void Stand()
    {
        if (currentState != GameState.PlayerTurn)
            return;

        Debug.Log("Žaidėjas pasirinko Stand.");
        ChangeState(GameState.DealerTurn);
    }

    public void Double()
    {
        if (currentState != GameState.PlayerTurn)
            return;

        if (playerCardCount != 2)
        {
            Debug.Log("Double galima tik su pirmomis 2 kortomis.");
            return;
        }

        currentBet *= 2;

        int value;
        CardDisplay playerdouble = SpawnCard(playerHandArea, true, out value);
        if (playerdouble != null)
        {
            AddCardToPlayer(value);
        }

        ChangeState(GameState.DealerTurn);
    }

    private IEnumerator DealerPlayRoutine()
    {
        yield return new WaitForSeconds(1f);

        if (dealerHiddenCard != null)
        {
            dealerHiddenCard.FlipCard(true);
            Debug.Log($"Dalintojas atverčia kortą. Jo taškai: {dealerScore}");
        }

        yield return new WaitForSeconds(1f);

        while (dealerScore < 17)
        {
            Debug.Log($"Dalintojas turi {dealerScore} taškų. Traukia dar vieną kortą...");

            int value;
            SpawnCard(dealerHandArea, true, out value);
            AddCardToDealer(value);

            yield return new WaitForSeconds(1.2f);
        }

        Debug.Log($"Dalintojas baigia ėjimą su {dealerScore} taškais.");
        ChangeState(GameState.RoundOver);
    }

    private void HandleRoundOver()
    {
        string result;

        if (playerScore > 21)
        {
            result = "Žaidėjas pralaimėjo (Bust)";
        }
        else if (dealerScore > 21)
        {
            result = "Žaidėjas laimėjo!";
            playerBudget += currentBet * 2;
        }
        else if (playerScore > dealerScore)
        {
            result = "Žaidėjas laimėjo!";
            playerBudget += currentBet * 2;
        }
        else if (playerScore < dealerScore)
        {
            result = "Dalintojas laimėjo.";
        }
        else
        {
            result = "Push (lygiosios)";
            playerBudget += currentBet;
        }

        Debug.Log(result);

        UpdateMoneyUI();
    }

    private void UpdateButtons()
    {
        bool playerTurn = currentState == GameState.PlayerTurn;

        if (hitButton != null)
            hitButton.interactable = playerTurn;

        if (standButton != null)
            standButton.interactable = playerTurn;

        if (doubleButton != null)
            doubleButton.interactable = playerTurn && playerCardCount == 2 && !doubleUsed;
    }

    private void ClearHand(Transform area)
    {
        for (int i = area.childCount - 1; i >= 0; i--)
        {
            Destroy(area.GetChild(i).gameObject);
        }
    }

    private void UpdateMoneyUI()
    {
        if (budgetText != null)
            budgetText.text = "$" + playerBudget;

        if (currentBetText != null)
            currentBetText.text = "$" + currentBet;
    }
}