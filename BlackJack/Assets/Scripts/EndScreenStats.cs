using UnityEngine;
using TMPro;


public class EndScreenStats : MonoBehaviour
{
	public TMP_Text lastRunRoundText;
	public TMP_Text lastRunMoneyText;
	public TMP_Text lastRunGamesText;
	public TMP_Text lastRunPowerUpsText;

	public TMP_Text bestRoundText;
	public TMP_Text bestMoneyText;

	void Start()
	{
		lastRunRoundText.text = "Pasiektas raundas: " + PlayerPrefs.GetInt("LastRunRound", 0);
		lastRunMoneyText.text = "Likęs biudžetas: $" + PlayerPrefs.GetInt("LastRunMoney", 0);
		lastRunGamesText.text = "Sužaista partijų: " + PlayerPrefs.GetInt("LastRunGames", 0);
		lastRunPowerUpsText.text = "Nupirkta galių: " + PlayerPrefs.GetInt("LastRunPowerUps", 0);

		bestRoundText.text = "Geriausias raundas: " + PlayerPrefs.GetInt("BestRound", 0);
		bestMoneyText.text = "Didžiausias biudžetas: $" + PlayerPrefs.GetInt("BestMoney", 0);
	}
}