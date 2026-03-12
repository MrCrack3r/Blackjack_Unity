using System.Collections.Generic;

public class Hand
{
    public List<Card> cards = new List<Card>();

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public int CalculateValue()
    {
        int total = 0;
        int aceCount = 0;

        foreach (Card card in cards)
        {
            total += card.GetBlackjackValue();

            if (card.Value == CardValue.Ace)
            {
                aceCount++;
            }
        }

        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }

        return total;
    }

    public void Clear()
    {
        cards.Clear();
    }
}