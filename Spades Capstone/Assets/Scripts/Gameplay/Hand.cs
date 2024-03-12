using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
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
        Card greatest = cardsInHand[suit][0];
        foreach(Card c in cardsInHand[suit]){
            if(c.val >= greatest.val){
                greatest = c;
            }
        }
        return greatest;
    }

    // return the lowest value card of a given suit
    public Card GetLowest(Suit suit){
        Card least = cardsInHand[suit][0];
        foreach(Card c in cardsInHand[suit]){
            if(c.val < least.val){
                least = c;
            }
        }
        return least;
    }

    // Given a Card, return the next Highest Value of the same Suit from current hand 
    public Card GetNextHighest(Suit suit, int lowerVal)
    {
        Card nextHighestCard = GetHighest(suit);
        List<Card> allHigherCards = new List<Card>();

        foreach(Card card in cardsInHand[suit]) {
            if (card.val > lowerVal) { allHigherCards.Add(card); }
        }

        int currentNextHighestValue = 15;
        foreach (Card card in allHigherCards)
        {
            if (card.val < currentNextHighestValue) { nextHighestCard = card; }
        }

        return nextHighestCard;
    }

    public Card GetWorst(){
        return GetLowest(GetFewestStillInHand());
    }

    // Should return the suit that has the lowest count and greater than 0
    public Suit GetFewestStillInHand(){ // BUGGED!!
        int numHearts = NumOfSuit(Suit.HEART);
        int numClubs = NumOfSuit(Suit.CLUB);
        int numDiamonds = NumOfSuit(Suit.DIAMOND);

        
        
        if (numHearts != 0 && numHearts <= numClubs && numHearts <= numDiamonds) { return Suit.HEART; }
        else if (numClubs != 0 && numClubs <= numHearts && numClubs <= numDiamonds) { return Suit.CLUB; }
        else if (numDiamonds != 0)
        {
            return Suit.DIAMOND;
        }
        else if(numClubs != 0)
        {
            return Suit.CLUB;
        }
        return Suit.HEART;

    }

    // get the number of cards of a particular suit 
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
                return true;
            }
        }
        return false;
    }

    public bool HasNonSpades()
    {
        return (NumOfSuit(Suit.DIAMOND) + NumOfSuit(Suit.CLUB) + NumOfSuit(Suit.HEART)) == 0;
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



