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
    // private List<Card> discardPile = new List<Card>();
    [SerializeField] private DiscardPile discardPile;

    [HideInInspector] public Phase currentPhase;
    public static event Action<Phase> OnPhaseChanged;
    public static event Action<Character> OnTrickTaken;

    [HideInInspector] public Character lead;

    [HideInInspector] public Card playerCard;
    [HideInInspector] public Card aiCard;
    [HideInInspector] public bool spadesBroken = false; // keeps track of if you are allowed to lead spades

    [HideInInspector] public int numDraftTurns = 0;
    [HideInInspector] public int numTurns = 0;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Initialize game components
        lead = Character.DEATH; // Death always goes first for tutorial

        StartCoroutine(deck.Shuffle());
    }

    // Called to move through phases of the game
    public void ChangePhase(Phase newPhase)
    {
        
        // Debug.Log("Change from " + currentPhase.ToString() + " to " + newPhase.ToString());
        currentPhase = newPhase;
        // UpdatePhaseName(currentPhase); - no longer necessary
        Debug.Log(newPhase);

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
                StartCoroutine(HandleEndOfTrick());
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
        StartCoroutine(deck.Shuffle());
        numDraftTurns = 0;
        numTurns = 0;
        playerCard = null;
        aiCard = null;

        // discardPile.Clear();
        discardPile.ClearDiscardPile();
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
        toDiscard.MoveToLocation(discardSpot.position, discardSpot.rotation, true);
        toDiscard.SetInteractable(false);
        // toDiscard.Unfreeze();

        // discardPile.Add(toDiscard);
        discardPile.AddCardToDiscardPile(toDiscard);
    }

    // Move a card from your hand to the discard pile
    public void DiscardCardFromHand(Card toDiscard)
    {
        toDiscard.MoveToLocation(discardSpot.position, discardSpot.rotation, true);
        toDiscard.SetInteractable(false);
        // toDiscard.Unfreeze(); -- this is now handled in MoveToLocation

        // discardPile.Add(toDiscard);
        discardPile.AddCardToDiscardPile(toDiscard);
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
    private IEnumerator HandleEndOfTrick()
    {
        yield return new WaitForSeconds(1f);
        OnTrickTaken.Invoke(DetermineTrickWinner());
        DiscardCardFromHand(playerCard);
        yield return new WaitForSeconds(1f);
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
        Character trickWinner;
        if(playerCard.suit == aiCard.suit) // if lead was followed, the higher value card wins
        {
            if (playerCard.val < aiCard.val) {
                trickWinner = Character.DEATH;
            }
            else
            {
                // If AI is in Cheat Mode Override outcome and Randomize playerCard.val then recalculate results
                if (AIManager.Instance.GetCanUseCheat(AICheatPhase.CheatPhaseTwo, CheatName.RandomizePlayerCardValue))
                {
                    int newPlayerCardValue = UnityEngine.Random.Range(2, 14);
                    AIManager.Instance.DecrementCheatUses(AICheatPhase.CheatPhaseTwo, CheatName.RandomizePlayerCardValue);
                    DialogueManager.Instance.AddCheatDialogue(CheatName.IgnorePenalty, true);
                    Debug.Log("Randomize Player Card Value Cheat has been activated");

                    if (newPlayerCardValue > aiCard.val) {
                        trickWinner = Character.PLAYER;
                    }
                    else {
                        trickWinner = Character.DEATH;
                    }
                }
                // If can use Add Value From Discard Cheat, do procedures and recalculate
                else if (AIManager.Instance.GetCanUseCheat(AICheatPhase.CheatPhaseTwo, CheatName.AddValueFromDiscard))
                {
                    int addedValueFromDiscard = discardPile.GetHighest(aiCard.suit).val;
                    AIManager.Instance.DecrementCheatUses(AICheatPhase.CheatPhaseTwo, CheatName.AddValueFromDiscard);
                    DialogueManager.Instance.AddCheatDialogue(CheatName.AddValueFromDiscard, true);
                    Debug.Log("Add Value from Discard Cheat has been activated");

                    if (playerCard.val > aiCard.val + addedValueFromDiscard) {
                        trickWinner = Character.PLAYER;
                    }
                    else {
                        trickWinner = Character.DEATH;
                    }
                }
                else
                {
                    trickWinner = Character.PLAYER;
                }
            }
        }
        else if(playerCard.suit == Suit.SPADE) // if lead suit was not followed, spades always wins
        {
            spadesBroken = true;
            trickWinner = Character.PLAYER;
        }
        else if(aiCard.suit == Suit.SPADE) // same as above
        {
            spadesBroken = true;
            trickWinner = Character.DEATH;
        }
        else if(PlayerManager.Instance.isLead) // if lead suit was not followed and a spade was not played, lead wins
        {
            trickWinner = Character.PLAYER;
        }
        else
        {
            trickWinner = Character.DEATH; // same as above
        }

        return trickWinner;
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
