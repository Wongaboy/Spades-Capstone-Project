using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] 
    GameObject deckObj;

    private Card[] cards;
    private Suit[] intToSuit = { Suit.SPADE, Suit.DIAMOND, Suit.CLUB, Suit.HEART};
    private const int numCards = 52;
    
    
    void Awake()
    {
        // Populate the standard 52 Card deck
        cards = new Card[numCards];
        int c = 0;
        for(int i  = 1; i <= 13; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                cards[c] = new Card(i, intToSuit[j]);
                c++;
            }
        }
    }

    // Randomize the order of cards in the deck
    public void Shuffle()
    {
        System.Random shuffler = new System.Random();
        int currCard = cards.Length;
        while (numCards > 1)
        {
            int nextCard = shuffler.Next(currCard--);
            Card temp = cards[currCard];
            cards[currCard] = cards[nextCard];
            cards[nextCard] = temp;
        }
    }
}
