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

    private Hand aiHand;
    private int currentBid;
    private Character _thisCharacter = Character.DEATH;

    void Start(){
        GameManager.OnPhaseChanged += AIManagerOnPhaseChanged;
    }

    private void AIManagerOnPhaseChanged(Phase phase){
        if(phase == Phase.AITURN){
            HandleAITurn();
        }
    }

    private void HandleAITurn(){
        PlayCard();
        // if there is dialogue to play, might want to activate it here
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
        float bidEstimate = 0.0f;
        
        // add a bid for every !! non Spades !! suit Death is low in
        for(int i = 0; i< 4; i++){
            Suit currSuit = Deck.intToSuit[i];
            int numCurrSuit = aiHand.NumOfSuit(currSuit);
            if( numCurrSuit <= 2 && i != 0){ // might want to refine this later
                bidEstimate += 1.0f; 
            }

            // add a bid for every ace
            if(aiHand.HasValue(currSuit, 14)){
                bidEstimate += 1.0f;
            }

            // add a half bid for every King
            if(aiHand.HasValue(currSuit, 13)){
                bidEstimate += 0.5f;
            }
        }
        
        currentBid = (int)bidEstimate; // need to make sure this rounds correctly
        return currentBid; 
    }

    // Function to Calculate Logic for what Death plays based on Death's hand
    public void PlayCard()
    {
        // !Work in Progress!
    }

    // unsubscribe from events when destroyed to prevent errors
    void OnDestroy(){
        GameManager.OnPhaseChanged -= AIManagerOnPhaseChanged;
    }
}
