using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallyBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject tallyBoard;
    private TextMesh playerScoreText;
    private TextMesh AIScoreText;

    void Start()
    {
        playerScoreText.text = "0";
        AIScoreText.text = "0";
    }

    // feed in new player and ai scores at the end of a round
    public void updateText(int playerScore, int aiScore){
        AIScoreText.text = aiScore.ToString();
        playerScoreText.text = playerScore.ToString();
    }
}
