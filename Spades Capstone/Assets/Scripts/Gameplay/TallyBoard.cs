using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallyBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject tallyBoard;
    private TextMesh playerScoreText;
    private TextMesh AIScoreText;
    private TextMesh AIBidText;
    private TextMesh PlayerBidText;

    void Start()
    {
        playerScoreText.text = "0";
        AIScoreText.text = "0";
    }

    // feed in new player and ai scores at the end of a round
    // WIP: will want more ceremony, ie a sound and anim to play
    public void updateScoreText(int playerScore, int aiScore){
        AIScoreText.text = aiScore.ToString();
        playerScoreText.text = playerScore.ToString();
    }

    public void updateAIBidText(int newBid){
        AIBidText.text = newBid.ToString();
    }

    public void updatePlayerBidText(int newBid){
        PlayerBidText.text = newBid.ToString();
    }
}
