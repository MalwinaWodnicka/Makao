using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardImageEntry
{
    public CardSuit suit;
    public CardRank rank;
    public Sprite sprite;
}

public class CardImages : MonoBehaviour
{
    [SerializeField] private CardImageEntry[] entries;
    public Sprite GetSprite(Card card)
    {
        foreach (CardImageEntry entry in entries)
        {
            if (entry.suit == card.Suit && entry.rank == card.Rank)
                return entry.sprite;
        }
        Debug.LogWarning($"Brakuje sprite dla:  {card}");
        return null;
    }
}
