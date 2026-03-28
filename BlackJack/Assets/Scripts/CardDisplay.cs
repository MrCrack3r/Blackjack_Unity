using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;
    public Sprite cardBackSprite;
    private Sprite cardFrontSprite;
    private bool isFaceUp = true;

    public void SetupCard(Sprite frontSprite, bool faceUp = true)
    {
        if (cardImage == null) cardImage = GetComponent<Image>();
        cardFrontSprite = frontSprite;
        isFaceUp = faceUp;
        UpdateCardVisual();
    }

    public void PlayDealAnimation(Vector3 startWorldPos, bool faceUp, Canvas rootCanvas, System.Action onComplete = null)
    {
        StartCoroutine(DealAnimation(startWorldPos, faceUp, rootCanvas, onComplete));
    }

    private IEnumerator DealAnimation(Vector3 startWorldPos, bool faceUp, Canvas rootCanvas, System.Action onComplete)
    {
        cardImage.enabled = false;

        GameObject ghost = new GameObject("GhostCard");
        ghost.transform.SetParent(rootCanvas.transform, false);

        Image ghostImage = ghost.AddComponent<Image>();
        ghostImage.sprite = cardBackSprite;
        ghostImage.raycastTarget = false;

        RectTransform ghostRect = ghost.GetComponent<RectTransform>();
        RectTransform myRect = GetComponent<RectTransform>();
        ghostRect.sizeDelta = myRect.sizeDelta;
        ghostRect.pivot = new Vector2(0.5f, 0.5f);

        ghostRect.position = startWorldPos;
        ghost.transform.localScale = Vector3.one * 0.5f;

        Vector3 targetWorldPos = myRect.position;

        float duration = 0.35f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            float eased = 1f - Mathf.Pow(1f - progress, 3f);

            ghostRect.position = Vector3.Lerp(startWorldPos, targetWorldPos, eased);
            ghost.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, eased);

            yield return null;
        }

        Destroy(ghost);
        cardImage.enabled = true;

        if (faceUp)
            yield return StartCoroutine(FlipAnimation(true));
        else
            UpdateCardVisual();

        onComplete?.Invoke();
    }

    public void FlipCard(bool faceUp)
    {
        if (isFaceUp == faceUp) return;
        StartCoroutine(FlipAnimation(faceUp));
    }

    public IEnumerator FlipAnimation(bool faceUp)
    {
        float duration = 0.15f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = new Vector3(1f - (t / duration), 1f, 1f);
            yield return null;
        }

        isFaceUp = faceUp;
        UpdateCardVisual();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFlipSound();

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = new Vector3(t / duration, 1f, 1f);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    private void UpdateCardVisual()
    {
        if (cardImage == null) return;
        cardImage.sprite = isFaceUp ? cardFrontSprite : cardBackSprite;
    }
}