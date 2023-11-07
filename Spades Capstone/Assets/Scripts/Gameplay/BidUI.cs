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
    [SerializeField] TMP_Text CurrentBid_Text;
    private int current_bid;



    // Start is called before the first frame update
    void Start()
    {
        BidUIPanel.SetActive(false);

        // !!Previous Version Code!!
        // GameManager.OnPhaseChanged += BidOnPhaseChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        current_bid = 0;
        CurrentBid_Text.text = current_bid.ToString();
    }

    // Function for UI buttons to increment/decrement BID selector
    public void ChangeBid(int change)
    {
        current_bid += change;
        // Limit Bid to 0-13
        LimitBid();
        CurrentBid_Text.text = current_bid.ToString();
    }

    // Limits selector Bid to 0-13
    private void LimitBid()
    {
        // If currentbid is Out of Bounds, set it to the boundary value
        if (current_bid > 13) { current_bid = 13; }
        else if (current_bid < 0) { current_bid = 0; }
    }

    // Confirm Players Bid and Switch to Appropriate Phase
    public void ConfirmPlayerBid()
    {
        // Disable UI before Switching
        ToggleBidUI(false);
        // Change Player bid to current_bid
        ScoreManager.Instance.SetPlayerBid(current_bid);

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
