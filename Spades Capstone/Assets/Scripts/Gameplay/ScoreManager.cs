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
    TallyBoard tallyBoard;

    [SerializeField] TMP_Text AI_Score;
    [SerializeField] TMP_Text AI_Bags;
    [SerializeField] TMP_Text AI_Tricks;
    [SerializeField] TMP_Text Player_Score;
    [SerializeField] TMP_Text Player_Bags;
    [SerializeField] TMP_Text Player_Tricks;

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
            HandleScoring(); // do everything that is required for scoring
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
    private void HandleScoring(){
        int roundScore;
        int roundBags;
        (roundScore, roundBags) = _CalcScore(playerBid, playerTricks); // idk if this is legal syntax, if not there are other ways to do this
        playerScore += roundScore;
        playerBags += roundBags;
        if(playerBags >= 10){
            playerScore -= 100;
            playerBags -= 10;
        }

        (roundScore, roundBags) = _CalcScore(aiBid, aiTricks);
        aiScore += roundScore;
        aiBags += roundBags;
        if(aiBags >= 10){
            aiScore -= 100;
            aiBags -= 10;
        }

        // update tallyboard and reset tricks for next round
        // tallyBoard.updateScoreText(playerScore, aiScore);
        playerTricks = 0;
        aiTricks = 0;

        UpdateScoreUI(); //Temp Code
        UpdateTrickUI(); //Temp Code

        if (playerScore > aiScore){
            winningChar = Character.PLAYER;
        }
        else{
            winningChar = Character.DEATH;
        }
        
        if(_CheckWin()){
            GameManager.Instance.EndGame(winningChar);
        }
        else
        {
            GameManager.Instance.ResetGM();
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

    public void UpdateScoreUI()
    {
        AI_Score.text = aiScore.ToString();
        AI_Bags.text = aiBags.ToString();

        Player_Score.text = playerScore.ToString();
        Player_Bags.text = playerBags.ToString();
    }

    public void UpdateTrickUI()
    {
        AI_Tricks.text = aiTricks.ToString();

        Player_Tricks.text = playerTricks.ToString();
        
    }

    // make a ui element visible/ glowing to the player that allows them to choose their bid
    private void HandlePlayerBid()
    {
        /* use this logic when exiting player bid logic
        if(currLead == Character.PLAYER){
            GameManager.Instance.ChangePhase(Phase.AIBID);
        } 
        else{
            GameManager.Instance.ChangePhase(Phase.AITURN);
        }
        */

    }

    public void SetPlayerBid(int new_bid)
    {
        playerBid = new_bid;
    }

    // have the AI place a bid based on their cards
    private void HandleAIBid()
    {
        aiBid = AIManager.Instance.GetBid();
        // tallyBoard.updateAIBidText(aiBid);

        // If player has already bid, they begin play. Otherwhise let them bid.
        if(currLead == Character.DEATH){
            GameManager.Instance.ChangePhase(Phase.PLAYERBID);
        } 
        else{
            GameManager.Instance.ChangePhase(Phase.PLAYERTURN);
        }
    }

    private void takeTrick(Character trickWinner)
    {
        if(trickWinner == Character.PLAYER)
        {
            playerTricks += 1;
        }
        else { aiTricks += 1; }

        GameManager.Instance.IncrementTurn();
        UpdateTrickUI();
    }

    // if either score is above 500, the game is over
    private bool _CheckWin()
    {
        // Temp 100 just for testing
        return (playerScore > 100 || aiScore > 100);
    }

    // helper func to calculate a score based on bid and tricks taken
    private (int, int) _CalcScore(int bid, int tricks){
        if(bid > tricks){
            return (-10 * (bid - tricks), 0); // lose ten points for every trick failed to take, get no bags
        }
        else{
            return ((10 * bid), tricks - bid); // gain ten points for every trick bid, and gain a bad for every extra trick
        }
    }

    // unsubscribe from events when destroyed to prevent errors
    void OnDestroy(){
        GameManager.OnPhaseChanged -= ScoreManagerOnPhaseChanged;
        GameManager.OnTrickTaken -= takeTrick;
    }
}
