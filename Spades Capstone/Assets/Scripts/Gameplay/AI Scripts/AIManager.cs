using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    #region "Singleton"
    private static AIManager _instance;

    public static AIManager Instance { get { return _instance; } }

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

    [SerializeField] Hand aiHand;
    private int currentBid;
    private Character thisCharacter = Character.DEATH;
    private bool isLead = true;

    // Start is called before the first frame update
    void Start(){
        GameManager.OnPhaseChanged += AIManagerOnPhaseChanged;
    }

    private void AIManagerOnPhaseChanged(Phase phase){
        if(phase == Phase.AITURN)
        {
            HandleAITurn();
        }
        else if(phase == Phase.AIDRAFT)
        {
            MakeDraftDecision(GameManager.Instance.DrawCard());
            GameManager.Instance.IncrementDraftTurn();
            StartCoroutine(WaitTimeDraft());
        }
        else if(phase == Phase.AIBID)
        {
            // GetBid();
        }
    }

    // Function to Decide if Death keeps or dumps the drawn card
    private bool MakeDraftDecision(Card card)
    {
        // !Work In Progress!

        // Temporary Formula:
        // If SPADE -> KEEP
        // Else -> DISCARD

        if (card.suit == Suit.SPADE)
        {
            DraftCard(card);
            GameManager.Instance.DiscardCard();
        }
        else
        {
            DraftCard(GameManager.Instance.DrawCard());
        }

        return true;
    }

    // Function to call to Add card to AI Hand
    public void DraftCard(Card card)
    {
        // !Work In Progress!
        aiHand.AddCardToHand(card);

        /* Debug Code
        Debug.Log("AI Drafted Card");
        Debug.Log(playerHand.NumOfSuit(Suit.DIAMOND));
        Debug.Log(playerHand.NumOfSuit(Suit.SPADE));
        Debug.Log(playerHand.NumOfSuit(Suit.CLUB));
        Debug.Log(playerHand.NumOfSuit(Suit.HEART));
        */
    }

    // Function to calculate Death's Bid amount based on Death's Hand
    public int GetBid()
    {
        // !Work In Progress!
        float bidEstimate = 0.0f;
        
        // add a bid for every !! non Spades !! suit Death is low in
        for(int i = 0; i< 4; i++){
            Suit currSuit = Deck.intToSuit[i];
            int numCurrSuit = aiHand.NumOfSuit(currSuit);
            if( numCurrSuit <= 2 && i != 0){ // might want to refine this later
                bidEstimate += 1.0f; 
            }

            // add a bid for every ace
            if(aiHand.HasValue(currSuit, 14)){
                bidEstimate += 1.0f;
            }

            // add a half bid for every King
            if(aiHand.HasValue(currSuit, 13)){
                bidEstimate += 0.5f;
            }
        }

        // Anthony Test Code
        bidEstimate += aiHand.NumOfSuit(Suit.SPADE);


        currentBid = (int)bidEstimate; // need to make sure this rounds correctly

        return currentBid; 
    }

    public void ChangeInternalLead(bool new_lead)
    {
        isLead = new_lead;
    }

    private void HandleAITurn(){

        PlayCard();

        StartCoroutine(WaitTimePlayCard());
        // if there is dialogue to play, might want to activate it here       
        //if (isLead == true)
        //{
        //    GameManager.Instance.ChangePhase(Phase.PLAYERTURN);
        //}
        //else
        //{
        //    GameManager.Instance.ChangePhase(Phase.ENDOFTRICK);
        //}
    }

    // Function to Calculate Logic for what Death plays based on Death's hand
    private void PlayCard()
    {
        Card tobe_played;
        if (isLead)
        {
            tobe_played = ChooseCardToLead();
        }
        else
        {
            tobe_played = ChooseCardToFollow(GameManager.Instance.playerCard);
        }

        GameManager.Instance.aiCard = tobe_played;
        TurnUI.Instance.UpdateAICardInfo(tobe_played);
        aiHand.RemoveCardFromHand(tobe_played);
    }
    // 
    private Card ChooseCardToLead()
    {
        // Temp Code
        return aiHand.GetAllCards()[0]; // INCOMPLETE
    }
    private Card ChooseCardToFollow(Card playerCard)
    {
        return aiHand.GetAllCards()[0]; // INCOMPLETE
    }

    public void Decide_DraftChangePhase()
    {
        if (GameManager.Instance.GetDraftTurn() >= 26)
        {
            GameManager.Instance.ChangePhase(Phase.PLAYERBID);
        }
        else
        {
            GameManager.Instance.ChangePhase(Phase.PLAYERDRAFT);
        }
    }

    private IEnumerator WaitTimeDraft()
    {      
        yield return new WaitForSeconds(1);

        Decide_DraftChangePhase();
    }

    private IEnumerator WaitTimeBid()
    {
        yield return new WaitForSeconds(1);

        Decide_DraftChangePhase();
    }

    private IEnumerator WaitTimePlayCard()
    {
        yield return new WaitForSeconds(1);

        if (isLead == true)
        {
            GameManager.Instance.ChangePhase(Phase.PLAYERTURN);
        }
        else
        {
            GameManager.Instance.ChangePhase(Phase.ENDOFTRICK);
        }
    }

    public void Debug_ShowHand()
    {
        Debug.Log(aiHand.NumOfSuit(Suit.DIAMOND));
        Debug.Log(aiHand.NumOfSuit(Suit.SPADE));
        Debug.Log(aiHand.NumOfSuit(Suit.CLUB));
        Debug.Log(aiHand.NumOfSuit(Suit.HEART));
    }

    // unsubscribe from events when destroyed to prevent errors
    void OnDestroy(){
        GameManager.OnPhaseChanged -= AIManagerOnPhaseChanged;
    }
}
