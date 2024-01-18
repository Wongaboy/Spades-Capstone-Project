using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandUI : MonoBehaviour
{
    [SerializeField]
    private Transform[] cardPositions;
    [SerializeField]
    private Transform tableSpot;
    [SerializeField]
    private Character handOwner;
    private List<Card> cardObjs;
    private static int[] draftIndexOrder = { 6, 5, 7, 4, 8, 3, 9, 2, 10, 1, 11, 0, 12 };

    public void Start()
    {
        cardObjs = new List<Card>();
        if(handOwner != Character.DEATH){
            CardInteraction.OnCardFinMove += SortHand;
        }
    }


    
    // moves the card into the player's hand in physical space
    public void ShowCard(Card card)
    {
        int openSlot = draftIndexOrder[cardObjs.Count];

        cardObjs.Add(card);
        card.MoveToLocation(cardPositions[openSlot].position, cardPositions[openSlot].rotation);
    }

    // moves the card onto the table and locks it out of being interacted with
    public void ShowCardPlayed(Card card){
        card.MoveToLocation(tableSpot.position, tableSpot.rotation, true);
        card.SetInteractable(false);
        cardObjs.Remove(card);
        // yield return new WaitForSeconds(1); - this might work later, but causes too many issues rn to figure out
        // SnapToPosition();
    }

    // alters the card interaction for EVERY card in hand
    public void AlterCardInteraction(bool interactable)
    {
        foreach(Card c in cardObjs)
        {
            c.SetInteractable(interactable);
        }
    }

    public void SortHand(Phase currPhase)
    {
        if(currPhase == Phase.PLAYERDRAFT || currPhase == Phase.PLAYERTURN)
        {
            SnapToPosition();
        }
    }

    
    private void SnapToPosition()
    {
        if (cardObjs.Count == 0) { return; } 

        cardObjs.Sort(new CardPosComparer());
        int index = 6 - (cardObjs.Count / 2);
        for (int c = 0; c < cardObjs.Count; c++) 
        {
            if (cardObjs[c].gameObject.transform.position != cardPositions[index + c].position && (cardObjs[c].gameObject.transform.position.y < 1.65))
            {
                cardObjs[c].MoveToLocation(cardPositions[index + c].position, cardPositions[index + c].rotation);
            }
        }
    }
    
    // Card Comparer used to sort the cards by how far left they are and then move them all to the correct spot
    private class CardPosComparer: Comparer<Card>
    {
        public override int Compare(Card a, Card b)
        {
            float xa = a.gameObject.transform.position.x;
            float xb = b.gameObject.transform.position.x;
            if (xa < xb) { return -1; }
            else if (xa > xb) { return 1; }
            return 0;
        }
    }

    /*
    public void SwitchOrder(Phase currPhase)
    {
        Transform[] newOrder = new Transform[cardPositions.Length];
        if(currPhase == Phase.PLAYERBID) // sort the transforms from left to right
        {
            newOrder[0] = cardPositions[11];
            newOrder[1] = cardPositions[9];
            newOrder[2] = cardPositions[7];
            newOrder[3] = cardPositions[5];
            newOrder[4] = cardPositions[3];
            newOrder[5] = cardPositions[1];
            newOrder[6] = cardPositions[0];
            newOrder[7] = cardPositions[2];
            newOrder[8] = cardPositions[4];
            newOrder[9] = cardPositions[6];
            newOrder[10] = cardPositions[8];
            newOrder[11] = cardPositions[10];
        }
        else { // sort the transforms from center out (default)
            newOrder[0] = cardPositions[6];
            newOrder[1] = cardPositions[5];
            newOrder[2] = cardPositions[7];
            newOrder[3] = cardPositions[4];
            newOrder[4] = cardPositions[8];
            newOrder[5] = cardPositions[3];
            newOrder[6] = cardPositions[9];
            newOrder[7] = cardPositions[2];
            newOrder[8] = cardPositions[10];
            newOrder[9] = cardPositions[1];
            newOrder[10] = cardPositions[11];
            newOrder[11] = cardPositions[0];
        }
        cardPositions = newOrder;
    }
    */
}
