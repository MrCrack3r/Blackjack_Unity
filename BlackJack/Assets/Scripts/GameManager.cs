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
    public Button goToShopButton;
    public Button allInButton;
    public Button clearBetButton;

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

    [Header("Deck pozicija")]
    public Transform deckPosition;
    public Canvas rootCanvas;

    [Header("Efektai")]
    public EndEffects endEffects;

    [Header("Bet Slider")]
    public Slider betSlider;
    public TextMeshProUGUI betSliderText;

    private CardDisplay dealerHiddenCard;

    private int dealerScore;
    private int dealerAcesAsEleven;

    private int playerScore;
    private int playerAcesAsEleven;
    private int playerCardCount;

    private int splitPlayerScore;
    private int splitPlayerAcesAsEleven;
    private int splitPlayerCardCount;

    private bool hasSplit = false;
    private int activeHandIndex = 0;

    private bool doubleUsed;

    private int firstHandBet;
    private int secondHandBet;

    private CardDisplay firstPlayerCardDisplay;
    private CardDisplay secondPlayerCardDisplay;
    private int firstPlayerCardValue;
    private int secondPlayerCardValue;
    private bool forceWin = false;
    private bool forceSkip = false;
    private bool handshakeActive = false;
    private bool isUpdatingSlider = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (resultText != null) resultText.gameObject.SetActive(false);

        if (goToShopButton != null) goToShopButton.gameObject.SetActive(false);

        if (betSlider != null)
        {
            betSlider.onValueChanged.AddListener(OnSliderChanged);
        }

        firstHandBet = 0;
        secondHandBet = 0;

        UpdateMoneyUI();
        UpdateScoreUI();
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

        HandleKeyboardInput();
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

                UpdateSliderLimits();

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
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSound();

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

        firstHandBet = currentBet;
        secondHandBet = 0;

        UpdateMoneyUI();

        ChangeState(GameState.Dealing);
    }

    private IEnumerator HandleDealing()
    {
        RectTransform mainHandRect = playerHandArea.GetComponent<RectTransform>();
        if (mainHandRect != null)
        {
            mainHandRect.anchoredPosition = new Vector2(0f, mainHandRect.anchoredPosition.y);
        }

        ClearHand(playerHandArea);
        ClearHand(dealerHandArea);

        if (playerSplitHandArea != null)
            ClearHand(playerSplitHandArea);

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

        int value = 0;
        CardDisplay card = null;

        yield return StartCoroutine(SpawnCardAnimated(playerHandArea, true, true, (v, c) => { value = v; card = c; }));
        if (card != null) { firstPlayerCardDisplay = card; firstPlayerCardValue = value; AddCardToHand(0, value); }

        yield return StartCoroutine(SpawnCardAnimated(dealerHandArea, true, true, (v, c) => { value = v; card = c; }));
        if (card != null) { dealerFirstCardValue = value; AddCardToDealer(value); }

        yield return StartCoroutine(SpawnCardAnimated(playerHandArea, true, true, (v, c) => { value = v; card = c; }));
        if (card != null) { secondPlayerCardDisplay = card; secondPlayerCardValue = value; AddCardToHand(0, value); }

        yield return StartCoroutine(SpawnCardAnimated(dealerHandArea, false, true, (v, c) => { value = v; card = c; }));
        if (card != null) { dealerHiddenCard = card; AddHiddenCardToDealer(value); }

        Debug.Log($"Žaidėjo taškai po dalinimo: {playerScore}");
        UpdateScoreUI();

        if (playerScore == 21 && playerCardCount == 2)
        {
            ChangeState(GameState.DealerTurn);
            yield break;
        }

        ChangeState(GameState.PlayerTurn);
    }

    private IEnumerator SpawnCardAnimated(Transform area, bool faceUp, bool playSound, System.Action<int, CardDisplay> onDone)
    {
        if (testCardSprites == null || testCardSprites.Length == 0) { onDone?.Invoke(0, null); yield break; }
        if (testCardValues == null || testCardValues.Length != testCardSprites.Length) { onDone?.Invoke(0, null); yield break; }

        GameObject newCard = Instantiate(cardPrefab, area);
        CardDisplay display = newCard.GetComponent<CardDisplay>();

        int randomIndex = Random.Range(0, testCardSprites.Length);
        Sprite randomSprite = testCardSprites[randomIndex];
        int cardValue = testCardValues[randomIndex];

        display.SetupCard(randomSprite, false);

        RunManager.instance.cardsUsedThisRun++;

        yield return null;

        Vector3 startPos = deckPosition != null ? deckPosition.position : area.position;

        bool done = false;
        display.PlayDealAnimation(startPos, faceUp, rootCanvas, () =>
        {
            if (playSound && AudioManager.Instance != null)
            {
                if (area == playerHandArea || area == playerSplitHandArea)
                    AudioManager.Instance.PlayPlayerCardSound();
                else if (area == dealerHandArea)
                    AudioManager.Instance.PlayDealerCardSound();
            }
            done = true;
        });

        yield return new WaitUntil(() => done);
        onDone?.Invoke(cardValue, display);
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

        RunManager.instance.cardsUsedThisRun++;

        if (playSound && AudioManager.Instance != null)
        {
            if (area == playerHandArea || area == playerSplitHandArea)
                AudioManager.Instance.PlayPlayerCardSound();
            else if (area == dealerHandArea)
                AudioManager.Instance.PlayDealerCardSound();
        }

        return display;
    }

    public void AddCardToHand(int handIndex, int value)
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
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCoinSound();

        if (currentState != GameState.Betting)
        {
            Debug.Log("Negalima statyti dabar!");
            return;
        }

        if (RunManager.instance.playerMoney < amount)
        {
            Debug.Log("Nepakanka pinigų!");
            return;
        }

        currentBet += amount;
        RunManager.instance.playerMoney -= amount;

        UpdateMoneyUI();
        UpdateButtons();

        UpdateSliderLimits();
    }



    public void Hit()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSound();

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
            else if (playerScore == 21)
            {
                Debug.Log("Žaidėjo 1 ranka surinko 21.");

                if (hasSplit)
                {
                    activeHandIndex = 1;
                    Debug.Log("Pereinama prie 2 rankos.");
                    UpdateButtons();
                    UpdateScoreUI();
                }
                else
                {
                    ChangeState(GameState.DealerTurn);
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
            else if (splitPlayerScore == 21)
            {
                Debug.Log("Žaidėjo 2 ranka surinko 21.");
                ChangeState(GameState.DealerTurn);
            }
        }
    }

    public void Stand()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSound();

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
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSound();

        if (currentState != GameState.PlayerTurn) return;

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

        if (RunManager.instance.playerMoney < firstHandBet)
        {
            Debug.Log("Nepakanka biudžeto double statymui.");
            return;
        }

        RunManager.instance.playerMoney -= firstHandBet;
        firstHandBet *= 2;
        doubleUsed = true;
        UpdateMoneyUI();

        int value;
        CardDisplay playerdouble = SpawnCard(playerHandArea, true, out value);
        if (playerdouble != null) AddCardToHand(0, value);

        if (playerScore > 21)
        {
            Debug.Log("Žaidėjas bust po double!");
            ChangeState(GameState.RoundOver);
        }
        else if (playerScore == 21)
        {
            Debug.Log("Žaidėjas surinko 21 po double.");
            ChangeState(GameState.DealerTurn);
        }
        else
        {
            ChangeState(GameState.DealerTurn);
        }
    }

    public void Split()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSound();

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

        if (RunManager.instance.playerMoney < currentBet)
        {
            Debug.Log("Nepakanka biudžeto split statymui.");
            return;
        }

        RectTransform mainHandRect = playerHandArea.GetComponent<RectTransform>();
        if (mainHandRect != null)
        {
            mainHandRect.anchoredPosition = new Vector2(-246f, mainHandRect.anchoredPosition.y);
        }

        hasSplit = true;
        activeHandIndex = 0;
        secondHandBet = currentBet;
        RunManager.instance.playerMoney -= secondHandBet;
        UpdateMoneyUI();

        if (secondPlayerCardDisplay != null)
            secondPlayerCardDisplay.transform.SetParent(playerSplitHandArea, false);

        playerScore = 0;
        playerAcesAsEleven = 0;
        playerCardCount = 0;

        splitPlayerScore = 0;
        splitPlayerAcesAsEleven = 0;
        splitPlayerCardCount = 0;

        AddCardToHand(0, firstPlayerCardValue);
        AddCardToHand(1, secondPlayerCardValue);

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
        if (hasSplit && playerScore > 21 && splitPlayerScore > 21)
        {
            ChangeState(GameState.RoundOver);
            yield break;
        }

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

            int value = 0;
            CardDisplay card = null;
            yield return StartCoroutine(SpawnCardAnimated(dealerHandArea, true, true, (v, c) => { value = v; card = c; }));
            if (card != null) AddCardToDealer(value);
        }

        Debug.Log($"Dalintojas baigia ėjimą su {dealerScore} taškais.");
        ChangeState(GameState.RoundOver);
    }

    private void FinishRound(string result)
    {
        Debug.Log(result);

        if (resultText != null)
        {
            resultText.text = result;
            resultText.gameObject.SetActive(true);
        }

        if (RunManager.instance.playerLives <= 0)
        {
            SceneManager.LoadScene("End_screen");
            return;
        }

        currentBet = 0;
        firstHandBet = 0;
        secondHandBet = 0;

        UpdateMoneyUI();

        bool isRoundComplete = RunManager.instance.IsRoundComplete();

        RunUI.instance.UpdateDisplay();

        if (isRoundComplete)
            ShowGoToShopButton();
        else
            StartCoroutine(NextHandDelay());
    }

    private void HandleRoundOver()
    {
        string result = "";

        if (forceWin)
        {
            result = "Player won! (Revolver)";
            RunManager.instance.OnHandWon(firstHandBet);

            forceWin = false;
            forceSkip = false;

            FinishRound(result);
            return;
        }

        if (forceSkip)
        {
            result = "Hand skipped";

            RunManager.instance.OnHandPush(firstHandBet);

            forceWin = false;
            forceSkip = false;

            FinishRound(result);
            return;
        }

        if (!hasSplit)
        {
            if (playerScore > 21)
            {
                result = "Player lost (Bust)";
                if (shieldActive)
                {
                    Debug.Log("Shield apsaugojo nuo pralaimėjimo!");
                    shieldActive = false;
                }
                else
                {
                    RunManager.instance.OnHandLost(firstHandBet);
                }
            }
            else if (dealerScore > 21)
            {
                if (handshakeActive && playerScore == 21)
                {
                    result = "Player won! (Handshake x3)";
                    RunManager.instance.playerMoney += firstHandBet * 3;
                }
                else
                {
                    result = "Player won!";
                    RunManager.instance.OnHandWon(firstHandBet);
                }

                handshakeActive = false;
            }
            else if (playerScore > dealerScore)
            {
                if (handshakeActive && playerScore == 21)
                {
                    result = "Player won! (Handshake x3)";
                    RunManager.instance.playerMoney += firstHandBet * 3;
                }
                else
                {
                    result = "Player won!";
                    RunManager.instance.OnHandWon(firstHandBet);
                }

                handshakeActive = false;
            }
            else if (playerScore < dealerScore)
            {
                result = "Dealer won.";
                if (shieldActive)
                {
                    Debug.Log("Shield apsaugojo nuo pralaimėjimo!");
                    shieldActive = false;
                }
                else
                {
                    RunManager.instance.OnHandLost(firstHandBet);
                }
            }
            else
            {
                result = "Push";
                RunManager.instance.OnHandPush(firstHandBet);
            }
        }
        else
        {
            string hand1Result = ResolveSingleHand(playerScore, firstHandBet, "Hand 1");
            string hand2Result = ResolveSingleHand(splitPlayerScore, secondHandBet, "Hand 2");
            result = hand1Result + "\n" + hand2Result;
        }

        Debug.Log(result);

        if (resultText != null)
        {
            resultText.text = result;
            resultText.gameObject.SetActive(true);
        }

        if (RunManager.instance.playerLives <= 0)
        {
            Debug.Log("Game Over - no lives!");
            SceneManager.LoadScene("End_screen");
            return;
        }

        currentBet = 0;
        firstHandBet = 0;
        secondHandBet = 0;

        UpdateMoneyUI();

        if (endEffects != null)
        {
            if (result.Contains("Player won"))
                endEffects.PlayWinEffect();
            else if (result.Contains("Dealer won") || result.Contains("Bust"))
                endEffects.PlayLoseEffect();
        }

        bool isRoundComplete = RunManager.instance.IsRoundComplete();

        RunUI.instance.UpdateDisplay();

        if (isRoundComplete)
        {
            Debug.Log("Raundas baigtas. Rodomas mygtukas...");
            ShowGoToShopButton();
        }
        else
        {
            Debug.Log($"Raunde dar {RunManager.instance.handsRequiredThisRound - RunManager.instance.handsSurvivedThisRound} handų");
            StartCoroutine(NextHandDelay());
        }
        handshakeActive = false;
    }

    private void ShowGoToShopButton()
    {
        if (RunManager.instance != null && RunManager.instance.isBossRound)
        {
            StartCoroutine(BossDefeatedRoutine());
        }
        else
        {
            if (goToShopButton != null)
            {
                goToShopButton.gameObject.SetActive(true);
                goToShopButton.onClick.RemoveAllListeners();
                goToShopButton.onClick.AddListener(GoToShop);
            }
        }
    }

    private IEnumerator BossDefeatedRoutine()
    {
        if (resultText != null)
        {
            resultText.text = "BOSS DEFEATED!";
            resultText.color = new Color(1f, 0.8f, 0f);
            resultText.gameObject.SetActive(true);
        }

        if (endEffects != null)
        {
            endEffects.PlayWinEffect();
        }

        yield return new WaitForSeconds(3.5f);

        if (RunManager.instance != null)
        {
            RunManager.instance.NextRound();
        }
    }

    public void GoToShop()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("Shop");
    }

    private IEnumerator NextHandDelay()
    {
        yield return new WaitForSeconds(2f);
        if (goToShopButton != null) goToShopButton.gameObject.SetActive(false);
        ChangeState(GameState.Betting);
    }

    private string ResolveSingleHand(int handScore, int handBet, string handName)
    {
        if (handScore > 21)
        {
            RunManager.instance.OnHandLost(handBet);
            return handName + ": Lost (Bust)";
        }
        else if (dealerScore > 21)
        {
            RunManager.instance.OnHandWon(handBet);
            return handName + ": Won";
        }
        else if (handScore > dealerScore)
        {
            RunManager.instance.OnHandWon(handBet);
            return handName + ": Won";
        }
        else if (handScore < dealerScore)
        {
            RunManager.instance.OnHandLost(handBet);
            return handName + ": Lost";
        }
        else
        {
            RunManager.instance.OnHandPush(handBet);
            return handName + ": Push";
        }
    }

    private void UpdateButtons()
    {
        bool playerTurn = currentState == GameState.PlayerTurn;
        bool isBetting = currentState == GameState.Betting;

        if (hitButton != null)
            hitButton.interactable = playerTurn;

        if (standButton != null)
            standButton.interactable = playerTurn;

        if (doubleButton != null)
            doubleButton.interactable = playerTurn && !hasSplit && activeHandIndex == 0 && playerCardCount == 2 && !doubleUsed;

        if (splitButton != null)
            splitButton.interactable = playerTurn &&
                          !hasSplit &&
                          !handshakeActive &&
                          playerCardCount == 2 &&
                          firstPlayerCardValue == secondPlayerCardValue &&
                          RunManager.instance.playerMoney >= currentBet;

        if (dealButton != null)
            dealButton.interactable = isBetting;

        if (allInButton != null)
            allInButton.interactable = isBetting && RunManager.instance.playerMoney > 0;

        if (clearBetButton != null)
            clearBetButton.interactable = isBetting && currentBet > 0;

        if (chipCanvasGroup != null)
        {
            chipCanvasGroup.alpha = isBetting ? 1f : 0.4f;
            chipCanvasGroup.interactable = isBetting;
            chipCanvasGroup.blocksRaycasts = isBetting;
        }

        if (InventoryManager.instance != null)
            InventoryManager.instance.UpdateInventoryUI();
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
            budgetText.text = "$" + RunManager.instance.playerMoney;

        if (currentBetText != null)
        {
            int shownBet = firstHandBet + secondHandBet;
            if (shownBet <= 0) shownBet = currentBet;
            currentBetText.text = "$" + shownBet;

            UpdateSliderLimits();
        }
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

    public void ActivateHandshake()
    {
        if (hasSplit)
        {
            Debug.Log("Handshake negalima naudoti po split!");
            return;
        }

        handshakeActive = true;

        Debug.Log("Handshake aktyvuotas");
    }

    public int GetActiveHand()
    {
        return activeHandIndex;
    }

    public void ForceWin()
    {
        Debug.Log("Force win!");

        forceWin = true;
        ChangeState(GameState.RoundOver);
    }

    public void ForceSkip()
    {
        forceSkip = true;
        ChangeState(GameState.RoundOver);
    }

    private void HandleKeyboardInput()
    {
        if (currentState == GameState.Betting && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnDealButton();
            return;
        }

        if (currentState != GameState.PlayerTurn)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Hit();
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            Stand();
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            Double();
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            TryUsePowerUp(0);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            TryUsePowerUp(1);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            TryUsePowerUp(2);

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            TryUsePowerUp(3);
    }

    private void TryUsePowerUp(int index)
    {
        if (ModifierManager.Instance == null)
            return;

        if (index >= InventoryManager.powerUps.Count)
            return;

        ModifierManager.Instance.ActivatePowerUp(index);
    }

    public void AllIn()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCoinSound();

        if (currentState != GameState.Betting)
        {
            Debug.Log("All-in galima tik statymo fazėje!");
            return;
        }

        int playerMoney = RunManager.instance.playerMoney;

        if (playerMoney <= 0)
        {
            Debug.Log("Neturi pinigų!");
            return;
        }

        currentBet += playerMoney;
        RunManager.instance.playerMoney = 0;

        UpdateMoneyUI();
        UpdateButtons();

        Debug.Log("All-in! Statymas: " + currentBet);
    }

    public void ClearBet()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCoinSound();

        if (currentState != GameState.Betting)
        {
            Debug.Log("Negalima išvalyti statymo dabar!");
            return;
        }

        if (currentBet <= 0)
            return;

        RunManager.instance.playerMoney += currentBet;
        currentBet = 0;

        UpdateMoneyUI();
        UpdateButtons();

        Debug.Log("Statymas išvalytas.");
    }

    public void OnSliderChanged(float value)
    {
        if (isUpdatingSlider) return;

        if (currentState != GameState.Betting) return;

        int sliderValue = Mathf.RoundToInt(value);
        int difference = sliderValue - currentBet;

        if (difference > 0)
        {
            if (RunManager.instance.playerMoney >= difference)
            {
                currentBet += difference;
                RunManager.instance.playerMoney -= difference;
            }
        }
        else if (difference < 0)
        {
            int refund = -difference;
            currentBet -= refund;
            RunManager.instance.playerMoney += refund;
        }

        UpdateMoneyUI();
    }

    void UpdateSliderText()
    {
        if (betSliderText != null)
            betSliderText.text = "Bet: $" + currentBet;
    }

    void UpdateSliderLimits()
    {
        if (betSlider == null) return;

        isUpdatingSlider = true;

        betSlider.maxValue = RunManager.instance.playerMoney + currentBet;
        betSlider.value = currentBet;

        isUpdatingSlider = false;

        UpdateSliderText();
    }

    public bool doubleRewardActive = false;
    public bool shieldActive = false;
    public bool premiumInsuranceActive = false;
    public bool skipHandActive = false;
}