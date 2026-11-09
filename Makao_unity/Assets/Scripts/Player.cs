
using System.Collections.Generic;

public class Player
{
    public string Name { get; private set; }
    public List<Card> Hand { get; private set; }

    public Player(string name)
    {
        Name = name;
        Hand = new List<Card>();
    }

    public void AddCard(Card card)
    {
        if (card != null)
        {
            Hand.Add(card);
        }
    }
}
