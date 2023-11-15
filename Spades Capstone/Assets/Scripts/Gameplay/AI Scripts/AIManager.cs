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

    [SerializeField] Hand aiHand;
    private int currentBid;
    private Character thisCharacter = Character.DEATH;
    // Tracks if Character Leads
    private bool isLead = true;

    // Start is called before the first frame update
    void Start(){
        GameManager.OnPhaseChanged += AIManagerOnPhaseChanged;
    }

    // React to Phase changes for turn based gameplayt
    private void AIManagerOnPhaseChanged(Phase phase)
    {
        if (phase == Phase.AITURN)
        {
            HandleAITurn();
        }
        else if (phase == Phase.AIDRAFT)
        {
            // Decides to Keep or Discard & then waits to switch Turn/Phase
            MakeDraftDecision(GameManager.Instance.DrawCard());
            StartCoroutine(WaitTimeDraft());
        }
    }
    #region "AI logic"

    // Function to Decide if Death keeps or dumps the drawn card
    private bool MakeDraftDecision(Card card)
    {
        // !Work In Progress!

        // Temporary Formula:
        // If SPADE -> KEEP
        // Else -> DISCARD

        if (card.suit == Suit.SPADE)
        {
            aiHand.AddCardToHand(card);
            GameManager.Instance.DiscardCard();
        }
        else
        {
            aiHand.AddCardToHand(GameManager.Instance.DrawCard());
        }

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

        // Anthony Test Code just so Bids are not very low
        bidEstimate += aiHand.NumOfSuit(Suit.SPADE);
        currentBid = (int)bidEstimate; // need to make sure this rounds correctly

        return currentBid; 
    }

    // Decide Card when AI plays 1st
    private Card ChooseCardToLead()
    {
        // !Work in Progress!
        return aiHand.GetAllCards()[0]; // INCOMPLETE
    }

    // Decide Card when AI plays 2nd
    private Card ChooseCardToFollow(Card playerCard)
    {
        // !Work in Progress!
        return aiHand.GetAllCards()[0]; // INCOMPLETE
    }

    #endregion

    #region "Public Helper Functions"

    // Changes Lead Boolean (bool is for Trick Lead, NOT Draft/Bid Lead)
    public void ChangeInternalLead(bool isCurrLead)
    {
        isLead = isCurrLead;
    }

    #endregion

    #region "Private Helper Functions"

    private void HandleAITurn()
    {
        // AI Determines what card to play and Plays It
        // !Work in Progress!
        PlayCard();

        // Wait to Switch Phases
        StartCoroutine(WaitTimePlayCard());

    }

    private void PlayCard()
    {
        Card toPlay;
        if (isLead)
        {
            // !Work in Progress!
            toPlay = ChooseCardToLead();
        }
        else
        {
            // !Work in Progress!
            toPlay = ChooseCardToFollow(GameManager.Instance.playerCard);
        }
        // Feed GameManager Played Card
        GameManager.Instance.aiCard = toPlay;
        // Update UI for AI's Card
        TurnUI.Instance.UpdateAICardInfo(toPlay); // SOON TO BE DEPRECATED
        // Remove Card from AI's Hand
        aiHand.RemoveCardFromHand(toPlay);
    }

    #endregion

    #region "IEnumerators"

    // Waits 1 second after Draft to Switch Phases
    private IEnumerator WaitTimeDraft()
    {      
        yield return new WaitForSeconds(1);

        GameManager.Instance.ChangePhase(Phase.PLAYERDRAFT);
    }

    // Waits 1 second after playing AI card to Switch Phases
    private IEnumerator WaitTimePlayCard()
    {
        yield return new WaitForSeconds(1);

        // if there is dialogue to play, might want to activate it here
        if (isLead == true)
        {
            GameManager.Instance.ChangePhase(Phase.PLAYERTURN);
        }
        else
        {
            GameManager.Instance.ChangePhase(Phase.ENDOFTRICK);
        }
    }

    #endregion

    #region "Debug Functions"
    // Debug Function to show amount of each suits in hand
    public void Debug_ShowHand()
    {
        Debug.Log(aiHand.NumOfSuit(Suit.DIAMOND));
        Debug.Log(aiHand.NumOfSuit(Suit.SPADE));
        Debug.Log(aiHand.NumOfSuit(Suit.CLUB));
        Debug.Log(aiHand.NumOfSuit(Suit.HEART));
    }
    #endregion

    // Unsubscribe from events when destroyed to prevent errors
    void OnDestroy(){
        GameManager.OnPhaseChanged -= AIManagerOnPhaseChanged;
    }
}
