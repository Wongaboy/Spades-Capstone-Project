using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void OnDestroy(){
        GameManager.OnPhaseChanged -= ScoreManagerOnPhaseChanged;
    }
    #endregion

    #region "Class Variables"
    int playerBid = 0;
    int aiBid = 0;
    int playerScore = 0;
    int playerBags = 0;
    int deathScore = 0;
    int deathBags = 0;
    Character currLead;
    TallyBoard tallyBoard;

    #endregion

    // start listening to the game manager 
    void Start()
    {
        GameManager.OnPhaseChanged += ScoreManagerOnPhaseChanged;
    }

    // when the phase is changed, see if it's time for scoring
    private void ScoreManagerOnPhaseChanged(Phase phase){
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

    // tally up the points for the round, add them to total, and see if the game is over
    private void HandleScoring(){
        
        
        
        if(CheckWin()){
            // end the game
        }
    }

    // make a ui element visible/ glowing to the player that allows them to choose their bid
    private void HandlePlayerBid(){
        
        /* use this logic when exiting player bid logic
        if(currLead == Character.PLAYER){
            GameManager.Instance.ChangePhase(Phase.AIBID);
        } 
        else{
            GameManager.Instance.ChangePhase(Phase.AITURN);
        }
        */

    }

    // have the AI place a bid based on their cards
    private void HandleAIBid(){
        aiBid = AIManager.Instance.GetBid();
        tallyBoard.updateAIBidText(aiBid);

        // If player has already bid, they begin play. Otherwhise let them bid.
        if(currLead == Character.DEATH){
            GameManager.Instance.ChangePhase(Phase.PLAYERBID);
        } 
        else{
            GameManager.Instance.ChangePhase(Phase.PLAYERTURN);
        }
    }

    // if either score is above 500, the game is over
    private bool CheckWin()
    {
        return (playerScore > 500 || deathScore > 500);
    }
}
