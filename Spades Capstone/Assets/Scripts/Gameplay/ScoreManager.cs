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
    #endregion

    #region "Class Variables"

    int playerScore = 0;
    int playerBags = 0;
    int deathScore = 0;
    int deathBags = 0;
    TallyBoard tallyBoard;

    #endregion

    // start listening to the game manager 
    void Start()
    {
        GameManager.OnPhaseChanged += ScoreManagerOnPhaseChanged;
    }

    // when the phase is changed, see if it's time for scoring
    private void ScoreManagerOnPhaseChanged(Phase phase){
        if(phase == Phase.SCORING){
            HandleScoring(); // do everything that is required for scoring
        }
        else if(phase == Phase.PLAYERBID){
            // idk how to do that
        }
        else if(phase == Phase.AIBID){
            
        }
    }

    // tally up the points for the round, add them to total, and see if the game is over
    private void HandleScoring(){
        bool gameover = CheckWin();
    }

    // if either score is above 500, the game is over
    private bool CheckWin()
    {
        return (playerScore > 500 || deathScore > 500);
    }
}
