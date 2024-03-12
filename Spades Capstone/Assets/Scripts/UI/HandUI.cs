using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    public static event Action<Character, Card> cardPlayed;

    private bool allowInteraction = false;

    public void Start()
    {
        cardObjs = new List<Card>();
        if(handOwner != Character.DEATH){
            CardInteraction.OnCardFinMove += SortHand;
            allowInteraction = true;
        }
    }


    
    // moves the card into the player's hand in physical space
    public void ShowCard(Card card)
    {
        int openSlot = draftIndexOrder[cardObjs.Count];

        cardObjs.Add(card);
        card.MoveToLocation(cardPositions[openSlot].position, cardPositions[openSlot].rotation, false, allowInteraction);
    }

    // moves the card onto the table and locks it out of being interacted with
    public void ShowCardPlayed(Card card){
        card.MoveToLocation(tableSpot.position, tableSpot.rotation, true, false);
        //card.SetInteractable(false); - now redundant
        cardObjs.Remove(card);
        cardPlayed.Invoke(handOwner, card);
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
        if(currPhase == Phase.PLAYERDRAFT || currPhase == Phase.PLAYERTURN || currPhase == Phase.PLAYERBID)
        {
            if (currPhase != Phase.PLAYERDRAFT) { SoundFXManager.Instance.PlayCardSFX(gameObject.transform, .1f); }
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
            // should optimize this by storing a reference on the card whether or not its valid when checking for vfx activation
            if ((cardObjs[c].gameObject.transform.position != cardPositions[index + c].position && cardObjs[c].gameObject.transform.position.y < 1.65))
            {
                cardObjs[c].MoveToLocation(cardPositions[index + c].position, cardPositions[index + c].rotation, false, allowInteraction);
            }
        }
    }

    public void ReturnCardToHand(Card card)
    {
        SoundFXManager.Instance.PlayCardSFX(gameObject.transform, .1f);
        int index = 6 - (cardObjs.Count / 2);
        for (int c=0; c < cardObjs.Count; c++)
        {
            if (cardObjs[c] == card)
            {
                cardObjs[c].MoveToLocation(cardPositions[index + c].position, cardPositions[index + c].rotation, false, allowInteraction);
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
}
