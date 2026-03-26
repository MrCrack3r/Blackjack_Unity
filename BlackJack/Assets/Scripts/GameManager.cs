using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    private int dealerFirstCardValue;

    public static GameManager Instance;
    public GameState currentState;
    public CanvasGroup chipCanvasGroup;

    [Header("Kortų dalinimo nustatymai")]
    public GameObject cardPrefab;
    public Transform playerHandArea;
    public Transform playerSplitHandArea;
    public Transform dealerHandArea;
    public Sprite[] testCardSprites;
    public int[] testCardValues;

    [Header("UI mygtukai")]
    public Button hitButton;
    public Button standButton;
    public Button doubleButton;
    public Button splitButton;
    public Button dealButton;

    [Header("Money UI")]
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI currentBetText;

    [Header("Money Settings")]
    public int playerBudget = 200;
    public int currentBet = 0;

    [Header("Score UI")]
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI dealerScoreText;

    [Header("Raundo pabaigos UI")]
    public TextMeshProUGUI resultText;

    private CardDisplay dealerHiddenCard;

    // Dealer
    private int dealerScore;
    private int dealerAcesAsEleven;

    // Pirma ranka
    private int playerScore;
    private int playerAcesAsEleven;
    private int playerCardCount;

    // Antra ranka po split
    private int splitPlayerScore;
    private int splitPlayerAcesAsEleven;
    private int splitPlayerCardCount;

    // Split būsena
    private bool hasSplit = false;
    private int activeHandIndex = 0; // 0 = pirma ranka, 1 = antra ranka

    // Double
    private bool doubleUsed;

    // Statymai per rankas
    private int firstHandBet;
    private int secondHandBet;

    // Pirmos dvi žaidėjo kortos split patikrai
    private CardDisplay firstPlayerCardDisplay;
    private CardDisplay secondPlayerCardDisplay;
    private int firstPlayerCardValue;
    private int secondPlayerCardValue;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (resultText != null) resultText.gameObject.SetActive(false);

        firstHandBet = 0;
        secondHandBet = 0;

        UpdateMoneyUI();
        ChangeState(GameState.Betting);
    }

    private void Update()
    {
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
                if (resultText != null) resultText.gameObject.SetActive(false);

                firstHandBet = 0;
                secondHandBet = 0;

                Debug.Log("Laukiama Deal mygtuko...");

                break;

            case GameState.Dealing:
                StartCoroutine(HandleDealing());
                break;

            case GameState.PlayerTurn:
                Debug.Log($"Žaidėjo ėjimas. Aktyvi ranka: {activeHandIndex + 1}");
                UpdateScoreUI();
                break;

            case GameState.DealerTurn:
                StartCoroutine(DealerPlayRoutine());
                break;

            case GameState.RoundOver:
                HandleRoundOver();
                break;
        }

    }

    public void OnDealButton()
    {
        if (currentState != GameState.Betting)
        {
            Debug.Log("Dabar negalima dealinti!");
            return;
        }

        if (currentBet <= 0)
        {
            Debug.Log("Pasirink statymą prieš žaidimą!");
            return;
        }

        if (playerBudget < currentBet)
        {
            Debug.Log("Nepakanka pinigų!");
            return;
        }

        playerBudget -= currentBet;
        firstHandBet = currentBet;
        secondHandBet = 0;

        UpdateMoneyUI();

        ChangeState(GameState.Dealing);
    }


    private IEnumerator HandleDealing()
    {
        ClearHand(playerHandArea);
        ClearHand(dealerHandArea);

        if (playerSplitHandArea != null)
            ClearHand(playerSplitHandArea);

        // Reset
        playerScore = 0;
        dealerScore = 0;
        splitPlayerScore = 0;

        playerAcesAsEleven = 0;
        dealerAcesAsEleven = 0;
        splitPlayerAcesAsEleven = 0;

        playerCardCount = 0;
        splitPlayerCardCount = 0;

        doubleUsed = false;
        hasSplit = false;
        activeHandIndex = 0;

        dealerHiddenCard = null;
        dealerFirstCardValue = 0;

        firstPlayerCardDisplay = null;
        secondPlayerCardDisplay = null;
        firstPlayerCardValue = 0;
        secondPlayerCardValue = 0;

        Debug.Log("Dalinamos kortos...");

        int value;

        firstPlayerCardDisplay = SpawnCard(playerHandArea, true, out value);
        if (firstPlayerCardDisplay != null)
        {
            firstPlayerCardValue = value;
            AddCardToHand(0, value);
        }
        yield return new WaitForSeconds(0.8f);

        CardDisplay dealer1 = SpawnCard(dealerHandArea, true, out value);
        if (dealer1 != null)
        {
            dealerFirstCardValue = value;
            AddCardToDealer(value);
        }
        yield return new WaitForSeconds(0.8f);

        secondPlayerCardDisplay = SpawnCard(playerHandArea, true, out value);
        if (secondPlayerCardDisplay != null)
        {
            secondPlayerCardValue = value;
            AddCardToHand(0, value);
        }
        yield return new WaitForSeconds(0.8f);

        dealerHiddenCard = SpawnCard(dealerHandArea, false, out value, playSound: true);
        if (dealerHiddenCard != null)
            AddHiddenCardToDealer(value);

        yield return new WaitForSeconds(0.8f);

        Debug.Log($"Žaidėjo taškai po dalinimo: {playerScore}");
        UpdateScoreUI();
        ChangeState(GameState.PlayerTurn);
    }

    private CardDisplay SpawnCard(Transform area, bool faceUp, out int cardValue, bool playSound = true)
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

        if (playSound && AudioManager.Instance != null)
        {
            if (area == playerHandArea || area == playerSplitHandArea)
                AudioManager.Instance.PlayPlayerCardSound();
            else if (area == dealerHandArea)
                AudioManager.Instance.PlayDealerCardSound();
        }

        return display;
    }

    private void AddCardToHand(int handIndex, int value)
    {
        if (handIndex == 0)
        {
            playerScore += value;
            if (value == 11) playerAcesAsEleven++;
            AdjustForAces(ref playerScore, ref playerAcesAsEleven);
            playerCardCount++;
            Debug.Log($"Žaidėjo 1 ranka gavo kortą už {value}. Iš viso: {playerScore}");
        }
        else
        {
            splitPlayerScore += value;
            if (value == 11) splitPlayerAcesAsEleven++;
            AdjustForAces(ref splitPlayerScore, ref splitPlayerAcesAsEleven);
            splitPlayerCardCount++;
            Debug.Log($"Žaidėjo 2 ranka gavo kortą už {value}. Iš viso: {splitPlayerScore}");
        }

        UpdateScoreUI();
    }

    private void AddCardToDealer(int value)
    {
        dealerScore += value;
        if (value == 11) dealerAcesAsEleven++;
        AdjustForAces(ref dealerScore, ref dealerAcesAsEleven);
        Debug.Log($"Dalintojo vidiniai taškai dabar: {dealerScore}");
        UpdateScoreUI();
    }

    private void AddHiddenCardToDealer(int value)
    {
        dealerScore += value;
        if (value == 11) dealerAcesAsEleven++;
        AdjustForAces(ref dealerScore, ref dealerAcesAsEleven);
        Debug.Log("Dalintojo antroji korta yra paslėpta");
    }

    private void AdjustForAces(ref int score, ref int acesAsEleven)
    {
        while (score > 21 && acesAsEleven > 0)
        {
            score -= 10;
            acesAsEleven--;
        }
    }

    public void AddBet(int amount)
    {
        if (currentState != GameState.Betting)
        {
            Debug.Log("Negalima statyti dabar!");
            return;
        }

        if (playerBudget < amount)
        {
            Debug.Log("Nepakanka pinigų!");
            return;
        }

        currentBet += amount;

        UpdateMoneyUI();
    }

    public void Hit()
    {
        if (currentState != GameState.PlayerTurn) return;

        int value;

        if (activeHandIndex == 0)
        {
            CardDisplay playerhit = SpawnCard(playerHandArea, true, out value);
            if (playerhit != null) AddCardToHand(0, value);

            if (playerScore > 21)
            {
                Debug.Log("Žaidėjo 1 ranka bust!");

                if (hasSplit)
                {
                    activeHandIndex = 1;
                    Debug.Log("Pereinama prie 2 rankos.");
                    UpdateButtons();
                    UpdateScoreUI();
                }
                else
                {
                    ChangeState(GameState.RoundOver);
                }
            }
        }
        else
        {
            CardDisplay splitHit = SpawnCard(playerSplitHandArea, true, out value);
            if (splitHit != null) AddCardToHand(1, value);

            if (splitPlayerScore > 21)
            {
                Debug.Log("Žaidėjo 2 ranka bust!");

                if (playerScore > 21)
                    ChangeState(GameState.RoundOver);
                else
                    ChangeState(GameState.DealerTurn);
            }
        }
    }

    public void Stand()
    {
        if (currentState != GameState.PlayerTurn) return;

        if (hasSplit && activeHandIndex == 0)
        {
            activeHandIndex = 1;
            Debug.Log("Baigta 1 ranka. Pereinama prie 2 rankos.");
            UpdateButtons();
            UpdateScoreUI();
            return;
        }

        Debug.Log("Žaidėjas pasirinko Stand.");
        ChangeState(GameState.DealerTurn);
    }

    public void Double()
    {
        if (currentState != GameState.PlayerTurn) return;

        // Paprastumo dėlei double po split neleistas
        if (hasSplit)
        {
            Debug.Log("Double po split šioje versijoje negalimas.");
            return;
        }

        if (playerCardCount != 2)
        {
            Debug.Log("Double galima tik su pirmomis 2 kortomis.");
            return;
        }

        if (playerBudget < firstHandBet)
        {
            Debug.Log("Nepakanka biudžeto double statymui.");
            return;
        }

        playerBudget -= firstHandBet;
        firstHandBet *= 2;
        doubleUsed = true;
        UpdateMoneyUI();

        int value;
        CardDisplay playerdouble = SpawnCard(playerHandArea, true, out value);
        if (playerdouble != null) AddCardToHand(0, value);

        if (playerScore > 21)
            ChangeState(GameState.RoundOver);
        else
            ChangeState(GameState.DealerTurn);
    }

    public void Split()
    {
        if (currentState != GameState.PlayerTurn) return;

        if (hasSplit)
        {
            Debug.Log("Split jau buvo panaudotas.");
            return;
        }

        if (playerSplitHandArea == null)
        {
            Debug.LogError("playerSplitHandArea nėra priskirtas!");
            return;
        }

        if (playerCardCount != 2)
        {
            Debug.Log("Split galima tik su pirmomis 2 kortomis.");
            return;
        }

        if (firstPlayerCardValue != secondPlayerCardValue)
        {
            Debug.Log("Split galima tik kai abi pirmos kortos vienodos vertės.");
            return;
        }

        if (playerBudget < currentBet)
        {
            Debug.Log("Nepakanka biudžeto split statymui.");
            return;
        }

        hasSplit = true;
        activeHandIndex = 0;
        secondHandBet = currentBet;
        playerBudget -= secondHandBet;
        UpdateMoneyUI();

        // Perkeliam antrą kortą į split rankos zoną
        if (secondPlayerCardDisplay != null)
            secondPlayerCardDisplay.transform.SetParent(playerSplitHandArea, false);

        // Perskaičiuojam abi rankas nuo nulio
        playerScore = 0;
        playerAcesAsEleven = 0;
        playerCardCount = 0;

        splitPlayerScore = 0;
        splitPlayerAcesAsEleven = 0;
        splitPlayerCardCount = 0;

        AddCardToHand(0, firstPlayerCardValue);
        AddCardToHand(1, secondPlayerCardValue);

        // Po vieną naują kortą kiekvienai rankai
        int value;

        CardDisplay firstHandNewCard = SpawnCard(playerHandArea, true, out value);
        if (firstHandNewCard != null) AddCardToHand(0, value);

        CardDisplay secondHandNewCard = SpawnCard(playerSplitHandArea, true, out value);
        if (secondHandNewCard != null) AddCardToHand(1, value);

        Debug.Log("Split atliktas.");
        UpdateButtons();
        UpdateScoreUI();
    }

    private IEnumerator DealerPlayRoutine()
    {
        // Jei abi rankos bust po split, dealerio ėjimo nereikia
        if (hasSplit && playerScore > 21 && splitPlayerScore > 21)
        {
            ChangeState(GameState.RoundOver);
            yield break;
        }

        // Jei viena ranka be split bust, dealerio nereikia
        if (!hasSplit && playerScore > 21)
        {
            ChangeState(GameState.RoundOver);
            yield break;
        }

        yield return new WaitForSeconds(1.5f);

        if (dealerHiddenCard != null)
        {
            dealerHiddenCard.FlipCard(true);
            UpdateScoreUI();
            Debug.Log($"Dalintojas atverčia kortą. Jo taškai: {dealerScore}");
        }

        yield return new WaitForSeconds(1.5f);

        while (dealerScore < 17)
        {
            Debug.Log($"Dalintojas turi {dealerScore} taškų. Traukia dar vieną kortą...");
            int value;
            SpawnCard(dealerHandArea, true, out value, playSound: true);
            AddCardToDealer(value);
            yield return new WaitForSeconds(1.5f);
        }

        Debug.Log($"Dalintojas baigia ėjimą su {dealerScore} taškais.");
        ChangeState(GameState.RoundOver);
    }

    private void HandleRoundOver()
    {
        string result = "";

        if (!hasSplit)
        {
            if (playerScore > 21)
            {
                result = "Player lost (Bust)";
            }
            else if (dealerScore > 21)
            {
                result = "Player won!";
                playerBudget += firstHandBet * 2;
            }
            else if (playerScore > dealerScore)
            {
                result = "Player won!";
                playerBudget += firstHandBet * 2;
            }
            else if (playerScore < dealerScore)
            {
                result = "Dealer won.";
            }
            else
            {
                result = "Push";
                playerBudget += firstHandBet;
            }
        }
        else
        {
            string hand1Result = ResolveSingleHand(playerScore, firstHandBet, "Hand 1");
            string hand2Result = ResolveSingleHand(splitPlayerScore, secondHandBet, "Hand 2");
            result = hand1Result + "\n" + hand2Result;
        }

        Debug.Log(result);
        UpdateMoneyUI();

        if (resultText != null)
        {
            resultText.text = result;
            resultText.gameObject.SetActive(true);
        }

        if (playerBudget <= 0)
        {
            Debug.Log("Pinigai baigėsi! Žaidimas baigtas.");
            StartCoroutine(GameOverSequence());
        }
        else
        {
            Debug.Log("Pinigų dar yra. Laukiamas naujas raundas...");
        }

        currentBet = 0;
        UpdateMoneyUI();
    }

    private string ResolveSingleHand(int handScore, int handBet, string handName)
    {
        if (handScore > 21)
        {
            return handName + ": Lost (Bust)";
        }
        else if (dealerScore > 21)
        {
            playerBudget += handBet * 2;
            return handName + ": Won";
        }
        else if (handScore > dealerScore)
        {
            playerBudget += handBet * 2;
            return handName + ": Won";
        }
        else if (handScore < dealerScore)
        {
            return handName + ": Lost";
        }
        else
        {
            playerBudget += handBet;
            return handName + ": Push";
        }
    }

    private void UpdateButtons()
    {
        bool playerTurn = currentState == GameState.PlayerTurn;

        if (hitButton != null)
            hitButton.interactable = playerTurn;

        if (standButton != null)
            standButton.interactable = playerTurn;

        if (doubleButton != null)
            doubleButton.interactable = playerTurn && !hasSplit && activeHandIndex == 0 && playerCardCount == 2 && !doubleUsed;

        if (splitButton != null)
            splitButton.interactable = playerTurn &&
                                      !hasSplit &&
                                      playerCardCount == 2 &&
                                      firstPlayerCardValue == secondPlayerCardValue &&
                                      playerBudget >= currentBet;
        if (dealButton != null)
            dealButton.interactable = currentState == GameState.Betting;

        if (chipCanvasGroup != null)
        {
            bool isBetting = currentState == GameState.Betting;

            chipCanvasGroup.alpha = isBetting ? 1f : 0.4f;
            chipCanvasGroup.interactable = isBetting;
            chipCanvasGroup.blocksRaycasts = isBetting;
        }

    }

    private void ClearHand(Transform area)
    {
        if (area == null) return;

        for (int i = area.childCount - 1; i >= 0; i--)
            Destroy(area.GetChild(i).gameObject);
    }

    private void UpdateMoneyUI()
    {
        if (budgetText != null)
            budgetText.text = "$" + playerBudget;

        if (currentBetText != null)
        {
            int shownBet = firstHandBet + secondHandBet;
            if (shownBet <= 0) shownBet = currentBet;
            currentBetText.text = "$" + shownBet;
        }
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("End_screen");
    }

    private void UpdateScoreUI()
    {
        if (playerScoreText != null)
        {
            if (!hasSplit)
            {
                playerScoreText.text = "Points: " + playerScore;
            }
            else
            {
                string activeText = activeHandIndex == 0 ? " (Hand 1)" : " (Hand 2)";
                playerScoreText.text = $"H1: {playerScore} | H2: {splitPlayerScore}{activeText}";
            }
        }

        if (dealerScoreText != null)
        {
            if (currentState == GameState.Dealing || currentState == GameState.PlayerTurn)
                dealerScoreText.text = "Points: " + dealerFirstCardValue.ToString();
            else
                dealerScoreText.text = "Points: " + dealerScore.ToString();
        }
    }
}