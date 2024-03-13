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
    [SerializeField] private VFXPlayer slashFX;

    [HideInInspector] public Phase currentPhase;
    public static event Action<Phase> OnPhaseChanged;
    public static event Action<Character> OnTrickTaken;

    [HideInInspector] public Character lead;

    [HideInInspector] public Card playerCard;
    [HideInInspector] public Card aiCard;
    [HideInInspector] public bool spadesBroken = false; // keeps track of if you are allowed to lead spades

    [HideInInspector] public int numDraftTurns = 0;
    [HideInInspector] public int numTurns = 0;

    public bool isInTutorial = false;

    public Phase phaseAfterDialogue;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Initialize game components
        lead = Character.DEATH; // Death always goes first for tutorial

        // Ask/Prompt for Tutorial          
        // StartCoroutine(StartTutorialPrompt());

        // StartCoroutine(StartGame());
    }

    // Called to move through phases of the game
    public void ChangePhase(Phase newPhase)
    {
        // Debug.Log("Change from " + currentPhase.ToString() + " to " + newPhase.ToString());
        // Debug.Log(newPhase);
        currentPhase = newPhase;

        // IF there is Dialogue Enqueued, Then Override ChangePhase to ResolveDialogue while Saving the Phase to change to After Dialogue
        if (DialogueManager.Instance.HasDialogueEnqueued() == true)
        {
            phaseAfterDialogue = newPhase;
            currentPhase = Phase.DIALOGUERESOLVE;
        }

        switch (currentPhase)
        {
            case Phase.PLAYERDRAFT:
                numDraftTurns++;
                if (numDraftTurns > 26) {
                    currentPhase = Phase.PLAYERBID;
                }
                break;
            case Phase.AIDRAFT:
                numDraftTurns++;
                if (numDraftTurns > 26) {
                    currentPhase = Phase.AIBID;
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
            case Phase.DIALOGUERESOLVE:
                StartCoroutine(DialogueManager.Instance.ResolveDialogue(phaseAfterDialogue));
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(currentPhase), currentPhase, null);
        }

        OnPhaseChanged?.Invoke(currentPhase);       
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

    // Reset Internal Trick Lead variables of the AI & player Manager
    public void ResetInternalTrickLead(Character newlead)
    {
        if (newlead == Character.DEATH)
        {
            AIManager.Instance.ChangeInternalLead(true);
            PlayerManager.Instance.ChangeInternalLead(false);
        }
        else
        {
            AIManager.Instance.ChangeInternalLead(false);
            PlayerManager.Instance.ChangeInternalLead(true);
        }
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
        if (winner == Character.PLAYER)
        {
            StartCoroutine(EndingManager.Instance.TriggerEndCutscene(winner));
        }
        else
        {
            StartCoroutine(EndingManager.Instance.TriggerEndCutscene(winner));
        }
        PlayerManager.Instance.ToggleCardAmountDisplay(false);
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

    // Returns list of cards that are currently interactable
    public List<Card> GetInteractableCards()
    {
        if (deck.isActiveAndEnabled) { return deck.GetInteractableCards(); }
        else { return new List<Card>(); }
    }
    #endregion

    #region "Private Helper Functions"
    // Called to start gameloop
    public IEnumerator StartGame()
    {
        StartCoroutine(deck.Shuffle());
        yield return new WaitUntil(() => !deck.isShuffling);
        //yield return new WaitForSeconds(7f);
        PlayerManager.Instance.ToggleCardAmountDisplay(true);
        ChangePhase(Phase.AIDRAFT);
    }

    // Seperate function to start game with Tutorial Prompt
    public void StartGameAfterIntro()
    {
        // Ask/Prompt for Tutorial          
        StartCoroutine(StartTutorialPrompt());
    }

    // Trigger Tutorial Prompt
    public IEnumerator StartTutorialPrompt()
    {
        // Play Dialogue
        // Wait Until Dialogue Over
        StartCoroutine(TutorialManager.Instance.TriggerTutorialPrompt());
        yield return new WaitUntil(() => (TutorialManager.Instance.IsTutorialPromptActive() == false && DialogueManager.Instance.IsDialogueActive() == false));

        if (TutorialManager.Instance.IsTutorialWanted())
        {
            // Do Tutorial
            isInTutorial = true;
            Debug.Log("They said YES to tutorial");
            StartCoroutine(StartGame());
        }
        else
        {
            // Do Normal Gameplay
            isInTutorial = false;
            DialogueManager.Instance.TurnOffPressSpace(false);
            Debug.Log("They said NO to tutorial");
            StartCoroutine(StartGame());
        }
    }
    // Figure out who won the trick, log it, and move on NOTE: Refactor to work with better encapsulated turn counting system
    private IEnumerator HandleEndOfTrick()
    {
        yield return new WaitForSeconds(1f);
        Character newTrickWinner = DetermineTrickWinner();

        // VFX handling - this took way too long and was so stupid but I think it looks alright
        Vector3 fxpos;
        Quaternion fxrot;
        if(newTrickWinner == Character.DEATH)
        {
            fxpos = aiCard.gameObject.transform.position;
            fxrot = new Quaternion(0, 0.42261827f, 0, 0.906307876f);
        }
        else { 
            fxpos = playerCard.gameObject.transform.position;
            fxrot = new Quaternion(0, -0.766044497f, 0, 0.642787635f);
        }

        slashFX.gameObject.transform.position = fxpos;
        slashFX.gameObject.transform.rotation = fxrot;
        // slashFX.transform.Rotate(new Vector3(90, 0, 90));
        slashFX.TriggerFX(slashFX.transform);


        OnTrickTaken.Invoke(newTrickWinner);
        DiscardCardFromHand(playerCard);
        yield return new WaitForSeconds(1f);
        DiscardCardFromHand(aiCard);
        TallyBoard.Instance.clearPlayedCard();

        if (numTurns >= 26)  // IF there are not more Tricks to play
        {
            // Switch to Score Phase
            ChangePhase(Phase.SCORING);
        }
        //else if (DetermineTrickWinner() == Character.DEATH)  // IF Death won Trick and more Tricks to Play
        else if (newTrickWinner == Character.DEATH)  // IF Death won Trick and more Tricks to Play
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

    // Implement the rules of trick taking (ONLY should be called to compute the logic, NOT to get value)
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
                    DialogueManager.Instance.AddCheatDialogue(CheatName.IgnorePenalty, false);
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
                    DialogueManager.Instance.AddCheatDialogue(CheatName.AddValueFromDiscard, false);
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

    #region "Temporary Functions"
    public void AltEndGame()
    {
        EndGame(Character.PLAYER);
    }
    #endregion
}
public enum Phase { PLAYERDRAFT, AIDRAFT, ENDOFDRAFT, PLAYERBID, AIBID, ENDOFTRICK, PLAYERTURN, AITURN, SCORING, ENDING, DIALOGUERESOLVE };
public enum Character { DEATH, PLAYER };
