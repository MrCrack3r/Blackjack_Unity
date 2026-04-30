using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager instance { get; private set; }

    public int currentRound = 1;
    public int handsRequiredThisRound = 3;
    public int handsSurvivedThisRound = 0;
    public int playerLives = 3;
    public int playerMoney = 200;
    public int gamesPlayed = 0;
    public int highestMoneyThisRun = 200;
    public int powerUpsBoughtThisRun = 0;
    public int handsWonThisRun = 0;
    public int handsLostThisRun = 0;
    public int cardsUsedThisRun = 0;

    // Boss Round kintamasis
    public bool isBossRound = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetupRound();
    }

    public int roundsSinceLastBoss = 0;

    // PAKEISK SETUPROUND FUNKCIJĄ:
    public void SetupRound()
    {
        handsRequiredThisRound = 2 + currentRound;
        handsSurvivedThisRound = 0;

        // Patikriname, ar DABAR yra boso raundas
        if (isBossRound)
        {
            Debug.Log("BOSS RAUNDAS AKTYVUOTAS!");
            if (BossModifierManager.Instance != null)
            {
                BossModifierManager.Instance.ApplyRandomModifier();
            }
        }
        else
        {
            // Tai normalus raundas, išvalome boso efektus
            if (BossModifierManager.Instance != null)
            {
                BossModifierManager.Instance.ClearModifier();
            }
        }
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    public void OnHandWon(int betAmount)
    {
        playerMoney += betAmount * 2;

        if (playerMoney > highestMoneyThisRun)
        {
            highestMoneyThisRun = playerMoney;
        }

        handsSurvivedThisRound++;
        handsWonThisRun++;
    }

    public void OnHandPush(int betAmount)
    {
        playerMoney += betAmount;

        if (playerMoney > highestMoneyThisRun)
        {
            highestMoneyThisRun = playerMoney;
        }

        handsSurvivedThisRound++;
    }

    public void OnHandLost(int betAmount)
    {
        handsSurvivedThisRound++;
        handsLostThisRun++;

        // STANDARTINĖ ŽALA YRA 1
        int damageToTake = 1;

        // TIKRINAME BOSO EFEKTĄ: Ar pritaikyta dviguba žala?
        if (BossModifierManager.Instance != null && BossModifierManager.Instance.currentModifier == BossModifierManager.ModifierType.DoubleDamage)
        {
            damageToTake = 2; // Atimame 2 gyvybes
            Debug.Log("BOSS EFEKTAS: Gauta dviguba žala!");
        }

        playerLives -= damageToTake;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDamageSound();
        }

        if (playerLives <= 0)
        {
            GameOver();
            return;
        }

        if (playerMoney <= 0)
        {
            GameOver();
            return;
        }
    }

    public bool IsRoundComplete()
    {
        return handsSurvivedThisRound >= handsRequiredThisRound;
    }

    public void NextRound()
    {
        // Jeigu ką tik baigėsi boso raundas
        if (isBossRound)
        {
            isBossRound = false; // Išjungiame boso būseną
            roundsSinceLastBoss = 0; // Nunuliname skaitliuką

            currentRound++; // Pereiname prie normalaus raundo (pvz. į 4-ą)
            Debug.Log("Boso raundas baigtas. Pereiname į normalų Raundą: " + currentRound);
        }
        else
        {
            // Tai buvo normalus raundas
            roundsSinceLastBoss++;

            // Jei tai buvo 3-ias normalus raundas (po paskutinio boso arba nuo žaidimo pradžios)
            if (roundsSinceLastBoss >= 3)
            {
                // Sekantis raundas bus boso raundas (nedidiname currentRound skaičiaus!)
                isBossRound = true;
                Debug.Log("3 Raundai praėjo. Sekantis bus BOSS RAUNDAS!");
            }
            else
            {
                // Jei dar nepraėjo 3 raundai, tiesiog einame į kitą normalų raundą
                currentRound++;
            }
        }

        SetupRound();
    }

    public void ResetRun()
    {
        currentRound = 1;
        roundsSinceLastBoss = 0; // PRIDĖTA EILUTĖ
        isBossRound = false;     // PRIDĖTA EILUTĖ
        handsSurvivedThisRound = 0;
        playerLives = 3;
        playerMoney = 200;
        gamesPlayed = 0;
        highestMoneyThisRun = 200;
        powerUpsBoughtThisRun = 0;
        handsWonThisRun = 0;
        handsLostThisRun = 0;
        cardsUsedThisRun = 0;

        SetupRound();
    }

    private void SaveEndGameStats()
    {
        PlayerPrefs.SetInt("LastRunRound", currentRound);
        PlayerPrefs.SetInt("LastRunMoney", playerMoney);
        PlayerPrefs.SetInt("LastRunGames", gamesPlayed);
        PlayerPrefs.SetInt("LastRunPowerUps", powerUpsBoughtThisRun);
        PlayerPrefs.SetInt("LastRunHandsWon", handsWonThisRun);
        PlayerPrefs.SetInt("LastRunHandsLost", handsLostThisRun);
        PlayerPrefs.SetInt("LastRunCardsUsed", cardsUsedThisRun);

        int bestRound = PlayerPrefs.GetInt("BestRound", 0);
        if (currentRound > bestRound)
        {
            PlayerPrefs.SetInt("BestRound", currentRound);
        }

        int bestMoney = PlayerPrefs.GetInt("BestMoney", 0);
        if (highestMoneyThisRun > bestMoney)
        {
            PlayerPrefs.SetInt("BestMoney", highestMoneyThisRun);
        }

        PlayerPrefs.Save();
    }

    private void OpenShop()
    {
        // Įgyvendinta SCRUM-50 taisyklė: po boso raundo į parduotuvę nepatenkama
        if (isBossRound)
        {
            Debug.Log("Parduotuvė praleidžiama po Boso Raundo! Tęsiamas žaidimas...");
            NextRound();
            return;
        }

        Debug.Log("Atidaroma parduotuvė...");
        SceneManager.LoadScene("Shop");
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER! Pasiektas: Round " + currentRound);

        SaveEndGameStats();

        PlayerPrefs.DeleteKey("Money");
        PlayerPrefs.DeleteKey("Lives");
        PlayerPrefs.DeleteKey("Round");
        PlayerPrefs.DeleteKey("Games");
        PlayerPrefs.DeleteKey("PowerUpCount");

        SceneManager.LoadScene("End_screen");
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("Money", playerMoney);
        PlayerPrefs.SetInt("Lives", playerLives);
        PlayerPrefs.SetInt("Round", currentRound);
        PlayerPrefs.SetInt("Games", gamesPlayed);

        // PlayerPrefs.SetInt("PowerUpCount", InventoryManager.powerUps.Count);

        PlayerPrefs.Save();

        Debug.Log("Game saved!");
    }

    public void LoadGame()
    {
        playerMoney = PlayerPrefs.GetInt("Money", 200);
        playerLives = PlayerPrefs.GetInt("Lives", 3);
        currentRound = PlayerPrefs.GetInt("Round", 1);
        gamesPlayed = PlayerPrefs.GetInt("Games", 0);

        Debug.Log("Game loaded!");
    }
}