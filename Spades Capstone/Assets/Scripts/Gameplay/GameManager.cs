using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region "Singleton"
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

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

    #region "Class References/Variables"

    [SerializeField] private Deck deck;
    [SerializeField] private Transform discardSpot;

    [HideInInspector] public Phase currentPhase;
    public static event Action<Phase> OnPhaseChanged;
    public static event Action<Character> OnTrickTaken;

    [HideInInspector] public Character lead;

    [HideInInspector] public Card playerCard;
    [HideInInspector] public Card aiCard;
    [HideInInspector] public bool spadesBroken = false; // keeps track of if you are allowed to lead spades

    [HideInInspector] public int numDraftTurns = 0;
    [HideInInspector] public int numTurns = 0;

    [SerializeField] private TMP_Text phase_text;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Initialize game components
        lead = Character.DEATH; // Death always goes first for tutorial

        deck.Shuffle();
    }

    // Called to move through phases of the game
    public void ChangePhase(Phase newPhase)
    {
        
        Debug.Log("Change from " + currentPhase.ToString() + " to " + newPhase.ToString());
        currentPhase = newPhase;
        UpdatePhaseName(currentPhase);

        switch (newPhase)
        {
            case Phase.PLAYERDRAFT:
                numDraftTurns++;
                if (numDraftTurns > 26)
                {
                    newPhase = Phase.PLAYERBID;
                }
                break;
            case Phase.AIDRAFT:
                numDraftTurns++;
                if (numDraftTurns > 26)
                {
                    newPhase = Phase.AIBID;
                }
                break;
            case Phase.PLAYERBID:
                break;
            case Phase.AIBID:
                break;
            case Phase.PLAYERTURN:
                numTurns++;
                break;
            case Phase.AITURN:
                numTurns++;
                break;
            case Phase.ENDOFTRICK:
                HandleEndOfTrick();
                break;
            case Phase.SCORING:
                break;
            case Phase.ENDING:
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(newPhase), newPhase, null);
        }

        OnPhaseChanged?.Invoke(newPhase);       
    }

    #region "Public Helper Functions"

    // Reset tracker variables within GameManager
    public void ResetGM()
    {
        // Don't need to clear hands bc they clear themselves
        spadesBroken = false;
        // StartCoroutine(MoveCardsBackToDeck()); -- was broken, removing for playtest
        deck.Shuffle();
        numDraftTurns = 0;
        numTurns = 0;
        playerCard = null;
        aiCard = null;
    }

    // Calls Deck.DrawCard
    public Card DrawCard()
    {
        return deck.DrawCard();
    }

    // move a card from the deck to the discard pile
    public void DiscardCardFromDeck()
    {
        Card toDiscard = deck.DrawCard();
        toDiscard.MoveToLocation(discardSpot.position, discardSpot.rotation);
        toDiscard.SetInteractable(false);
        toDiscard.Unfreeze();
    }

    // Move a card from your hand to the discard pile
    public void DiscardCardFromHand(Card toDiscard)
    {
        toDiscard.MoveToLocation(discardSpot.position, discardSpot.rotation);
        toDiscard.SetInteractable(false);
        toDiscard.Unfreeze();
    }

    // End the game - ending cutscene based on winner
    public void EndGame(Character winner){
        Debug.Log("Winner is " + winner.ToString());
    }

    // Swap Lead of next Round(Draft & Bid)
    public void SwapLead()
    {
        if (lead == Character.DEATH)
        {
            lead = Character.PLAYER;
            SwapTurnLead(Character.PLAYER);
        }
        else
        {
            lead = Character.DEATH;
            SwapTurnLead(Character.DEATH);
        }
    }
    #endregion

    #region "Private Helper Functions"
    // Figure out who won the trick, log it, and move on NOTE: Refactor to work with better encapsulated turn counting system
    private void HandleEndOfTrick()
    {
        OnTrickTaken.Invoke(DetermineTrickWinner());
        DiscardCardFromHand(playerCard);
        DiscardCardFromHand(aiCard);

        if (numTurns >= 26)  // IF there are not more Tricks to play
        {
            // Switch to Score Phase
            ChangePhase(Phase.SCORING);
        }
        else if (DetermineTrickWinner() == Character.DEATH)  // IF Death won Trick and more Tricks to Play
        {
            // Ensure Death Leads next Trick and Change Phase
            SwapTurnLead(Character.DEATH);
            ChangePhase(Phase.AITURN);
        }
        else // IF Player won Trick and more Tricks to Play
        {
            // Ensure Player Leads next Trick and Change Phase
            SwapTurnLead(Character.PLAYER);
            ChangePhase(Phase.PLAYERTURN);
        }
    }

    // Implement the rules of trick taking
    private Character DetermineTrickWinner()
    {
        if(playerCard.suit == aiCard.suit) // if lead was followed, the higher value card wins
        {
            if(playerCard.val > aiCard.val)
            {
                return Character.PLAYER;
            }
            return Character.DEATH;
        }
        else if(playerCard.suit == Suit.SPADE) // if lead suit was not followed, spades always wins
        {
            spadesBroken = true;
            return Character.PLAYER;
        }
        else if(aiCard.suit == Suit.SPADE) // same as above
        {
            spadesBroken = true;   
            return Character.DEATH;
        }
        else if(lead == Character.PLAYER) // if lead suit was not followed and a spade was not played, lead wins
        {
            return Character.PLAYER;
        }
        else
        {
            return Character.DEATH; // same as above
        }
    }

    // Update UI with new Phase Name
    private void UpdatePhaseName(Phase newPhase)
    {
        phase_text.text = newPhase.ToString();
    }

    // Changes Internal Player/AI Manager Lead variable (Is used to check who leads a Trick, NOT the Draft or Bid)
    private void SwapTurnLead(Character trick_winner)
    {
        if (trick_winner == Character.DEATH)
        {
            PlayerManager.Instance.ChangeInternalLead(false);
            AIManager.Instance.ChangeInternalLead(true);
        }
        else
        {
            PlayerManager.Instance.ChangeInternalLead(true);
            AIManager.Instance.ChangeInternalLead(false);
        }
    }

    #endregion

    #region "Debugging"

    //Testing Functions (Switch to PlayerDraft or AIDraft)
    public void TestPDraft()
    {
        ChangePhase(Phase.PLAYERDRAFT);
    }

    public void TestD_Draft()
    {
        ChangePhase(Phase.AIDRAFT);
    }

    #endregion
}
public enum Phase { PLAYERDRAFT, AIDRAFT, ENDOFDRAFT, PLAYERBID, AIBID, ENDOFTRICK, PLAYERTURN, AITURN, SCORING, ENDING };
public enum Character { DEATH, PLAYER };
