using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    public Card[] cardsInDeck;

    private List<Card> cards;
    public static Suit[] intToSuit = { Suit.SPADE, Suit.DIAMOND, Suit.CLUB, Suit.HEART};
    private const int numCards = 52;
    
    
    void Awake()
    {
        cards = new List<Card>();
        foreach(Card card in cardsInDeck)
        {
            cards.Add(card);
        }
    }

    // Randomize the order of cards in the deck
    public void Shuffle()
    {
        System.Random shuffler = new System.Random();
        int currCard = cards.Count;
        while (currCard > 1)
        {
            int nextCard = shuffler.Next(currCard--);
            Card temp = cards[currCard];
            cards[currCard] = cards[nextCard];
            cards[nextCard] = temp;
        }
    }

    // return the "top" card of the deck, and "discard" it (put at the Bottom of Deck, end of list)
    public Card DrawCard()
    {
        Card toReturn = cards[0];
        cards.RemoveAt(0);
        cards.Add(toReturn);
        return toReturn;
    }

    // Discard the "top" card of the deck (Gets put at the Bottom of Deck, end of list)
    public void DiscardCard()
    {
        Card toDiscard = cards[0];
        cards.RemoveAt(0);
        cards.Add(toDiscard);
    }
}
