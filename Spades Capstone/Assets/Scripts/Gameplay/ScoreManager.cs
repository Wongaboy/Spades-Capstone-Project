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

    int player_Score = 0;
    int player_Bags = 0;

    int death_Score = 0;
    int death_Bags = 0;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Function to calculate result of round (i.e Who Won?)
    public void CheckRoundResult()
    {
        // Check Death's Played Card

        // Check Player's Played Card

        // Compare Cards

        // Update Respective Scores Based on result
        
    }

    public void CheckWin()
    {
        // Check if player_Score > 500

        // Check if death_Score > 500
    }
}
