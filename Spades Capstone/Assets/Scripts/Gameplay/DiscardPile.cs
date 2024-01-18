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

    // Return true/false if AI can be a card in discard? -- NOT FINISHED
    public bool CanBeatDiscard()
    {
        return false;
    }
    // Return card in Discard that can be beat by given card -- NOT FINISHED
    public Card GetBeatableCard(Card card)
    {
        // If card to beat is a SPADE
        // Else if there is a higher
        return card;
    }

    public void ClearDiscardPile()
    {
        cardsInDiscardPile[Suit.CLUB].Clear();
        cardsInDiscardPile[Suit.DIAMOND].Clear();
        cardsInDiscardPile[Suit.HEART].Clear();
        cardsInDiscardPile[Suit.SPADE].Clear();
    }
}
