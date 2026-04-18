using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("UI Elementai")]
    public TextMeshProUGUI notificationText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 1. Paleidžiame parduotuvės muziką
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShopMusic();
        }

        // 2. Paslepiame pranešimą parduotuvės atidarymo metu
        if (notificationText != null) notificationText.gameObject.SetActive(false);
    }

    public void ShowNotification(string message, Color color)
    {
        if (notificationText != null)
        {
            StopAllCoroutines();
            StartCoroutine(NotificationRoutine(message, color));
        }
    }

    private IEnumerator NotificationRoutine(string message, Color color)
    {
        notificationText.text = message;
        notificationText.color = color;
        notificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        notificationText.gameObject.SetActive(false);
    }

    public void ContinueToNextRound()
    {
        Debug.Log("Parduotuvė uždaroma. Pradedamas kitas raundas!");

        if (RunManager.instance != null)
        {
            RunManager.instance.NextRound();
        }

        SceneManager.LoadScene("Backjack_table_scene");
    }
}