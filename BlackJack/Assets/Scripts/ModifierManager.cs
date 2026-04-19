using UnityEngine;

public class ModifierManager : MonoBehaviour
{
    public static ModifierManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ActivatePowerUp(int index)
    {
        if (GameManager.Instance.currentState != GameState.PlayerTurn)
        {
            Debug.Log("Galios tik per player turn");
            return;
        }

        if (index < 0 || index >= InventoryManager.powerUps.Count)
            return;

        PowerUpData powerUp = InventoryManager.powerUps[index];

        Debug.Log("Naudojama: " + powerUp.powerUpName);

        ApplyPowerUp(powerUp);

        InventoryManager.powerUps.RemoveAt(index);
        InventoryManager.instance.UpdateInventoryUI();
    }

    private void ApplyPowerUp(PowerUpData p)
    {
        switch (p.id)
        {
            case 1: DoubleReward(); break;
            case 2: Dice(); break;
            case 3: PremiumInsurance(); break;
            case 4: QuickCash(); break;
            case 5: HandBoost(); break;
            case 6: Revolver(); break;
            case 7: Shield(); break;
            case 8: SkipHand(); break;
        }
    }

    // =========================
    //  REALÛS EFEKTAI
    // =========================

    void DoubleReward()
    {
        GameManager.Instance.doubleRewardActive = true;
        Debug.Log("2x reward aktyvuotas");
    }

    void Dice()
    {
        int roll = Random.Range(1, 4);

        if (roll == 1)
        {
            RunManager.instance.playerLives++;
            Debug.Log("+1 life");
        }
        else if (roll == 2)
        {
            GameManager.Instance.doubleRewardActive = true;
            Debug.Log("2x reward");
        }
        else
        {
            RunManager.instance.playerMoney += 200;
            Debug.Log("+200");
        }
    }

    void PremiumInsurance()
    {
        if (GameManager.Instance.currentState != GameState.PlayerTurn)
            return;

        GameManager.Instance.ActivateHandshake();
    }

    void QuickCash()
    {
        RunManager.instance.playerMoney += 50;
        Debug.Log("+50");
    }

    void HandBoost()
    {
        if (GameManager.Instance.currentState != GameState.PlayerTurn)
            return;

        Debug.Log("+11 points");

        int handIndex = GameManager.Instance.GetActiveHand();

        GameManager.Instance.AddCardToHand(handIndex, 11);
    }

    void Revolver()
    {
        Debug.Log("Revolver - instant win");

        GameManager.Instance.ForceWin();
    }

    void Shield()
    {
        GameManager.Instance.shieldActive = true;
        Debug.Log("Shield aktyvuotas!");
    }

    void SkipHand()
    {
        Debug.Log("Hand skipped!");

        GameManager.Instance.ForceSkip();
    }
}