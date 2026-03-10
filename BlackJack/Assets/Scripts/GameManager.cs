using UnityEngine;
using System.Collections; // Reikalinga Coroutine (laikmačiams) naudoti
using UnityEngine.InputSystem;

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

    // Išsaugome nuorodą į užverstą dalintojo kortą, kad vėliau galėtume ją atversti
    private CardDisplay dealerHiddenCard;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ChangeState(GameState.Betting);
    }

    // LAIKINAS TESTAVIMAS: Paspaudus SPACE klavišą, imituojame "Stand" mygtuko paspaudimą
    private void Update()
    {
        if (currentState == GameState.PlayerTurn && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Žaidėjas paspaudė Space (Stand). Eilė pereina dalintojui.");
            ChangeState(GameState.DealerTurn);
        }
    }



    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("Žaidimo būsena pasikeitė į: " + newState);

        switch (currentState)
        {
            case GameState.Betting:
                ChangeState(GameState.Dealing); // Laikinai iškart praleidžiame statymus
                break;
            case GameState.Dealing:
                HandleDealing();
                break;
            case GameState.PlayerTurn:
                Debug.Log("Laukiame žaidėjo... (PASPAUSKITE 'SPACE' KAD BAIGTI ĖJIMĄ)");
                break;
            case GameState.DealerTurn:
                StartCoroutine(DealerPlayRoutine()); // Naudojame Coroutine dėl pauzių
                break;
            case GameState.RoundOver:
                Debug.Log("Raundas baigėsi! Skaičiuojami rezultatai.");
                break;
        }
    }

    private void HandleDealing()
    {
        Debug.Log("Dalinamos kortos...");
        SpawnCard(playerHandArea, true);
        SpawnCard(playerHandArea, true);

        SpawnCard(dealerHandArea, true);
        // Išsaugome antrąją dalintojo kortą į kintamąjį
        dealerHiddenCard = SpawnCard(dealerHandArea, false);

        ChangeState(GameState.PlayerTurn);
    }

    // Pakeitėme funkciją, kad ji grąžintų sukurtą CardDisplay skriptą
    private CardDisplay SpawnCard(Transform area, bool faceUp)
    {
        GameObject newCard = Instantiate(cardPrefab, area);
        CardDisplay display = newCard.GetComponent<CardDisplay>();

        Sprite randomSprite = null;
        if (testCardSprites != null && testCardSprites.Length > 0)
        {
            randomSprite = testCardSprites[Random.Range(0, testCardSprites.Length)];
        }

        display.SetupCard(randomSprite, faceUp);
        return display; // Grąžiname kortos komponentą
    }

    // --- NAUJA SCRUM-109 LOGIKA ---
    private IEnumerator DealerPlayRoutine()
    {
        yield return new WaitForSeconds(1f); // Palaukiame 1 sekundę dėl natūralumo

        // 1. Dalintojas atverčia savo paslėptą kortą
        if (dealerHiddenCard != null)
        {
            dealerHiddenCard.FlipCard(true);
            Debug.Log("Dalintojas atverčia savo kortą.");
        }

        yield return new WaitForSeconds(1f);

        // Laikinas taškų kintamasis (Mocking), kol kiti padarys SCRUM-10
        // Pradedame pvz. nuo 14 taškų, kad pamatytume, kaip jis traukia kortą
        int dummyDealerScore = 14;

        // 2. Traukimo ciklas: kol taškai mažiau nei 17
        while (dummyDealerScore < 17)
        {
            Debug.Log($"Dalintojas turi {dummyDealerScore} taškų. Traukia dar vieną kortą...");
            SpawnCard(dealerHandArea, true);

            // Pridedame atsitiktinį taškų kiekį (nuo 2 iki 10)
            dummyDealerScore += Random.Range(2, 11);

            yield return new WaitForSeconds(1.5f); // Palaukiame prieš traukiant kitą
        }

        Debug.Log($"Dalintojas baigia ėjimą turėdamas {dummyDealerScore} taškų.");

        // 3. Raundo pabaiga
        ChangeState(GameState.RoundOver);
    }
}