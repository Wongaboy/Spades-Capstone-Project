using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Initialize game components
        lead = Character.DEATH; // Death always goes first for tutorial
    }

    public void ChangePhase(Phase newPhase)
    {
        currentPhase = newPhase;
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
                // Clear hands
                spadesBroken = false;
                SwapLead();
                deck.Shuffle();
                break;
            case Phase.ENDING:
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(newPhase), newPhase, null);
        }

        OnPhaseChanged?.Invoke(newPhase);
    }

    public Card DrawCard()
    {
        return deck.DrawCard();
    }

    // end the game - ending cutscene based on winner
    public void EndGame(Character winner){
        
    }

    // figure out who won the trick, log it, and move on
    private void HandleEndOfTrick()
    {
        OnTrickTaken.Invoke(DetermineTrickWinner());
    }

    // implement the rules of trick taking - warning: awful code
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

    private void SwapLead()
    {
        if(lead == Character.DEATH)
        {
            lead = Character.PLAYER;
        }
        else { lead = Character.DEATH; }
    }
}
public enum Phase { PLAYERDRAFT, AIDRAFT, PLAYERBID, AIBID, ENDOFTRICK, PLAYERTURN, AITURN, SCORING, ENDING };
public enum Character { DEATH, PLAYER };
