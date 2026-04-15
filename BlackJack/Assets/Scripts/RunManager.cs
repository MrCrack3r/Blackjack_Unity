using UnityEngine;

public class RunManager : MonoBehaviour
{
	public static RunManager instance;

	public int currentRound = 1;
	public int gamesPlayed = 0;
	public int playerLives = 3;
	public int playerMoney = 200;

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
}