using System.Collections.Generic; // Būtina, kad galėtume naudoti List<>
using System;                    // Būtina, kad galėtume naudoti Enum ir Random

public class Deck
{
    // Sąrašas, kuriame saugosime visas kaladės kortas.
    public List<Card> cards;

    // Konstruktorius, kuris iškviečiamas sukūrus naują Deck objektą.
    public Deck()
    {
        GenerateDeck();
    }

    // Privatus metodas, kuris sugeneruoja visas 52 kortas.
    private void GenerateDeck()
    {
        // Inicializuojame naują kortų sąrašą.
        cards = new List<Card>();

        // Ciklas per visus kortų tipus (Hearts, Diamonds, etc.)
        foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
        {
            // Ciklas per visas kortų vertes (Two, Three, ..., Ace)
            foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
            {
                // Sukuriame naują kortą su dabartiniu tipu ir verte
                // ir pridedame ją į savo sąrašą (kaladę).
                cards.Add(new Card(suit, value));
            }
        }
    }

    // Metodas kaladės maišymui.
    // Naudoja Fisher-Yates algoritmą - standartinį ir efektyvų būdą maišymui.
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

    // Metodas kortos paėmimui iš kaladės viršaus.
    public Card DrawCard()
    {
        // Patikriname, ar kaladėje dar yra kortų.
        if (cards.Count == 0)
        {
            // Jei kaladė tuščia, grąžiname null (reikšmės nebuvimas).
            // Vėliau savo kode galėsime patikrinti, ar gauta korta nėra null.
            return null;
        }

        // Pasiimame viršutinę kortą (paskutinę sąraše).
        Card cardToDraw = cards[cards.Count - 1];
        // Pašaliname ją iš sąrašo.
        cards.RemoveAt(cards.Count - 1);
        // Grąžiname ištrauktą kortą.
        return cardToDraw;
    }
}