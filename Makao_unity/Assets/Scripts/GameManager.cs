
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Deck deck;
    public List<Player> players;
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private CardImages cardImages;
    [SerializeField] private Transform[] handPanel;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        deck = new Deck();
        deck.Shuffle();

        players = new List<Player>();

        for (int i = 0; i < 3; i++)
        {
            players.Add(new Player("Gracz " + (i + 1)));
        }

        // Rozdajemy po 5 kart ka¿demu graczowi
        for (int round = 0; round < 5; round++)
        {
            foreach (Player player in players)
            {
                player.AddCard(deck.DrawCard());
            }
        }

        Debug.Log("Pozosta³o kart: " + deck.Cards.Count);

        foreach (Player player in players)
        {
            Debug.Log(player.Name + " ma " +
                      player.Hand.Count + " kart.");

            foreach (Card card in player.Hand)
            {
                Debug.Log(card.ToString());
            }
        }
    for (int i = 0; i < players.Count; i++)
    {
        foreach (Card card in players[i].Hand)
        {
            CardView view = Instantiate(cardPrefab, handPanel[i]);
            view.Setup(card, cardImages);
        }
    }
}
}
