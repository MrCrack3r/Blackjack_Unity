using UnityEngine;

public class RulesMenu : MonoBehaviour
{
    public GameObject rulesPanel;

    public void OpenRules()
    {
        rulesPanel.SetActive(true);
    }

    public void CloseRules()
    {
        rulesPanel.SetActive(false);
    }
}