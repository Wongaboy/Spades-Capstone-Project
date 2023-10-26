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

    private Hand playerHand;
    private int currentPlayerBid;
    private Character _thisCharacter = Character.PLAYER;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnPhaseChanged -= PlayerManagerOnPhaseChanged;
    }

    private void PlayerManagerOnPhaseChanged(Phase phase){
        if(phase == Phase.PLAYERTURN){
            HandlePlayerTurn();
        }
    }

    // !THESE FUNCTIONS ARE JUST STUFF COPIED FROM DEATH, CAN CHANGE THEM LATER!

    // Function to call when Decides to Player keeps or dumps the drawn card
    // !May become obsolete depending on how we implement Card Drawing (i.e What object/script performs the "draw")
    public bool DraftDecision(Card card)
    {
        // !Work In Progress!
        return true;
    }

    // Function to call to set Player's Bid
    public int SetBid(int bid)
    {
        currentPlayerBid = bid;
        return currentPlayerBid;
    }

    // Function to call to Play Chosen Card
    public void PlayCard(Card played_card)
    {
        // !Work In Progress!
    }

    // Used to check valid moves (Ex. Only allow same suit card if opponent plays a suit you have)
    public bool CheckValidMove(Card picked_card)
    {
        // !Work In Progress!

        return true; // Temp value
    }

    
    private void HandlePlayerTurn(){

    }

    // unsubscribe from events when destroyed to prevent errors
    void OnDestroy(){
        GameManager.OnPhaseChanged -= PlayerManagerOnPhaseChanged;
    }
}
