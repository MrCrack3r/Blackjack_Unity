public enum CardSuit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public enum CardValue
{
    Two = 2,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}

public class Card
{
    public CardSuit Suit;
    public CardValue Value;

    public Card(CardSuit suit, CardValue value)
    {
        Suit = suit;
        Value = value;
    }

    public int GetBlackjackValue()
    {
        if (Value == CardValue.Jack || Value == CardValue.Queen || Value == CardValue.King)
            return 10;

        if (Value == CardValue.Ace)
            return 11;

        return (int)Value;
    }
}