using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] 
    GameObject deckObj; // maybe we do it this way?

    private List<Card> cards;
    public static Suit[] intToSuit = { Suit.SPADE, Suit.DIAMOND, Suit.CLUB, Suit.HEART};
    private const int numCards = 52;
    
    
    void Awake()
    {
        // Populate the standard 52 Card deck
        cards = new List<Card>();
        int c = 0;
        for(int i  = 2; i <= 14; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                cards.Add(new Card(i, intToSuit[j]));
                c++;
            }
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

    // return the "top" card of the deck, and "discard" it (put it at the very end)
    public Card DrawCard()
    {
        Card toReturn = cards[0];
        cards.RemoveAt(0);
        cards.Add(toReturn);
        return toReturn;
    }

    // Discard the "top" card of the deck
    public void DiscardCard()
    {
        Card toDiscard = cards[0];
        cards.RemoveAt(0);
        cards.Add(toDiscard);
    }
}
