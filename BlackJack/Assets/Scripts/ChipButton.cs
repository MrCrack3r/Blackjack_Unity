using UnityEngine;

public class ChipButton : MonoBehaviour
{
    public int chipValue;

    public void OnChipClicked()
    {
        GameManager.Instance.AddBet(chipValue);
    }
}