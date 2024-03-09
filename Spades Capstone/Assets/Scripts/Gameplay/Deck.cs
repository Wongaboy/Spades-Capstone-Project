using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    public Card[] cardsInDeck;
    [SerializeField]
    private Transform deckSpot;

    private List<Card> cards;
    public static Suit[] intToSuit = { Suit.SPADE, Suit.DIAMOND, Suit.CLUB, Suit.HEART};
    
    
    void Awake()
    {
        cards = new List<Card>();
        foreach(Card card in cardsInDeck)
        {
            cards.Add(card);
        }
    }

    // Randomize the order of cards in the deck, now with a cool animation
    public IEnumerator Shuffle()
    {
        System.Random shuffler = new();
        int currCard = cards.Count;
        while (currCard > 1)
        {
            int nextCard = shuffler.Next(currCard--);
            Card temp = cards[currCard];
            cards[currCard] = cards[nextCard];
            cards[nextCard] = temp;
        }
        // Solution? to card falling through table
        yield return new WaitForSeconds(1f);
        for(int c = 51; c >= 0; c--)
        {
            cards[c].MoveToLocation(deckSpot.transform.position, deckSpot.transform.rotation, true);
            yield return new WaitForSeconds(.1f);
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

    public List<Card> GetInteractableCards()
    {
        List<Card> allInteractableCards = new List<Card>();

        foreach(Card card in cards)
        {
            if (card.IsInteractable()) { allInteractableCards.Add(card); }
        }

        return allInteractableCards;
    }
}
