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

    
    [SerializeField] HandUI playerHandUI;
    [SerializeField] Hand playerHand;
    [SerializeField] private Transform displaySpot;
    [SerializeField] private GameObject[] DraftZones;
    [SerializeField] private GameObject PlayZone;
    private Card cardToDraft;

    //private Character thisCharacter = Character.PLAYER;
    [HideInInspector]
    public bool isLead = false;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnPhaseChanged += PlayerManagerOnPhaseChanged;
        // playerHandUI.AssignHand(playerHand);
    }

    private void PlayerManagerOnPhaseChanged(Phase phase){
        if(phase == Phase.PLAYERTURN)
        {
            PlayZone.SetActive(true);
            HandlePlayerTurn();
        }
        else if(phase == Phase.PLAYERDRAFT)
        {
            DisplayCardToDraft();
            DraftZones[0].SetActive(true);
            DraftZones[1].SetActive(true);
        }
        else if(phase == Phase.PLAYERBID)
        {
            // Like AI, this is handled rn in the ScoreManager
            // SetBid(int bid)
        }
    }
    #region "Draft Handling"
    private void DisplayCardToDraft()
    {
        cardToDraft = GameManager.Instance.DrawCard();
        cardToDraft.Freeze(); // prevent card from falling in space
        cardToDraft.MoveToLocation(displaySpot.position, displaySpot.rotation);
        cardToDraft.SetInteractable(true);
        Debug.Log(cardToDraft);
    }

    // Function call to add a card to hand
    private void DraftCard(Card card)
    {
        card.SetInteractable(false);
        playerHand.AddCardToHand(card);
        playerHandUI.ShowCard(card);
        GameManager.Instance.ChangePhase(Phase.AIDRAFT);
    }

    public void DraftDecision(bool decision)
    {
        DraftZones[0].SetActive(false);
        DraftZones[1].SetActive(false);
        if (decision)
        {
            DraftCard(cardToDraft);
            GameManager.Instance.DiscardCardFromDeck();
        }
        else
        {
            GameManager.Instance.DiscardCardFromHand(cardToDraft);
            DraftCard(GameManager.Instance.DrawCard());
        }
    }
    #endregion

    // Changes Lead Boolean (bool is for Trick Lead, NOT Draft/Bid Lead)
    public void ChangeInternalLead(bool isCurrLead)
    {
        isLead = isCurrLead;
    }

    // Used to check valid moves (Ex. Only allow same suit card if opponent plays a suit you have)
    public bool CheckValidMove(Card selected)
    {
        if(isLead && selected.suit == Suit.SPADE && !GameManager.Instance.spadesBroken)
        { 
            return false; // cannot lead a Spade if spades haven't been played yet
        }
        else if(!isLead && selected.suit != GameManager.Instance.aiCard.suit)
        {
            if (playerHand.HasSuit(GameManager.Instance.aiCard.suit))
            {
                return false; // must follow suit if you can
            }
        }
        return true;
    }
    #region "Turn Handling"
    // allow cards to be moveable again
    public void HandlePlayerTurn(){
        playerHandUI.AlterCardInteraction(true);
    }

    // Function to call to attempt to play a card
    public void PlayCard(Card playedCard)
    {
        if(CheckValidMove(playedCard)){
            GameManager.Instance.playerCard = playedCard;
            playerHand.RemoveCardFromHand(playedCard);
            playerHandUI.ShowCardPlayed(playedCard);
            EndTurn();
        }
        else
        {
            // tell player this card could not be played, move it back into their hand -- WIP
            // also here is place for easter egg
        }
    }

    private void EndTurn()
    {
        PlayZone.SetActive(false);
        if(isLead)
        {
            GameManager.Instance.ChangePhase(Phase.AITURN);
        }
        else { GameManager.Instance.ChangePhase(Phase.ENDOFTRICK); }
    }
    #endregion

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
