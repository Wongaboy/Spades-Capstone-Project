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

    private Card[] Player_Hand;
    private int current_PlayerBid;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        current_PlayerBid = bid;
        return current_PlayerBid;
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
}
