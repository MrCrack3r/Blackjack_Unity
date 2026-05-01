using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class CardUseAnimator : MonoBehaviour
{
    public static CardUseAnimator Instance { get; private set; }

    public Image cardImage;
    public TextMeshProUGUI powerUpText;

    private CanvasGroup canvasGroup;

    public float moveDuration = 0.5f;
    public float displayDuration = 1.0f;
    public float fadeDuration = 0.4f;
    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);

    private Vector3 centerPosition;

    private void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();

        centerPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        gameObject.SetActive(false);
    }

    public void AnimateCardUse(Vector3 startPosition, Sprite sprite, string cardName)
    {
        cardImage.sprite = sprite;
        powerUpText.text = cardName;

        transform.position = startPosition;
        transform.localScale = Vector3.one;
        canvasGroup.alpha = 1f;

        gameObject.SetActive(true);
        StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

            float easeT = t * (2f - t);

            transform.position = Vector3.Lerp(startPos, centerPosition, easeT);
            transform.localScale = Vector3.Lerp(Vector3.one, targetScale, easeT);
            yield return null;
        }

        transform.position = centerPosition;
        transform.localScale = targetScale;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPowerUpSound();
        }

        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            transform.localScale = Vector3.Lerp(targetScale, targetScale * 1.3f, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}