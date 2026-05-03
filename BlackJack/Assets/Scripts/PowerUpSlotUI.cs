using UnityEngine;
using UnityEngine.EventSystems;

public class PowerUpSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int slotIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.ShowTooltip(slotIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.HideTooltip();
        }
    }
}