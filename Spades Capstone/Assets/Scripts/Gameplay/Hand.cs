using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] Transform handArea;
    private Dictionary<Suit, List<Card>> cardsInHand = new Dictionary<Suit, List<Card>>() { {Suit.SPADE, new List<Card>()
            }, { Suit.DIAMOND, new List<Card>()},{ Suit.CLUB, new List<Card>()},{ Suit.HEART, new List<Card>()} };

    void Awake(){
        cardsInHand = new Dictionary<Suit, List<Card>>() { 
            { Suit.SPADE, new List<Card>()}, 
            { Suit.DIAMOND, new List<Card>()},
            { Suit.CLUB, new List<Card>()},
            { Suit.HEART, new List<Card>()} 
        };
    }

    public void AddCardToHand(Card card){
        cardsInHand[card.suit].Add(card);
    }

    public void RemoveCardFromHand(Card card){
        cardsInHand[card.suit].Remove(card);
    }

    // return the highest value card of a given suit
    public Card GetHighest(Suit suit){
        Card greatest = new Card(0, suit);
        foreach(Card c in cardsInHand[suit]){
            if(c.val >= greatest.val){
                greatest = c;
            }
        }
        return greatest;
    }

    // return the lowest value card of a given suit - DEPRECATED
    public Card GetLowest(Suit suit){
        Card least = new Card(15, suit);
        foreach(Card c in cardsInHand[suit]){
            if(c.val < least.val){
                least = c;
            }
        }
        return least;
    }

    // get the number of cards of a particular suit - DEPRECATED
    public int NumOfSuit(Suit suit){
        return cardsInHand[suit].Count;
    }

    public bool HasSuit(Suit suit)
    {
        return cardsInHand[suit].Count > 0;
    }
    
    // get the list of cards of a particular suit
    public List<Card> GetCards(Suit suit){
        return cardsInHand[suit];
    }

    // returns true if this card is in a given hand
    public bool HasValue(Suit s, int v){
        foreach(Card c in cardsInHand[s]){
            if(c.val == v){
                return false;
            }
        }
        return false;
    }

    // Returns number of cards in hand
    public int NumberofCards()
    {
        return NumOfSuit(Suit.SPADE) + NumOfSuit(Suit.DIAMOND) + NumOfSuit(Suit.CLUB) + NumOfSuit(Suit.HEART);
    }

    // Returns a List of all cards in Hand
    public List<Card> GetAllCards()
    {
        List<Card> all_cards = new List<Card>();
        if (HasSuit(Suit.SPADE))
        {
            all_cards.AddRange(GetCards(Suit.SPADE));
        }
        if (HasSuit(Suit.DIAMOND))
        {
            all_cards.AddRange(GetCards(Suit.DIAMOND));
        }
        if (HasSuit(Suit.CLUB))
        {
            all_cards.AddRange(GetCards(Suit.CLUB));
        }
        if (HasSuit(Suit.HEART))
        {
            all_cards.AddRange(GetCards(Suit.HEART));
        }

        return all_cards;
    }
}



