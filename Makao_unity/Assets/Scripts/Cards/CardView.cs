using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image cardImage;

    public void Setup(Card card, CardImages images)
    {
        cardImage.sprite = images.GetSprite(card);
        cardImage.preserveAspect = true;
    }
}
