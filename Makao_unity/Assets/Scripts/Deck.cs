
using System;
using System.Collections.Generic;

public class Deck
{
    public List<Card> Cards { get; private set; }

    public Deck()
    {
        Cards = new List<Card>();

        foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
            {
                Cards.Add(new Card(suit, rank));
            }
        }
    }

    public void Shuffle()
    {
        Random random = new Random();

        for (int i = Cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);

            Card temp = Cards[i];
            Cards[i] = Cards[j];
            Cards[j] = temp;
        }
    }

    public Card DrawCard()
    {
        if (Cards.Count == 0)
            return null;

        Card card = Cards[0];
        Cards.RemoveAt(0);

        return card;
    }
}
