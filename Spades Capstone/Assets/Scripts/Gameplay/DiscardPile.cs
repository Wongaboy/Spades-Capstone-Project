using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    private Dictionary<Suit, List<Card>> cardsInDiscardPile = new Dictionary<Suit, List<Card>>() { {Suit.SPADE, new List<Card>()
            }, { Suit.DIAMOND, new List<Card>()},{ Suit.CLUB, new List<Card>()},{ Suit.HEART, new List<Card>()} };

    void Awake()
    {
        cardsInDiscardPile = new Dictionary<Suit, List<Card>>() {
            { Suit.SPADE, new List<Card>()},
            { Suit.DIAMOND, new List<Card>()},
            { Suit.CLUB, new List<Card>()},
            { Suit.HEART, new List<Card>()}
        };
    }

    public void AddCardToDiscardPile(Card card)
    {
        cardsInDiscardPile[card.suit].Add(card);
    }

    public void RemoveCardFromDiscardPile(Card card)
    {
        cardsInDiscardPile[card.suit].Remove(card);
    }

    // Return the highest value card of a given suit
    public Card GetHighest(Suit suit)
    {
        Card greatest = cardsInDiscardPile[suit][0];
        foreach (Card c in cardsInDiscardPile[suit])
        {
            if (c.val >= greatest.val)
            {
                greatest = c;
            }
        }
        return greatest;
    }

    public void ClearDiscardPile()
    {
        cardsInDiscardPile[Suit.CLUB].Clear();
        cardsInDiscardPile[Suit.DIAMOND].Clear();
        cardsInDiscardPile[Suit.HEART].Clear();
        cardsInDiscardPile[Suit.SPADE].Clear();
    }
}
