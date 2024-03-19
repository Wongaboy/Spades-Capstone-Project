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
    [SerializeField] private TMP_Text AIName;
    [SerializeField] private CardDisplay PlayerCard;
    [SerializeField] private CardDisplay AICard;
    [SerializeField] private VFXPlayer puffFX;

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
        SoundFXManager.Instance.PlayChalkSFX(gameObject.transform, .2f);
        puffFX.TriggerFX(PlayerScoreText.gameObject.transform);
        PlayerScoreText.text = playerScore.ToString();
        puffFX.TriggerFX(AIScoreText.gameObject.transform);
        AIScoreText.text = aiScore.ToString();
    }

    // Feed in new player and ai BAGS at the end of a Phase.SCORING
    // WIP: will want more ceremony, ie a sound and anim to play
    public void updateBagText(int playerBag, int aiBag)
    {
        SoundFXManager.Instance.PlayChalkSFX(gameObject.transform, .2f);
        puffFX.TriggerFX(PlayerBagText.gameObject.transform);
        PlayerBagText.text = playerBag.ToString();
        puffFX.TriggerFX(AIBagText.gameObject.transform);
        AIBagText.text = aiBag.ToString();
    }

    // Feed in new player OR ai TRICKS at the end of a Phase.ENDOFTRICK
    // WIP: will want more ceremony, ie a sound and anim to play
    public void updateTrickText(Character character, int newTrick)
    {
        SoundFXManager.Instance.PlayChalkSFX(gameObject.transform, .2f);
        if (character == Character.DEATH)
        {
            puffFX.TriggerFX(AITrickText.gameObject.transform);
            AITrickText.text = newTrick.ToString();
        }
        else 
        {
            puffFX.TriggerFX(PlayerTrickText.gameObject.transform);
            PlayerTrickText.text = newTrick.ToString(); 
        }        
    }

    // Feed in new player OR ai BIDS at the end of a Phase.BID
    // WIP: will want more ceremony, ie a sound and anim to play
    public void updateBidText(Character character, int newBid)
    {
        SoundFXManager.Instance.PlayChalkSFX(gameObject.transform, .2f);
        if (character == Character.DEATH)
        { 
            AIBidText.text = newBid.ToString();
            puffFX.TriggerFX(AIBidText.gameObject.transform);
        }
        else
        { 
            PlayerBidText.text = newBid.ToString();
            puffFX.TriggerFX(PlayerBidText.gameObject.transform);
        }
    }

    // might want tp have the chalk vfx here, but since sfx aren't we should be consistent for now
    public void updatePlayedCard(Character character, Card card)
    {
        // SoundFXManager.Instance.PlayChalkSFX(gameObject.transform, .2f);
        if (character == Character.DEATH) {
            AICard.show(card);
        }
        else
        {
            PlayerCard.show(card);
        }
    }

    public void updateAIName(string name)
    {
        AIName.text = name;
        SoundFXManager.Instance.PlayChalkSFX(gameObject.transform, .2f);
        puffFX.TriggerFX(AIName.gameObject.transform);

    }

    public void clearPlayedCard()
    {
        AICard.hide();
        PlayerCard.hide();
    }
}
