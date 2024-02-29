using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    // All the Text for TallyBoard, Will eventually be replaced with Art/UI
    [SerializeField] private TMP_Text PlayerScoreText;
    [SerializeField] private TMP_Text AIScoreText;
    [SerializeField] private TMP_Text PlayerBagText;
    [SerializeField] private TMP_Text AIBagText;
    [SerializeField] private TMP_Text PlayerTrickText;
    [SerializeField] private TMP_Text AITrickText;
    [SerializeField] private TMP_Text PlayerBidText;
    [SerializeField] private TMP_Text AIBidText;
    [SerializeField] private CardDisplay PlayerCard;
    [SerializeField] private CardDisplay AICard;

    void Start()
    {
        PlayerScoreText.text = "0";
        AIScoreText.text = "0";
        HandUI.cardPlayed += updatePlayedCard;
    }

    void OnDestroy()
    {
        HandUI.cardPlayed -= updatePlayedCard;
    }


    // Feed in new player and ai SCORES during of Phase.SCORING
    // WIP: will want more ceremony, ie a sound and anim to play
    public void updateScoreText(int playerScore, int aiScore){
        PlayerScoreText.text = playerScore.ToString();
        AIScoreText.text = aiScore.ToString();
    }

    // Feed in new player and ai BAGS at the end of a Phase.SCORING
    // WIP: will want more ceremony, ie a sound and anim to play
    public void updateBagText(int playerBag, int aiBag)
    {
        PlayerBagText.text = playerBag.ToString();
        AIBagText.text = aiBag.ToString();
    }

    // Feed in new player OR ai TRICKS at the end of a Phase.ENDOFTRICK
    // WIP: will want more ceremony, ie a sound and anim to play
    public void updateTrickText(Character character, int newTrick)
    {
        if (character == Character.DEATH)
        { AITrickText.text = newTrick.ToString(); }
        else 
        { PlayerTrickText.text = newTrick.ToString(); }        
    }

    // Feed in new player OR ai BIDS at the end of a Phase.BID
    // WIP: will want more ceremony, ie a sound and anim to play
    public void updateBidText(Character character, int newBid)
    {
        if (character == Character.DEATH)
        { AIBidText.text = newBid.ToString(); }
        else
        { PlayerBidText.text = newBid.ToString(); }
    }

    public void updatePlayedCard(Character character, Card card)
    {
        if (character == Character.DEATH) {
            AICard.show(card);
        }
        else
        {
            PlayerCard.show(card);
        }
    }

    public void clearPlayedCard()
    {
        AICard.hide();
        PlayerCard.hide();
    }
}
