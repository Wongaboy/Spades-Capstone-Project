using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private Dictionary<Suit, List<Card>> cardsInHand;

    void Awake(){
        cardsInHand = new Dictionary<Suit, List<Card>>();
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

    // return the lowest value card of a given suit
    public Card GetLowest(Suit suit){
        Card least = new Card(0, suit);
        foreach(Card c in cardsInHand[suit]){
            if(c.val < least.val){
                least = c;
            }
        }
        return least;
    }

    public int NumOfSuit(Suit suit){
        return cardsInHand[suit].Count;
    }
    
    public List<Card> GetCards(Suit suit){
        return cardsInHand[suit];
    }

    public bool HasValue(Suit s, int v){
        foreach(Card c in cardsInHand[s]){
            if(c.val == v){
                return false;
            }
        }
        return false;
    }
}


