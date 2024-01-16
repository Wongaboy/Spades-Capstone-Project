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

    [SerializeField] int cheatPhaseOnePointThreshold;
    [SerializeField] DialogueSO cheat1Dialogue;
    [SerializeField] int cheatPhaseTwoPointThreshold;
    [SerializeField] DialogueSO cheat2Dialogue;

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

        // Anthony Personal Note
        // If AI is in Cheat Mode Override "playerBid" -> "playerBid - 1" on EXACT MATCH
        if (playerBid == playerTricks && AIManager.Instance.GetCanUseCheat(AIManager.AICheatPhase.CheatPhaseOne, "ChangeBid"))
        {
            (roundScore, roundBags) = _CalcScore(playerBid - 1, playerTricks);
            AIManager.Instance.DecrementCheatUses(AIManager.AICheatPhase.CheatPhaseOne, "ChangeBid");
            DialogueManager.Instance.AddCheatDialogueToQueue("ChangeBid");
            DialogueManager.Instance.StartDialogue();
            Debug.Log("PlayerBid Cheat has been activated");
        }
        else
        {
            (roundScore, roundBags) = _CalcScore(playerBid, playerTricks);
        }
        // (roundScore, roundBags) = _CalcScore(playerBid, playerTricks);

        playerScore += roundScore;
        playerBags += roundBags;
        if(playerBags >= 10){
            playerScore -= 100;
            playerBags -= 10;
        }

        (roundScore, roundBags) = _CalcScore(aiBid, aiTricks);
        aiScore += roundScore;

        // Anthony Personal Note
        // If AI is in Cheat Mode Override "roundBags" -> "0" when Bag penalty
        if (roundBags > 0 && AIManager.Instance.GetCanUseCheat(AIManager.AICheatPhase.CheatPhaseOne, "IgnorePenalty"))
        {
            roundBags = 0;
            AIManager.Instance.DecrementCheatUses(AIManager.AICheatPhase.CheatPhaseOne, "IgnorePenalty");
            DialogueManager.Instance.AddCheatDialogueToQueue("IgnorePenalty");
            DialogueManager.Instance.StartDialogue();
            Debug.Log("Override Bag Gain Cheat has been activated");
        }

        aiBags += roundBags;
        if(aiBags >= 10){
            // Anthony Personal Note
            // If AI is in Cheat Mode Override aiScore to NOT lose 100 points
            if (AIManager.Instance.GetCanUseCheat(AIManager.AICheatPhase.CheatPhaseOne, "IgnorePenalty"))
            {
                AIManager.Instance.DecrementCheatUses(AIManager.AICheatPhase.CheatPhaseOne, "IgnorePenalty");
                DialogueManager.Instance.AddCheatDialogueToQueue("IgnorePenalty");
                DialogueManager.Instance.StartDialogue();
                Debug.Log("Override Bag penalty Cheat has been activated");
            }
            else
            {
                aiScore -= 100;
            }
            // aiScore -= 100;
            aiBags -= 10;
        }

        // Reset Tricks
        playerTricks = 0;
        aiTricks = 0;

        // Update tallyboard with correct info & Reset Tricks and Bid UI for next round
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

        // Anthony Personal Note
        // Check if Score checkpoints have been reached to activate AI Cheat Set if they have not already entered cheat phase
        if (playerScore >= cheatPhaseTwoPointThreshold)
        {
            if (AIManager.Instance.GetCheatPhase() != AIManager.AICheatPhase.CheatPhaseTwo) 
            {
                AIManager.Instance.ChangeCheatPhase(AIManager.AICheatPhase.CheatPhaseTwo);
                // DialogueManager.Instance.AddToDialogueQueue(cheat2Dialogue);
                DialogueManager.Instance.AddCheatDialogueToQueue("CheatPhaseTwo");
                DialogueManager.Instance.StartDialogue();
                Debug.Log("AI has entered Cheat Phase 2");
            }
            
        }
        else if (playerScore >= cheatPhaseOnePointThreshold)
        {
            if (AIManager.Instance.GetCheatPhase() != AIManager.AICheatPhase.CheatPhaseOne)
            {
                AIManager.Instance.ChangeCheatPhase(AIManager.AICheatPhase.CheatPhaseOne);
                // DialogueManager.Instance.AddToDialogueQueue(cheat1Dialogue);
                DialogueManager.Instance.AddCheatDialogueToQueue("CheatPhaseOne");
                DialogueManager.Instance.StartDialogue();
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
            yield return new WaitForSeconds(1);
            GameManager.Instance.ResetGM();
            yield return new WaitForSeconds(6.2f); // wait long enough for the game manager to fully reset before moving to draft phase
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
        // If player has already bid, they begin play. Otherwhise let them bid.
        //if (currLead == Character.DEATH){
        //    GameManager.Instance.ChangePhase(Phase.PLAYERBID);
        //} 
        //else{
        //    GameManager.Instance.ChangePhase(Phase.PLAYERTURN);
        //}
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

    // unsubscribe from events when destroyed to prevent errors
    void OnDestroy(){
        GameManager.OnPhaseChanged -= ScoreManagerOnPhaseChanged;
        GameManager.OnTrickTaken -= takeTrick;
    }
}
