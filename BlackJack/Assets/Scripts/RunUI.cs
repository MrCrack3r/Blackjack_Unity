using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RunUI : MonoBehaviour
{
	public TextMeshProUGUI roundText;
	public TextMeshProUGUI gamesText;

	public Image[] hearts;

	void Update()
	{
		roundText.text = "Round: " + RunManager.instance.currentRound;
		gamesText.text = "Games: " + RunManager.instance.gamesPlayed;

		for (int i = 0; i < hearts.Length; i++)
		{
			hearts[i].enabled = i < RunManager.instance.playerLives;
		}
	}
}