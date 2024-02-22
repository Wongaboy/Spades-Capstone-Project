using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Enums for Cheats
public enum AICheatPhase { NoCheats, CheatPhaseOne, CheatPhaseTwo }
public enum CheatName { IgnorePenalty, ChangeBid, RandomizePlayerCardValue, ChoosePlayerCard, AddValueFromDiscard }

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

    #region "Class Variables"
    [SerializeField] Hand aiHand;
    [SerializeField] HandUI aiHandUI;
    [SerializeField] Transform displaySpot;
    [SerializeField] TMP_Text cardDisplay; // for testing only, will remove once we have stuff figured out
    // Tells if AI is leading the Trick
    private bool isTrickLead = true;

    // Cheat Tracker Variables
    private int cheatsLeftPhaseOne = 1;
    private int cheatsLeftPhaseTwo = 2;

    private AICheatPhase currentCheatPhase = AICheatPhase.NoCheats;

    // AI Cheat Database
    private Dictionary<CheatName, bool> cheatSetOneDatabase = new Dictionary<CheatName, bool>()
    {
        { CheatName.IgnorePenalty, true },
        { CheatName.ChangeBid, true }
    };

    private Dictionary<CheatName, bool> cheatSetTwoDatabase = new Dictionary<CheatName, bool>()
    {
        { CheatName.RandomizePlayerCardValue, true },
        { CheatName.ChoosePlayerCard, true },
        { CheatName.AddValueFromDiscard, true }
    };
    #endregion

    // Start is called before the first frame update
    void Start(){
        // Subscribe to Event(OnPhaseChanged)
        GameManager.OnPhaseChanged += AIManagerOnPhaseChanged;
    }

    // OnDestroy is called when script/object is destroyed
    void OnDestroy(){
        // Unsubscribe from Event(OnPhaseChanged) when destroyed to prevent errors
        GameManager.OnPhaseChanged -= AIManagerOnPhaseChanged;
    }

    // React to OnPhaseChange & Perform necessary actions based on Current Phase
    private void AIManagerOnPhaseChanged(Phase phase)
    {
        if (phase == Phase.AITURN)
        {
            StartCoroutine(HandleAITurn());
            // cardDisplay.gameObject.SetActive(true); deprecated
        }
        else if (phase == Phase.AIDRAFT)
        {
            // Decides to Keep or Discard & then waits to switch Turn/Phase
            // cardDisplay.gameObject.SetActive(false); deprecated
            StartCoroutine(MakeDraftDecision(GameManager.Instance.DrawCard()));
        }

    }
    #region "AI logic"

    // Function to Calculate AI Draft Decision & Draft Card
    private IEnumerator MakeDraftDecision(Card card)
    {
        ConsiderCard(card);
        yield return new WaitForSeconds(1);
        // Temporary Formula:
        // If SPADE or a (f)ace card -> KEEP; ELSE -> DISCARD
        if (card.suit == Suit.SPADE || card.val > 10)
        {
            DraftCard(card);
            GameManager.Instance.DiscardCardFromDeck();
        }
        else
        { 
            GameManager.Instance.DiscardCardFromHand(card);
            DraftCard(GameManager.Instance.DrawCard());
        }

        GameManager.Instance.ChangePhase(Phase.PLAYERDRAFT);
    }

    // Function to calculate AI's Bid amount based on Death's Hand
    public int GetBid()
    {
        // !Work In Progress!
        float bidEstimate = 0.0f;
        
        // Add a bid for every !! non Spades !! suit Death is low in
        for(int i = 0; i < 4; i++){
            Suit currentSuit = Deck.intToSuit[i];
            int numCurrentSuit = aiHand.NumOfSuit(currentSuit);
            if(numCurrentSuit <= 2 && i != 0){ // might want to refine this later
                bidEstimate += 1.0f; 
            }

            // Add a bid for every ace
            if(aiHand.HasValue(currentSuit, 14)){
                bidEstimate += 1.0f;
            }

            // Add a half bid for every King
            if(aiHand.HasValue(currentSuit, 13)){
                bidEstimate += 0.5f;
            }
        }

        // Anthony Test Code just so Bids are not very low
        bidEstimate += aiHand.NumOfSuit(Suit.SPADE)/2;
        // currentBid = (int)bidEstimate; -- DEPRECATED

        return (int)bidEstimate; 
    }

    // Function to Choose AI's Card when Leading Trick
    private Card ChooseCardToLead()
    {
        // IF: spadesBroken == false -> Play highest non-spade (Prioritize Suit with Most Cards, !Don't Know if Good!)
        if (GameManager.Instance.spadesBroken == false)
        {
            Suit suitWithMostCards = Suit.HEART;
            // Check Most Cards between Heart and Diamond
            if (aiHand.NumOfSuit(suitWithMostCards) < aiHand.NumOfSuit(Suit.DIAMOND)) {
                suitWithMostCards = Suit.DIAMOND;
            }
            // Check Most Cards between Heart/Diamond and Club
            if (aiHand.NumOfSuit(suitWithMostCards) < aiHand.NumOfSuit(Suit.CLUB)) {
                suitWithMostCards = Suit.CLUB;
            }

            // Check if any remaining non-Spade, IF NOT play lowest Spade
            if (aiHand.HasSuit(suitWithMostCards)) 
            { return aiHand.GetHighest(suitWithMostCards); }
            else 
            { return aiHand.GetLowest(Suit.SPADE); }
        }
        // ELSE: spadesBroken == true -> Same Procedure, but include Spades from the Beginning
        else
        {
            Suit suitWithMostCards = Suit.HEART;
            // Check Most Cards between Heart and Diamond
            if (aiHand.NumOfSuit(suitWithMostCards) < aiHand.NumOfSuit(Suit.DIAMOND)) {
                suitWithMostCards = Suit.DIAMOND;
            }
            // Check Most Cards between Heart/Diamond and Club
            if (aiHand.NumOfSuit(suitWithMostCards) < aiHand.NumOfSuit(Suit.CLUB)) {
                suitWithMostCards = Suit.CLUB;
            }
            // Check Most Cards between Heart/Diamond/Club and Spade
            if (aiHand.NumOfSuit(suitWithMostCards) < aiHand.NumOfSuit(Suit.SPADE)) {
                suitWithMostCards = Suit.SPADE;
            }

            return aiHand.GetHighest(suitWithMostCards);
        }
    }

    // Function to Choose AI's Card when Following Player
    private Card ChooseCardToFollow(Card playerCard)
    {
        // IF: AI has same suit as played card
        if (aiHand.HasSuit(playerCard.suit))
        {
            // Card possibleCard = aiHand.GetHighest(playerCard.suit); -- DEPRECATED

            Card possibleCard = aiHand.GetNextHighest(playerCard.suit, playerCard.val);

            // If: highest value of suit is greater play it
            // Else: play lowest value of suit 
            if (possibleCard.val < playerCard.val)
            { possibleCard = aiHand.GetLowest(playerCard.suit); }

            return possibleCard;
        }
        // Else IF: Has any Spades, play lowest Spade
        else if (aiHand.HasSuit(Suit.SPADE))
        {
            return aiHand.GetLowest(Suit.SPADE);
        }
        // Else: Play lowest non-Spade suit
        else
        {
            return aiHand.GetWorst();
        }      
    }
    #endregion

    #region "Public Helper Functions"
    // Function to Change TrickLead Boolean (Bool is for Trick Lead, NOT Draft/Bid Lead)
    public void ChangeInternalLead(bool isCurrLead)
    {
        isTrickLead = isCurrLead;
    }

    // Function to Add given Card to AI Hand
    public void DraftCard(Card card)
    {
        aiHand.AddCardToHand(card);
        aiHandUI.ShowCard(card);
    }

    // Call with Cheat Phase to enable AI Cheats
    public void ChangeCheatPhase(AICheatPhase cheatPhase)
    {
        currentCheatPhase = cheatPhase;
    }

    public void DecrementCheatUses(AICheatPhase cheatPhase, CheatName cheatName)
    {
        switch (cheatPhase)
        {
            case AICheatPhase.CheatPhaseOne:
                cheatSetOneDatabase[cheatName] = false;
                cheatsLeftPhaseOne--;
                break;
            // return canCheatPhase1;              
            case AICheatPhase.CheatPhaseTwo:
                cheatSetTwoDatabase[cheatName] = false;
                cheatsLeftPhaseTwo--;
                break;
            // return canCheatPhase2;
            default:
                Debug.Log("Invalid Cheat Phase");
                break;
                
        }
    }

    // Call to GET if AI can use a given cheat or not
    public bool GetCanUseCheat(AICheatPhase cheatPhase, CheatName cheatName)
    {
        switch (cheatPhase)
        {
            case AICheatPhase.CheatPhaseOne:
                if (currentCheatPhase != AICheatPhase.CheatPhaseOne || cheatsLeftPhaseOne <= 0) { return false; }
                else { return cheatSetOneDatabase[cheatName]; }
                // return canCheatPhase1;              
            case AICheatPhase.CheatPhaseTwo:
                if (currentCheatPhase != AICheatPhase.CheatPhaseTwo || cheatsLeftPhaseTwo <= 0) { return false; }
                else { return cheatSetTwoDatabase[cheatName]; }
            // return canCheatPhase2;
            default:
                Debug.Log("Invalid Cheat Phase");
                return false;
        }
    }

    // Call to GET what Cheat Phase AI is currently at
    public AICheatPhase GetCheatPhase()
    {
        return currentCheatPhase;
    }
    #endregion

    #region "Private Helper Functions"
    // Function to perform AI Actions on Phase.AITURN
    private IEnumerator HandleAITurn()
    {
        yield return new WaitForSeconds(2); // wait to preserve game flow
        // AI Determines what card to play and Plays It
        PlayCard();
    }

    // Function to Calculate AI's Card to play
    private Card DecideCard()
    {
        if (isTrickLead) {
            return ChooseCardToLead();
        }
        else {
            return ChooseCardToFollow(GameManager.Instance.playerCard);
        }     
    }

    // playcard handles the mechanics of the AI's turn
    private void PlayCard()
    {
        Card cardToPlay = DecideCard();
        // Feed GameManager Played Card
        GameManager.Instance.aiCard = cardToPlay;

        // If card has Dialogue attached Enqueue it
        if (cardToPlay.HasDialogueAttached() && GameManager.Instance.isInTutorial == false)
        {
            DialogueManager.Instance.EnqueueDialogueSO(cardToPlay.GetDialogueSO(), false);
        }

        // Update UI for AI's Card
        // use dialogue to announce card played here?
        // Remove Card from AI's Hand
        aiHand.RemoveCardFromHand(cardToPlay);
        aiHandUI.ShowCardPlayed(cardToPlay);
        // cardDisplay.text = cardToPlay.ToString(); deprecated - do need to update this to show card in new way

        // Move On
        if (isTrickLead == true)
        {
            GameManager.Instance.ChangePhase(Phase.PLAYERTURN);
        }
        else
        {
            GameManager.Instance.ChangePhase(Phase.ENDOFTRICK);
        }
    }

    // call this during draft to show the card move in front of the opponent's face "for consideration"
    private void ConsiderCard(Card card)
    {
        card.MoveToLocation(displaySpot.position, displaySpot.rotation);
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
}
