using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TallyBoard : MonoBehaviour
{
    #region "Singleton"
    private static TallyBoard _instance;

    public static TallyBoard Instance { get { return _instance; } }

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

    [SerializeField]
    private GameObject tallyBoard;
    [SerializeField] private TMP_Text PlayerScoreText;
    [SerializeField] private TMP_Text AIScoreText;
    [SerializeField] private TMP_Text PlayerBagText;
    [SerializeField] private TMP_Text AIBagText;
    [SerializeField] private TMP_Text PlayerTrickText;
    [SerializeField] private TMP_Text AITrickText;
    [SerializeField] private TMP_Text PlayerBidText;
    [SerializeField] private TMP_Text AIBidText;

    void Start()
    {
        PlayerScoreText.text = "0";
        AIScoreText.text = "0";
    }

    // feed in new player and ai scores at the end of a round
    // WIP: will want more ceremony, ie a sound and anim to play
    public void updateScoreText(int playerScore, int aiScore){
        PlayerScoreText.text = playerScore.ToString();
        AIScoreText.text = aiScore.ToString();
    }

    public void updateBagText(int playerBag, int aiBag)
    {
        PlayerBagText.text = playerBag.ToString();
        AIBagText.text = aiBag.ToString();
    }

    public void updateTrickText(Character character, int newTrick)
    {
        if (character == Character.DEATH)
        { AITrickText.text = newTrick.ToString(); }
        else 
        { PlayerTrickText.text = newTrick.ToString(); }        
    }

    public void updateBidText(Character character, int newBid)
    {
        if (character == Character.DEATH)
        { AIBidText.text = newBid.ToString(); }
        else
        { PlayerBidText.text = newBid.ToString(); }
    }
}
