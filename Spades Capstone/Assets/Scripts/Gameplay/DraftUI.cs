using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DraftUI : MonoBehaviour
{
    #region "Singleton"
    private static DraftUI _instance;

    public static DraftUI Instance { get { return _instance; } }

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

    [SerializeField] GameObject DraftUIPanel;
    [SerializeField] TMP_Text cardSuit_Text;
    [SerializeField] TMP_Text cardValue_Text;
    Card decision_Card;
    
    // Start is called before the first frame update
    void Start()
    {
        DraftUIPanel.SetActive(false);

        GameManager.OnPhaseChanged += DraftUIOnPhaseChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // On Phase Change to Phase.PlayerDraft
    private void DraftUIOnPhaseChanged(Phase phase)
    {
        if(phase == Phase.PLAYERDRAFT)
        {
            // Deal/Draw Card
            decision_Card = GameManager.Instance.DrawCard();
            // Toggle UI for Player to See
            ToggleDraftUI(true);
        }
    }


    // Called to Trigger Draft UI Prompt for Player to Choose Card
    public void ToggleDraftUI(bool ui_state)
    {
        DraftUIPanel.SetActive(ui_state);
        // Display Card info to Player
        cardSuit_Text.text = decision_Card.suit.ToString();
        cardValue_Text.text = decision_Card.val.ToString();

    }

    // Function for when Players KEEPs Drawn Card (KEEP Button Pressed)
    public void OnKeepClick()
    {
        // Disable UI
        ToggleDraftUI(false);

        // Add Card to Player's Hand
        PlayerManager.Instance.DraftCard(decision_Card);

        // Discard Next Card in Deck
        GameManager.Instance.DiscardCard();

        GameManager.Instance.IncrementDraftTurn();
        // Switch Turns
        Decide_ChangePhase();
    }

    // Function for when Players DISCARDs Drawn Card (DISCARD Button Pressed)
    public void OnDiscardClick()
    {
        // Disable UI 
        ToggleDraftUI(false);

        // "Discard" currently Viewed Card (i.e Overwrite it with new one)
        decision_Card = GameManager.Instance.DrawCard();

        // Add Next Card to Player Hand
        PlayerManager.Instance.DraftCard(decision_Card);

        GameManager.Instance.IncrementDraftTurn();
        // Switch Turns
        Decide_ChangePhase();
    }

    public void Decide_ChangePhase()
    {
        if (GameManager.Instance.GetDraftTurn() >= 26)
        {
            GameManager.Instance.ChangePhase(Phase.AIBID);
        }
        else
        {
            GameManager.Instance.ChangePhase(Phase.AIDRAFT);
        }
    }
}
