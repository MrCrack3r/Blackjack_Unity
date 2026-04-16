using System.Collections.Generic; 
using System;                    

public class Deck
{
    public List<Card> cards;

    public Deck()
    {
        GenerateDeck();
    }

    private void GenerateDeck()
    {
        cards = new List<Card>();

        foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
            {
                cards.Add(new Card(suit, value));
            }
        }
    }

    public void Shuffle()
    {
        System.Random rng = new System.Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card temp = cards[k];
            cards[k] = cards[n];
            cards[n] = temp;
        }
    }

    public Card DrawCard()
    {
        if (cards.Count == 0)
        {
            return null;
        }

        Card cardToDraw = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        return cardToDraw;
    }
}