using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class EndEffects : MonoBehaviour
{
    [Header("Particle Prefabai")]
    public GameObject winParticlePrefab;
    public GameObject loseParticlePrefab;

    [Header("UI Efektai")]
    public TextMeshProUGUI resultText;
    public Image screenFlash;

    public void PlayWinEffect()
    {
        if (winParticlePrefab != null)
        {
            GameObject p = Instantiate(winParticlePrefab, Vector3.zero, Quaternion.identity);
            Destroy(p, 4f);
        }

        if (screenFlash != null)
            StartCoroutine(FlashScreen(new Color(1f, 0.9f, 0f, 0.3f)));

        if (resultText != null)
            StartCoroutine(AnimateResultText(true));
    }

    public void PlayLoseEffect()
    {
        if (loseParticlePrefab != null)
        {
            GameObject p = Instantiate(loseParticlePrefab, Vector3.zero, Quaternion.identity);
            Destroy(p, 4f);
        }

        if (screenFlash != null)
            StartCoroutine(FlashScreen(new Color(1f, 0f, 0f, 0.3f)));

        if (resultText != null)
            StartCoroutine(AnimateResultText(false));
    }

    private IEnumerator FlashScreen(Color flashColor)
    {
        screenFlash.gameObject.SetActive(true);
        screenFlash.color = flashColor;

        float t = 0f;
        float duration = 0.5f;
        Color start = flashColor;
        Color end = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);

        while (t < duration)
        {
            t += Time.deltaTime;
            screenFlash.color = Color.Lerp(start, end, t / duration);
            yield return null;
        }

        screenFlash.gameObject.SetActive(false);
    }

    private IEnumerator AnimateResultText(bool isWin)
    {
        resultText.gameObject.SetActive(true);
        resultText.transform.localScale = Vector3.zero;

        Color targetColor = isWin ? new Color(1f, 0.85f, 0f) : new Color(1f, 0.2f, 0.2f);
        resultText.color = targetColor;

        // Įsijungia
        float t = 0f;
        float popIn = 0.3f;
        while (t < popIn)
        {
            t += Time.deltaTime;
            float progress = t / popIn;
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            resultText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.2f, eased);
            yield return null;
        }

        t = 0f;
        float scaleBack = 0.1f;
        while (t < scaleBack)
        {
            t += Time.deltaTime;
            resultText.transform.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, t / scaleBack);
            yield return null;
        }

        // Laimėjimo atveju – pulsuoja
        if (isWin)
        {
            for (int i = 0; i < 3; i++)
            {
                t = 0f;
                while (t < 0.2f)
                {
                    t += Time.deltaTime;
                    float pulse = 1f + Mathf.Sin(t / 0.2f * Mathf.PI) * 0.1f;
                    resultText.transform.localScale = Vector3.one * pulse;
                    yield return null;
                }

            }
            resultText.transform.localScale = Vector3.one;
        }
    }
}