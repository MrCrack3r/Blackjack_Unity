using UnityEngine;

[CreateAssetMenu(fileName = "NewPowerUp", menuName = "PowerUps/PowerUp Data")]
public class PowerUpData : ScriptableObject
{
	public string id;
	public string powerUpName;
	public string description;

	public Sprite icon;

	public int baseCost;

}

