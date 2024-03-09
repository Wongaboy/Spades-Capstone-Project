using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BidUI : MonoBehaviour
{
    #region "Singleton"
    private static BidUI _instance;

    public static BidUI Instance { get { return _instance; } }

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
    // UI Panel Object
    [SerializeField] GameObject BidUIPanel;
    private int current_bid;

    // Start is called before the first frame update
    void Start()
    {
        BidUIPanel.SetActive(false);
        BidButton.setBid += getBid;

        // !!Previous Version Code!!
        // GameManager.OnPhaseChanged += BidOnPhaseChanged;
    }

    void getBid(int bid)
    {
        current_bid = bid;
    }

    // !!Previous Version Code!!
    // On Phase Change to Phase.PlayerDraft
    private void BidOnPhaseChanged(Phase phase)
    {
        //if (phase == Phase.PLAYERBID)
        //{
        //    ToggleBidUI(true);
        //}
    }

    // Called to Trigger Bid UI Prompt for Player to Select Bid
    public void ToggleBidUI(bool ui_state)
    {
        BidUIPanel.SetActive(ui_state);

        // Initialize Selector UI
        current_bid = -1;
    }

    // Confirm Players Bid and Switch to Appropriate Phase
    public void ConfirmPlayerBid()
    {
        // Way to prevent no ConfirmBid when the Player has not selected a peg yet
        if (current_bid == -1) { return; }

        // Change Player bid to current_bid
        ScoreManager.Instance.SetPlayerBid(current_bid);
        // Disable UI before Switching
        ToggleBidUI(false);
        // Change Phase based on if you were lead (LEAD = AI -> Phase.AITURN; LEAD = PLAYER -> Phase.AIBID)
        if (GameManager.Instance.lead == Character.DEATH)
        {
            GameManager.Instance.ChangePhase(Phase.AITURN);
        }
        else
        {
            GameManager.Instance.ChangePhase(Phase.AIBID);
        }
    }
}
