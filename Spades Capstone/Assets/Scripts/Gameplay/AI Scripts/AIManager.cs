using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    #region "Singleton"
    private static AIManager _instance;

    public static AIManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    private Card[] Death_Hand;
    private int current_Death_Bid;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function to Decide if Death keeps or dumps the drawn card
    public bool DraftDecision(Card card)
    {
        // !Work In Progress!

        // Temporary Formula:
        // If SPADE -> KEEP
        // Else If Above 10 (i.e Jacks, Queens, Kings, Ace) -> KEEP
        // Else -> DISCARD
        return true;
    }
    // Function to calculate Death's Bid amount based on Death's Hand
    public int GetBid()
    {
        // !Work In Progress!

        // Temporary Formula:
        // Number of Spades + Number of Card Above 10 (i.e Jacks, Queens, Kings, Ace) + a little up/down?

        int new_bid = 0;
        current_Death_Bid = new_bid;
        return current_Death_Bid; // Temp Value
    }

    // Function to Calculate Logic for what Death plays based on Death's hand
    public void PlayCard()
    {
        // !Work in Progress!
    }
}
