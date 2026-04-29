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

    public void SetupRound()
    {
        handsRequiredThisRound = 2 + currentRound;
        handsSurvivedThisRound = 0;
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
        playerLives--;

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
        currentRound++;
        SetupRound();
    }

    public void ResetRun()
    {
        currentRound = 1;
        handsSurvivedThisRound = 0;
        playerLives = 3;
        playerMoney = 200;
		gamesPlayed = 0;
		highestMoneyThisRun = 200;
		powerUpsBoughtThisRun = 0;

		SetupRound();
	}

	private void SaveEndGameStats()
	{
		PlayerPrefs.SetInt("LastRunRound", currentRound);
		PlayerPrefs.SetInt("LastRunMoney", playerMoney);
		PlayerPrefs.SetInt("LastRunGames", gamesPlayed);
		PlayerPrefs.SetInt("LastRunPowerUps", powerUpsBoughtThisRun);

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

        PlayerPrefs.SetInt("PowerUpCount", InventoryManager.powerUps.Count);

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