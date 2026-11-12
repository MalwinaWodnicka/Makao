
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

        if (currentPlayerIndex >= players.Count)
        {
            currentPlayerIndex = 0;
        }
        else if (currentPlayerIndex < 0)
        {
            currentPlayerIndex = players.Count - 1;
        }
        Debug.Log($"Tura gracza: {players[currentPlayerIndex].Name}");
    }

    public bool IsMoveLegal(Card cardToPlay)
    {
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
                break;
            case CardRank.Four:
                break;
            case CardRank.Jack:
                break;
            case CardRank.Queen:
                break;
            case CardRank.King:
                if (card.Suit == CardSuit.Hearts)
                {
                    break;
                }
                if (card.Suit == CardSuit.Spades)
                {
                    break;
                }
                break;

            case CardRank.Ace:
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
