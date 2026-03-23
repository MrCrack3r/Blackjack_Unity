using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardDisplay : MonoBehaviour
{
    [Header("Kortos grafika")]
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

    public void FlipCard(bool faceUp)
    {
        if (isFaceUp == faceUp) return;
        StartCoroutine(FlipAnimation(faceUp));
    }

    private IEnumerator FlipAnimation(bool faceUp)
    {
        float duration = 0.15f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float scale = 1f - (t / duration);
            transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        isFaceUp = faceUp;
        UpdateCardVisual();

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float scale = t / duration;
            transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    private void UpdateCardVisual()
    {
        if (cardImage == null) return;

        if (isFaceUp)
            cardImage.sprite = cardFrontSprite;
        else
            cardImage.sprite = cardBackSprite;
    }
}