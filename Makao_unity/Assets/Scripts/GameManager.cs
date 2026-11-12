
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Deck deck;
    public List<Player> players;
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private CardImages cardImages;
    [SerializeField] private Transform[] handPanel;
    [SerializeField] private Transform discardPilePanel;

    public int currentPlayerIndex = 0;
    public int playDirection = 1;
    public Card topCard;
    public int numberCardsToDraw = 0;
    private int turnToSkip = 0;
    public CardRank? requestedRank = null;
    public CardSuit? requestedSuit = null;

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

        topCard = deck.DrawCard();
        Debug.Log("Start gry.");
        UpdateUI();
    }

    public void NextPlayer()
    {
        currentPlayerIndex += playDirection;
        WrapPlayerIndex();

        while (turnToSkip > 0 && numberCardsToDraw == 0)
        {
            turnToSkip--;
            currentPlayerIndex += playDirection;
            WrapPlayerIndex();
        }
        Debug.Log($"Tura gracza: {players[currentPlayerIndex].Name}");
    }

    private void WrapPlayerIndex()
    {
        if (currentPlayerIndex >= players.Count)
        {
            currentPlayerIndex = 0;
        }
        else if (currentPlayerIndex < 0)
        {
            currentPlayerIndex = players.Count - 1;
        }
    }

    public bool IsMoveLegal(Card cardToPlay)
    {
        // Dobieranie
        if (numberCardsToDraw > 0)
        {
            if (cardToPlay.Rank == topCard.Rank)
            {
                return true;
            }

            if (cardToPlay.Suit == topCard.Suit)
            {
                if (cardToPlay.Rank == CardRank.Two || cardToPlay.Rank == CardRank.Three)
                {
                    return true;
                }
                if (cardToPlay.Rank == CardRank.King && (cardToPlay.Suit == CardSuit.Hearts || cardToPlay.Suit == CardSuit.Spades))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        // Pominiêcie tury
        if (turnToSkip > 0)
        {
            return cardToPlay.Rank == CardRank.Four;
        }

        // ¯¹danie Jack'a
        if (requestedRank.HasValue)
        {
            return cardToPlay.Rank == requestedRank.Value || cardToPlay.Rank == CardRank.Jack || cardToPlay.Rank == CardRank.Queen;
        }

        // ¯¹danie Ace
        if (requestedSuit.HasValue)
        {
            return cardToPlay.Suit == requestedSuit.Value || cardToPlay.Rank == CardRank.Ace || cardToPlay.Rank == CardRank.Queen;
        }

        // Queen
        if (cardToPlay.Rank == CardRank.Queen || topCard.Rank == CardRank.Queen)
        {
            return true;
        }

        // Podstawowe zasady
        if (cardToPlay.Suit == topCard.Suit || cardToPlay.Rank == topCard.Rank)
        {
            return true;
        } 

        return false;
    }

    public void TryPlayCard(Player player, Card card)
    {
        Debug.Log("Klikniêto przycisk karty: " + card.Rank + " of " + card.Suit);
        if (players[currentPlayerIndex] != player)
        {
            Debug.Log("Tura z³ego gracza");
            return;
        }

        if (IsMoveLegal(card))
        {
            player.RemoveCard(card);
            topCard = card;
            Debug.Log($"{player.Name} zagra³ {card}");

            if (card.Rank != CardRank.Jack)
            {
                requestedRank = null;
            }
           
            if (card.Rank != CardRank.Ace)
            {
                requestedSuit = null;
            }

            ApplySpecialCardEffect(card);
            NextPlayer();
            UpdateUI();
        }
        else
        {
            Debug.Log("Nielegalny ruch");
        }
    }

    public void DrawCard(Player player)
    {
        if (players[currentPlayerIndex] != player)
        {
            return;
        }

        int drawCount = numberCardsToDraw > 0 ? numberCardsToDraw : 1;

        for (int i = 0; i < drawCount; i++)
        {
            Card drawnCard = deck.DrawCard();
            if (drawnCard != null)
            {
                player.AddCard(drawnCard);
            }
        }

        Debug.Log($"{player.Name} dobra³ {drawCount} kart");
        numberCardsToDraw = 0;
        requestedSuit = null;
        requestedRank = null;

        NextPlayer();
        UpdateUI();
    }

    private void ApplySpecialCardEffect(Card card)
    {
        switch (card.Rank)
        {
            case CardRank.Two:
                numberCardsToDraw += 2;
                break;
            case CardRank.Three:
                numberCardsToDraw += 3;
                break;
            case CardRank.Four:
                turnToSkip += 1;
                break;
            case CardRank.Jack:
                CardRank[] possibleRanks = { CardRank.Five, CardRank.Six, CardRank.Seven, CardRank.Eight, CardRank.Nine, CardRank.Ten, };
                requestedRank = possibleRanks[Random.Range(0, possibleRanks.Length)];
                Debug.Log($"Zosta³ zagrany Jack. ¯¹dana karta: {requestedRank.Value}");
                break;
            case CardRank.King:
                if (card.Suit == CardSuit.Hearts)
                {
                    numberCardsToDraw += 5;
                }
                else if (card.Suit == CardSuit.Spades)
                {
                    numberCardsToDraw += 5;
                    playDirection *= -1;
                }
                break;

            case CardRank.Ace:
                requestedSuit = (CardSuit)Random.Range(0, 4);
                topCard = new Card(requestedSuit.Value, CardRank.Ace);
                break;
        }
    }

    private void UpdateUI()
    {

        foreach (Transform child in discardPilePanel)
        {
            Destroy(child.gameObject);
        }

        if (topCard != null)
        {
            CardView topView = Instantiate(cardPrefab, discardPilePanel);
            topView.Setup(topCard, cardImages);
            Button topBtn = topView.GetComponent<Button>();
            if (topBtn != null)
            {
                topBtn.interactable = false;
            }
        }

        for (int i = 0; i < handPanel.Length; i++)
        {
            foreach (Transform child in handPanel[i])
            {
                Destroy(child.gameObject);
            }

            if (i < players.Count)
            {
                foreach (Card card in players[i].Hand)
                {
                    CardView view = Instantiate(cardPrefab, handPanel[i]);
                    view.Setup(card, cardImages);
                    Button btn = view.GetComponent<Button>();
                    if (btn != null)
                    {
                        Player owner = players[i];
                        Card cardToPlay = card;

                        btn.onClick.AddListener(() => TryPlayCard(owner, cardToPlay));
                    }
                }
            }
        }
    }

    public void OnDrawCardButtonClicked()
    {
        DrawCard(players[currentPlayerIndex]);
    }
}
