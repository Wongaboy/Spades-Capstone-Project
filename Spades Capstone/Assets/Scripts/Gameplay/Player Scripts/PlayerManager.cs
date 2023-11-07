using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region "Singleton"
    private static PlayerManager _instance;

    public static PlayerManager Instance { get { return _instance; } }

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

    [SerializeField] Hand playerHand;
    private int currentPlayerBid; // not sure if we need this here
    private Character thisCharacter = Character.PLAYER;
    public bool isLead = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnPhaseChanged += PlayerManagerOnPhaseChanged;
    }

    private void PlayerManagerOnPhaseChanged(Phase phase){
        // A lot of these sections get handled In the respective "name"+UI scripts
        // I believe it is possible to allow Player Manager to handle them
        if(phase == Phase.PLAYERTURN)
        {
            // HandlePlayerTurn();
        }
        else if(phase == Phase.PLAYERDRAFT)
        {
            // DraftDecision(Card card)
        }
        else if(phase == Phase.PLAYERBID)
        {
            // Like AI, this is handles rn in the ScoreManager
            // SetBid(int bid)
        }
    }

    // Function to call when Player keeps the shown card
    public void DraftCard(Card card)
    {
        playerHand.AddCardToHand(card);
    }

    // Function to call to set Player's Bid
    public int SetBid(int bid)
    {
        currentPlayerBid = bid;
        return currentPlayerBid;
    }

    // Changes Lead Boolean (bool is for Trick Lead, NOT Draft/Bid Lead)
    public void ChangeInternalLead(bool new_lead)
    {
        isLead = new_lead;
    }

    // Used to check valid moves (Ex. Only allow same suit card if opponent plays a suit you have)
    public bool CheckValidMove(Card picked_card)
    {
        if(isLead && picked_card.suit == Suit.SPADE && !GameManager.Instance.spadesBroken)
        { 
            return false; // cannot lead a Spade if spades haven't been played yet
        }
        else if(!isLead && picked_card.suit != GameManager.Instance.aiCard.suit)
        {
            if (playerHand.HasSuit(GameManager.Instance.aiCard.suit))
            {
                return false; // must follow suit if you can
            }
        }
        return true;
    }

    // activate UI that lets the player play a card - maybe move this to draft ui thing
    public void HandlePlayerTurn(){
        // !Work in Progress!

        // Plays First Card that gets returned from Hand.GetAllCards()
        Card tobe_played = playerHand.GetAllCards()[0];
        PlayCard(tobe_played);
        // Remove Card from Player's hand
        playerHand.RemoveCardFromHand(tobe_played);

        // if there is dialogue to play, might want to activate it here
        // Right Now this section below is handled in TurnUI (Can be rearranged)
        //if (isLead)
        //{
        //    GameManager.Instance.ChangePhase(Phase.AITURN);
        //}
        //else
        //{
        //    GameManager.Instance.ChangePhase(Phase.ENDOFTRICK);
        //}
    }

    // Function to call to tell the GameManager what card was played & Update UI
    private void PlayCard(Card playedCard)
    {
        GameManager.Instance.playerCard = playedCard;
        TurnUI.Instance.UpdatePlayerCardInfo(playedCard);
    }

    // Debug Function to show amount of each suits in hand
    public void Debug_ShowHand()
    {
        Debug.Log(playerHand.NumOfSuit(Suit.DIAMOND));
        Debug.Log(playerHand.NumOfSuit(Suit.SPADE));
        Debug.Log(playerHand.NumOfSuit(Suit.CLUB));
        Debug.Log(playerHand.NumOfSuit(Suit.HEART));
    }

    // unsubscribe from events when destroyed to prevent errors
    void OnDestroy(){
        GameManager.OnPhaseChanged -= PlayerManagerOnPhaseChanged;
    }
}
