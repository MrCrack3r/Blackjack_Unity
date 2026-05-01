using UnityEngine;
using TMPro;
using System.Collections;

public class RoundAnnouncer : MonoBehaviour
{
    [Header("Priskirkite tekstą čia:")]
    public TextMeshProUGUI announceText;

    [Header("Nustatymai")]
    public float displayTime = 2f;
    public float fadeTime = 1f;

    void Start()
    {
        // Jei pamiršite priskirti tekstą, kodas pats jį ras ir nesulūš
        if (announceText == null)
            announceText = GetComponent<TextMeshProUGUI>();

        // Saugiklis: jei teksto apskritai nėra, kodas tiesiog sustoja, bet žaidimas veikia toliau
        if (announceText == null) return;

        announceText.raycastTarget = false; // Išjungiam blokavimą

        if (RunManager.instance != null)
        {
            announceText.text = "Round " + RunManager.instance.currentRound;
        }

        StartCoroutine(AnnounceRoutine());
    }

    private IEnumerator AnnounceRoutine()
    {
        Color textColor = announceText.color;
        textColor.a = 1f;
        announceText.color = textColor;

        yield return new WaitForSeconds(displayTime);

        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            announceText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }

        // SAUGUS BŪDAS: Užuot naikinę objektą, mes jį tiesiog paslepiame
        announceText.gameObject.SetActive(false);
    }
}