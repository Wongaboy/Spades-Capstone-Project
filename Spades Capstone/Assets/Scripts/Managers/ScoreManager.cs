using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    #region "Singleton"
    private static ScoreManager _instance;

    public static ScoreManager Instance { get { return _instance; } }

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
    public int playerBid = 0;
    public int aiBid = 0;
    int playerScore = 0;
    int playerBags = 0;
    int aiScore = 0;
    int aiBags = 0;
    int playerTricks;
    int aiTricks;
    Character currLead;
    Character winningChar;
    [SerializeField] TallyBoard tallyBoard;
    [SerializeField] int scoreToWin;

    [SerializeField] int phaseOnePointThreshold;
    //[SerializeField] DialogueSO phaseOneDialogue;
    [SerializeField] int phaseTwoPointThreshold;
    //[SerializeField] DialogueSO phaseTwoDialogue;

    #endregion

    // start listening to the game manager 
    void Start()
    {
        GameManager.OnPhaseChanged += ScoreManagerOnPhaseChanged;
        GameManager.OnTrickTaken += takeTrick;
    }

    // when the phase is changed, see if it's time for scoring
    private void ScoreManagerOnPhaseChanged(Phase phase)
    {
        currLead = GameManager.Instance.lead;
        if(phase == Phase.SCORING){
            StartCoroutine(HandleScoring()); // do everything that is required for scoring
        }
        else if(phase == Phase.PLAYERBID){
            HandlePlayerBid(); // let the player use UI to make a bid
        }
        else if(phase == Phase.AIBID){
            HandleAIBid(); // have the AI make its bid
        }
    }

    // tally up the points for the round, add them to total, then subtract bags if applicable
    // if a player has won, end the game
    private IEnumerator HandleScoring(){
        int roundScore;
        int roundBags;

        // If AI is in Cheat Mode Override "playerBid" -> "playerBid - 1" on EXACT MATCH
        if (playerBid == playerTricks && AIManager.Instance.GetCanUseCheat(AICheatPhase.CheatPhaseOne, CheatName.ChangeBid))
        {
            (roundScore, roundBags) = _CalcScore(playerBid - 1, playerTricks);
            AIManager.Instance.DecrementCheatUses(AICheatPhase.CheatPhaseOne, CheatName.ChangeBid);
            DialogueManager.Instance.AddCheatDialogue(CheatName.ChangeBid, false);
            Debug.Log("ChangeBid Cheat has been activated");
        }
        else
        {
            (roundScore, roundBags) = _CalcScore(playerBid, playerTricks);
        }

        playerScore += roundScore;
        playerBags += roundBags;
        if(playerBags >= 10){
            playerScore -= 100;
            playerBags -= 10;
        }

        (roundScore, roundBags) = _CalcScore(aiBid, aiTricks);
        aiScore += roundScore;

        // If AI is in Cheat Mode Override "roundBags" -> "0" when Bag penalty
        if (roundBags > 0 && AIManager.Instance.GetCanUseCheat(AICheatPhase.CheatPhaseOne, CheatName.IgnorePenalty))
        {
            roundBags = 0;
            AIManager.Instance.DecrementCheatUses(AICheatPhase.CheatPhaseOne, CheatName.IgnorePenalty);
            DialogueManager.Instance.AddCheatDialogue(CheatName.IgnorePenalty, false);
            Debug.Log("Override Bag Gain Cheat has been activated");
        }

        aiBags += roundBags;
        if(aiBags >= 10){
            // If AI is in Cheat Mode Override aiScore to NOT lose 100 points
            if (AIManager.Instance.GetCanUseCheat(AICheatPhase.CheatPhaseOne, CheatName.IgnorePenalty))
            {
                AIManager.Instance.DecrementCheatUses(AICheatPhase.CheatPhaseOne, CheatName.IgnorePenalty);
                DialogueManager.Instance.AddCheatDialogue(CheatName.IgnorePenalty, false);
                Debug.Log("Override Bag penalty Cheat has been activated");
            }
            else { aiScore -= 100; }
            aiBags -= 10;
        }

        // Reset Tricks
        playerTricks = 0;
        aiTricks = 0;

        // Update tallyboard with correct info & Reset Tricks and Bid UI for next round
        /*

            This is the area to Add the Chalk VFX for Tallyboard Changes

        */
        tallyBoard.updateScoreText(playerScore, aiScore);
        tallyBoard.updateBagText(playerBags, aiBags);     

        tallyBoard.updateTrickText(Character.PLAYER, 0);
        tallyBoard.updateTrickText(Character.DEATH, 0);

        tallyBoard.updateBidText(Character.PLAYER, 0);
        tallyBoard.updateBidText(Character.DEATH, 0);

        if (playerScore > aiScore){
            winningChar = Character.PLAYER;
        }
        else{
            winningChar = Character.DEATH;
        }

        // Check if Score checkpoints have been reached to activate AI Cheats & Check if already in said phase
        if (playerScore >= phaseTwoPointThreshold)
        {
            if (AIManager.Instance.GetCheatPhase() != AICheatPhase.CheatPhaseTwo) 
            {
                if (!AIManager.Instance.HasEnteredCheatPhaseBefore(AICheatPhase.CheatPhaseTwo)) { DialogueManager.Instance.AddCheatPhaseDialogue(2, false); }
                AIManager.Instance.ChangeCheatPhase(AICheatPhase.CheatPhaseTwo);
                Debug.Log("AI has entered Cheat Phase 2");
            }
            
        }
        else if (playerScore >= phaseOnePointThreshold)
        {
            if (AIManager.Instance.GetCheatPhase() != AICheatPhase.CheatPhaseOne)
            {
                if (!AIManager.Instance.HasEnteredCheatPhaseBefore(AICheatPhase.CheatPhaseOne)) { DialogueManager.Instance.AddCheatPhaseDialogue(1, false); }
                AIManager.Instance.ChangeCheatPhase(AICheatPhase.CheatPhaseOne);
                Debug.Log("AI has entered Cheat Phase 1");
            }
        }

        // If there is a Winner - End the game
        if (CheckWin()){
            GameManager.Instance.EndGame(winningChar);
        }
        // Else: Reset GameManager counters, Swap Lead, and change phase to approriate Character
        else
        {
            if (GameManager.Instance.isInTutorial)
            {
                StartCoroutine(TutorialManager.Instance.EndTutorial());
            }
            else
            {
                yield return new WaitForSeconds(1);
                GameManager.Instance.ResetGM();
                yield return new WaitForSeconds(8.2f); // wait long enough for the game manager to fully reset before moving to draft phase
                GameManager.Instance.SwapLead();
             
                if (GameManager.Instance.lead == Character.DEATH)
                {
                    GameManager.Instance.ChangePhase(Phase.AIDRAFT);
                }
                else
                {
                    GameManager.Instance.ChangePhase(Phase.PLAYERDRAFT);
                }
            }
        }
    }

    // Make a ui element visible/ glowing to the player that allows them to choose their bid
    private void HandlePlayerBid()
    {
        // Toggle BID UI - TEMP
        BidUI.Instance.ToggleBidUI(true); 

        /* use this logic when exiting player bid logic
        if(currLead == Character.PLAYER){
            GameManager.Instance.ChangePhase(Phase.AIBID);
        } 
        else{
            GameManager.Instance.ChangePhase(Phase.AITURN);
        }
        */

    }

    // Update Player Bid data & UI
    public void SetPlayerBid(int new_bid)
    {
        playerBid = new_bid;
        tallyBoard.updateBidText(Character.PLAYER, playerBid);
    }

    // Have the AI place a bid based on their cards
    private void HandleAIBid()
    {
        aiBid = AIManager.Instance.GetBid();
        tallyBoard.updateBidText(Character.DEATH, aiBid);

        StartCoroutine(WaitTimeBid());
    }

    private IEnumerator WaitTimeBid()
    {
        yield return new WaitForSeconds(1);

        if (currLead == Character.DEATH)
        {
            GameManager.Instance.ChangePhase(Phase.PLAYERBID);
        }
        else
        {
            GameManager.Instance.ChangePhase(Phase.PLAYERTURN);
        }
    }

    private void takeTrick(Character trickWinner)
    {
        if(trickWinner == Character.PLAYER)
        {
            playerTricks += 1;
            tallyBoard.updateTrickText(Character.PLAYER, playerTricks);
        }
        else 
        { 
            aiTricks += 1;
            tallyBoard.updateTrickText(Character.DEATH, aiTricks);
        }
    }

    // if either score is above 500, the game is over
    private bool CheckWin()
    {
        return (playerScore > scoreToWin || aiScore > scoreToWin);
    }

    // helper func to calculate a score based on bid and tricks taken
    private (int, int) _CalcScore(int bid, int tricks){
        if(bid > tricks){
            return (-10 * (bid - tricks), 0); // lose ten points for every trick failed to take, get no bags
        }
        else if(bid == 0) // succesful nil bid gains you 100 points, unsuccesful nil bid loses you 100 
        {
            if(tricks == 0) { return (100, 0); }
            return (-100, tricks);
        }
        else{
            return (10 * bid, tricks - bid); // gain ten points for every trick bid, and gain a bad for every extra trick
        }
    }

    // Reset tallyboard to all zeroes (used for Tutorial purposes)
    public void ResetTallyBoardScores()
    {
        ResetInternalScoreVariables();

        tallyBoard.updateScoreText(0, 0);
        tallyBoard.updateBagText(0, 0);

        tallyBoard.updateTrickText(Character.PLAYER, 0);
        tallyBoard.updateTrickText(Character.DEATH, 0);

        tallyBoard.updateBidText(Character.PLAYER, 0);
        tallyBoard.updateBidText(Character.DEATH, 0);
    }

    public void ResetInternalScoreVariables()
    {
        playerBid = 0;
        aiBid = 0;
        playerScore = 0;
        playerBags = 0;
        aiScore = 0;
        aiBags = 0;
        playerTricks = 0;
        aiTricks = 0;
    }

    #region "Debug Functions"
    public void AlterScore(int newScore)
    {
        playerScore = newScore;
    }
    #endregion
    // unsubscribe from events when destroyed to prevent errors
    void OnDestroy(){
        GameManager.OnPhaseChanged -= ScoreManagerOnPhaseChanged;
        GameManager.OnTrickTaken -= takeTrick;
    }
}
