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

    [SerializeField] GameObject BidUIPanel;
    [SerializeField] TMP_Text CurrentBid_Text;
    private int current_bid;



    // Start is called before the first frame update
    void Start()
    {
        BidUIPanel.SetActive(false);

        GameManager.OnPhaseChanged += BidOnPhaseChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // On Phase Change to Phase.PlayerDraft
    private void BidOnPhaseChanged(Phase phase)
    {
        //if (phase == Phase.PLAYERBID)
        //{
        //    ToggleBidUI(true);
        //}
    }

    public void ToggleBidUI(bool ui_state)
    {
        BidUIPanel.SetActive(ui_state);
        current_bid = 0;
        CurrentBid_Text.text = current_bid.ToString();
    }

    public void ChangeBid(int change)
    {
        current_bid += change;
        LimitBid();
        CurrentBid_Text.text = current_bid.ToString();
    }

    private void LimitBid()
    {
        if (current_bid > 13) { current_bid = 13; }
        else if (current_bid < 0) { current_bid = 0; }
    }

    public void ConfirmPlayerBid()
    {
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

        ToggleBidUI(false);
    }
}
