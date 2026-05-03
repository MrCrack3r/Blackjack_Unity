using UnityEngine;
using TMPro;

public class EndScreenStats : MonoBehaviour
{
	public TMP_Text lastRunRoundText;
	public TMP_Text lastRunMoneyText;
	public TMP_Text lastRunGamesText;
	public TMP_Text lastRunPowerUpsText;
	public TMP_Text lastRunHandsWonText;
	public TMP_Text lastRunHandsLostText;
	public TMP_Text lastRunCardsUsedText;

	public TMP_Text bestRoundText;
	public TMP_Text bestMoneyText;

	void Start()
	{
		lastRunRoundText.text = "Reached Round: " + PlayerPrefs.GetInt("LastRunRound", 0);
		lastRunMoneyText.text = "Remaining Budget: $" + PlayerPrefs.GetInt("LastRunMoney", 0);
		lastRunGamesText.text = "Games Played: " + PlayerPrefs.GetInt("LastRunGames", 0);
		lastRunPowerUpsText.text = "Power-Ups Bought: " + PlayerPrefs.GetInt("LastRunPowerUps", 0);
		lastRunHandsWonText.text = "Hands Won: " + PlayerPrefs.GetInt("LastRunHandsWon", 0);
		lastRunHandsLostText.text = "Hands Lost: " + PlayerPrefs.GetInt("LastRunHandsLost", 0);
		lastRunCardsUsedText.text = "Cards Used: " + PlayerPrefs.GetInt("LastRunCardsUsed", 0);

		bestRoundText.text = "Best Round: " + PlayerPrefs.GetInt("BestRound", 0);
		bestMoneyText.text = "Highest Budget: $" + PlayerPrefs.GetInt("BestMoney", 0);
	}
}