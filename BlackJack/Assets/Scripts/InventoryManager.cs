using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
	public static InventoryManager instance;

	public List<PowerUpData> powerUps = new List<PowerUpData>();

	private int maxPowerUps = 5;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	public bool AddPowerUp(PowerUpData newPowerUp)
	{
		if (powerUps.Count >= maxPowerUps)
		{
			Debug.Log("Inventory full!");
			return false;
		}

		powerUps.Add(newPowerUp);
		return true;
	}

	public void RemovePowerUp(PowerUpData powerUp)
	{
		powerUps.Remove(powerUp);
	}
}