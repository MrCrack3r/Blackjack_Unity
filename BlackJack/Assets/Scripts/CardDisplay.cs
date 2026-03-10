using UnityEngine;
using UnityEngine.UI;

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
        isFaceUp = faceUp;

        UpdateCardVisual();
    }

    private void UpdateCardVisual()
    {
        if (isFaceUp)
        {
            cardImage.sprite = cardFrontSprite;
        }
        else
        {
            cardImage.sprite = cardBackSprite;
        }
    }
}
