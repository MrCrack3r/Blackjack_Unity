using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public int slotIndex;

    public void OnClick()
    {
        // Tik shop'e leidþiam iðmesti
        if (ShopManager.instance == null)
        {
            Debug.Log("Kortas galima mesti tik shop'e!");
            return;
        }

        if (slotIndex >= InventoryManager.powerUps.Count)
            return;

        Debug.Log("Iðmesta korta: " + InventoryManager.powerUps[slotIndex].powerUpName);

        InventoryManager.powerUps.RemoveAt(slotIndex);
        InventoryManager.instance.UpdateInventoryUI();
    }
}