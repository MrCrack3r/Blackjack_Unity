using UnityEngine;

public class InventorySlotUI : MonoBehaviour
{
    public int slotIndex;
    public int sellValue = 100; 

    public void OnClick()
    {
   
        if (ShopManager.instance == null)
        {
            Debug.Log("Parduoti galima tik shop'e!");
            return;
        }

        if (slotIndex >= InventoryManager.powerUps.Count)
            return;

        PowerUpData powerUp = InventoryManager.powerUps[slotIndex];

        Debug.Log("Parduota korta: " + powerUp.powerUpName);

 
        RunManager.instance.playerMoney += sellValue;

     
        InventoryManager.powerUps.RemoveAt(slotIndex);

       
        InventoryManager.instance.UpdateInventoryUI();


        if (ShopManager.instance != null)
            ShopManager.instance.ShowNotification("Sold for $" + sellValue, Color.green);
    }
}