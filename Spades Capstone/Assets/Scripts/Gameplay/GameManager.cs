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

    public Phase currentPhase;
    public static event Action<Phase> OnPhaseChanged;
    public static event Action<Character> OnTrickTaken;
    public Character lead;

    public Card playerCard;
    public Card aiCard;
    public bool spadesBroken = false; // keeps track of if you are allowed to lead spades

    public int num_DraftTurns = 0;
    public int num_Turns = 0;

    [SerializeField] private TMP_Text phase_text;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Initialize game components
        lead = Character.DEATH; // Death always goes first for tutorial

        deck.Shuffle();
    }

    // Debug Testing Functions (Switch to PlayerDraft or AIDraft)
    public void TestPDraft()
    {
        ChangePhase(Phase.PLAYERDRAFT);
    }

    public void TestD_Draft()
    {
        ChangePhase(Phase.AIDRAFT);
    }

    public void ChangePhase(Phase newPhase)
    {
        Debug.Log("Change to " + newPhase.ToString());

        currentPhase = newPhase;
        UpdatePhaseName(currentPhase);
        switch (newPhase)
        {
            case Phase.PLAYERDRAFT:
                break;
            case Phase.AIDRAFT:
                break;
            case Phase.PLAYERBID:
                break;
            case Phase.AIBID:
                break;
            case Phase.PLAYERTURN:
                break;
            case Phase.AITURN:
                break;
            case Phase.ENDOFTRICK:
                HandleEndOfTrick();
                break;
            case Phase.SCORING:
                // I moved this section into the scoremanager just for the time being, pretty sure with some refactoring of my code it can be moved
                // Clear hands
                //spadesBroken = false;
                //deck.Shuffle();
                //num_DraftTurns = 0;
                //num_Turns = 0;
                //playerCard = null;
                //aiCard = null;
                break;
            case Phase.ENDING:
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(newPhase), newPhase, null);
        }

        OnPhaseChanged?.Invoke(newPhase);

    }

    // Reset tracker variables within GameManager
    public void ResetGM()
    {
        // Clear hands
        spadesBroken = false;
        deck.Shuffle();
        num_DraftTurns = 0;
        num_Turns = 0;
        playerCard = null;
        aiCard = null;
    }

    // Calls Deck.DrawCard
    public Card DrawCard()
    {
        return deck.DrawCard();
    }

    // Calls Deck.Discard
    public void DiscardCard()
    {
        deck.DiscardCard();
    }

    // Increment DraftTurn counter
    public void IncrementDraftTurn() 
    {
        num_DraftTurns++; 
    }

    // Gets DraftTurn counter
    public int GetDraftTurn()
    {
        return num_DraftTurns;
    }

    // Increment "Round" Turn counter
    public void IncrementTurn()
    {
        num_Turns++;
    }

    // Gets "Round" Turn counter
    public int GetTurn()
    {
        return num_Turns;
    }

    // Update UI with new Phase Name
    public void UpdatePhaseName(Phase newPhase)
    {
        phase_text.text = newPhase.ToString();
    }

    // End the game - ending cutscene based on winner
    public void EndGame(Character winner){
        Debug.Log("Winner is " + winner.ToString());
    }

    // Figure out who won the trick, log it, and move on
    private void HandleEndOfTrick()
    {
        OnTrickTaken.Invoke(DetermineTrickWinner());
        // Clear Card UI for next Trick
        TurnUI.Instance.ClearCardInfo();
        if (num_Turns >= 13)  // IF there are not more Tricks to play
        {
            // Turn Off UI and Switch to Score Phase
            TurnUI.Instance.ToggleTurnUI(false);
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

    // Implement the rules of trick taking - warning: awful code
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

    // Swap Lead of next Round(Draft & Bid)
    public void SwapLead()
    {
        if(lead == Character.DEATH)
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
}
public enum Phase { PLAYERDRAFT, AIDRAFT, PLAYERBID, AIBID, ENDOFTRICK, PLAYERTURN, AITURN, SCORING, ENDING };
public enum Character { DEATH, PLAYER };
