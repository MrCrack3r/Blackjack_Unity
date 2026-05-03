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

    public void SetupRound()
    {
        handsSurvivedThisRound = 0;

        if (isBossRound)
        {
            handsRequiredThisRound = 2;

            if (BossModifierManager.Instance != null)
            {
                BossModifierManager.Instance.ApplyRandomModifier();
            }
        }
        else
        {
            handsRequiredThisRound = 2 + currentRound;

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

        int damageToTake = 1;

        if (BossModifierManager.Instance != null && BossModifierManager.Instance.currentModifier == BossModifierManager.ModifierType.DoubleDamage)
        {
            damageToTake = 2;
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
        if (isBossRound)
        {
            isBossRound = false;
            roundsSinceLastBoss = 0;
            currentRound++;
        }
        else
        {
            roundsSinceLastBoss++;

            if (roundsSinceLastBoss >= 3)
            {
                isBossRound = true;
            }
            else
            {
                currentRound++;
            }
        }

        SetupRound();

        SceneManager.LoadScene("Backjack_table_scene");
    }

    public void ResetRun()
    {
        currentRound = 1;
        roundsSinceLastBoss = 0;
        isBossRound = false;
        handsSurvivedThisRound = 0;
        playerLives = 3;
        playerMoney = 200;
        gamesPlayed = 0;
        highestMoneyThisRun = 200;
        powerUpsBoughtThisRun = 0;
        handsWonThisRun = 0;
        handsLostThisRun = 0;
        cardsUsedThisRun = 0;

        PlayerPrefs.DeleteKey("SavedInventory");
        PlayerPrefs.DeleteKey("HasStartCard");

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
        PlayerPrefs.SetInt("LastRunHighestMoney", highestMoneyThisRun);

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

    private void GameOver()
    {
        SaveEndGameStats();

        PlayerPrefs.DeleteKey("Money");
        PlayerPrefs.DeleteKey("Lives");
        PlayerPrefs.DeleteKey("Round");
        PlayerPrefs.DeleteKey("Games");
        PlayerPrefs.DeleteKey("PowerUpCount");
        PlayerPrefs.DeleteKey("SavedInventory");
        PlayerPrefs.DeleteKey("HasStartCard");

        InventoryManager.ClearInventory();

        SceneManager.LoadScene("End_screen");
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("Money", playerMoney);
        PlayerPrefs.SetInt("Lives", playerLives);
        PlayerPrefs.SetInt("Round", currentRound);
        PlayerPrefs.SetInt("Games", gamesPlayed);

        string invStr = "";
        for (int i = 0; i < InventoryManager.powerUps.Count; i++)
        {
            invStr += InventoryManager.powerUps[i].name;
            if (i < InventoryManager.powerUps.Count - 1) invStr += ",";
        }
        PlayerPrefs.SetString("SavedInventory", invStr);
        PlayerPrefs.SetInt("HasStartCard", InventoryManager.hasReceivedStartCard ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        playerMoney = PlayerPrefs.GetInt("Money", 200);
        playerLives = PlayerPrefs.GetInt("Lives", 3);
        currentRound = PlayerPrefs.GetInt("Round", 1);
        gamesPlayed = PlayerPrefs.GetInt("Games", 0);
    }
}