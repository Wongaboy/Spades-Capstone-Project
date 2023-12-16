using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField]
    private Transform[] cardPositions;
    [SerializeField]
    private Transform tableSpot;

    private List<Card> cardObjs;

    public void Start()
    {
        cardObjs = new List<Card>();
    }
    
    public bool ShowCard(Card card)
    {
        int openSlot = cardObjs.Count;
        // if there are already 13 cards in Hand, return false
        if(openSlot == 13) { return false; }

        cardObjs.Add(card);
        card.MoveToLocation(cardPositions[openSlot].position, cardPositions[openSlot].rotation);

        return true;
    }

    public void ShowCardPlayed(Card card){
        card.MoveToLocation(tableSpot.position, tableSpot.rotation, true);
        card.SetInteractable(false);
        cardObjs.Remove(card);
    }

    public void AlterCardInteraction(bool interactable)
    {
        foreach(Card c in cardObjs)
        {
            c.SetInteractable(interactable);
        }
    }
    
}
