using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
	public TextMeshProUGUI moneyText;

	void Update()
	{
		moneyText.text = "$" + RunManager.instance.playerMoney.ToString();
	}
}