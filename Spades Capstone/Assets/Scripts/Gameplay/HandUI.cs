using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField]
    private Transform[] cardPositions;
    [SerializeField]
    private Hand myHand;
    private Transform tableSpot;

    private List<GameObject> cardObjs;

    public void Start()
    {
        cardObjs = new List<GameObject>();
    }
    
    public bool ShowCard(GameObject card)
    {
        int openSlot = cardObjs.Count;
        
        // if there are already 13 cards in Hand, return false
        if(openSlot == 13) { return false; }

        cardObjs.Add(card);
        card.transform.position = cardPositions[openSlot].position;

        return true;
    }

    public bool ShowCardPlayed(GameObject card){
        if(!cardObjs.Contains(card)){return false;}
        else{
            // move card to spot on table 
            card.transform.position = tableSpot.position; // will replace this with smooth anim to later
            cardObjs.Remove(card);
        }
    }
    
}
