using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public int slotIndex;

    public void OnClick()
    {
        if (ShopManager.instance == null)
        {
            Debug.Log("Kortas galima mesti tik shop'e!");
            return;
        }

        if (slotIndex >= InventoryManager.powerUps.Count)
            return;

        Debug.Log("Išmesta korta: " + InventoryManager.powerUps[slotIndex].powerUpName);

        ShopManager.instance.ShowNotification("Išmesta: " + InventoryManager.powerUps[slotIndex].powerUpName, Color.yellow);

        InventoryManager.powerUps.RemoveAt(slotIndex);
        InventoryManager.instance.UpdateInventoryUI();
    }
}