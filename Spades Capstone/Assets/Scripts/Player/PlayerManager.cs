using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    [SerializeField] private DialogueSO easteregg;
    private Card cardToDraft;
    [SerializeReference] private GameObject cardAmountDisplay;
    [SerializeReference] private TMP_Text cardAmountText;
    //private Character thisCharacter = Character.PLAYER;
    [HideInInspector]
    public bool isLead = false;
    private int numRulebreakAttempts = 0;


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
        cardToDraft.MoveToLocation(displaySpot.position, displaySpot.rotation, false, true);
        cardToDraft.SetInteractable(true);
        // Debug.Log(cardToDraft);
    }

    // Function call to add a card to hand
    private void DraftCard(Card card)
    {
        card.SetInteractable(false);
        playerHand.AddCardToHand(card);
        UpdateCardAmountText();
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
            return playerHand.HasNonSpades(); // cannot lead a Spade if spades haven't been played yet OR you have non spades
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

    // Change Card Amount Display
    public void UpdateCardAmountText()
    {
        cardAmountText.text = playerHand.NumberofCards() + "/13";
    }

    // Toggle Card Amount Display
    public void ToggleCardAmountDisplay(bool activeState)
    {
        cardAmountDisplay.SetActive(activeState);
    }

    #region "Turn Handling"
    // allow cards to be moveable again
    public void HandlePlayerTurn(){
        // Toggle On all the Card Border VFX
        foreach(Card card in playerHand.GetAllCards())
        {
            card.ToggleOnVFXBorder(true, CheckValidMove(card));
        }

        playerHandUI.AlterCardInteraction(true);
    }

    // Function to call to attempt to play a card
    public void PlayCard(Card playedCard)
    {
        if(CheckValidMove(playedCard)){
            GameManager.Instance.playerCard = playedCard;
            // If card has Dialogue attached Enqueue it if the timing is appropriate
            if (playedCard.HasDialogueAttached() && GameManager.Instance.isInTutorial == false && AIManager.Instance.GetCheatPhase() != AICheatPhase.NoCheats)
            {
                DialogueManager.Instance.EnqueueDialogueSO(playedCard.GetDialogueSO(), false);
            }
            playerHand.RemoveCardFromHand(playedCard);
            playedCard.ToggleOnVFXBorder(false, false);
            UpdateCardAmountText();
            playerHandUI.ShowCardPlayed(playedCard);
            EndTurn();
        }
        else
        {
            // RETURN CARD BACK TO HAND HERE
            playerHandUI.ReturnCardToHand(playedCard);
            numRulebreakAttempts++;
            if (numRulebreakAttempts > 5) {
                DialogueManager.Instance.EnqueueDialogueSO(easteregg, true);
            }
        }
    }

    private void EndTurn()
    {
        // Turn Off Glow on Cards
        foreach (Card card in playerHand.GetAllCards())
        {
            card.ToggleOnVFXBorder(false, false);
        }

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
