using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    // Function for when Players KEEPs Drawn Card (KEEP Button Pressed)
    public void OnKeepClick()
    {
        // Disable UI
        ToggleDraftUI(false);

        // Add Card to Player's Hand
            // PlayerManager.Instance.Hand.AddCardToHand(decision_Card);

        // Discard Next Card in Deck
        GameManager.Instance.DiscardCard();

        // Switch Turns
        // (Need to add check somewhere for IF need to switch to BID PHASE)
        GameManager.Instance.ChangePhase(Phase.AIDRAFT);
    }

    // Function for when Players DISCARDs Drawn Card (DISCARD Button Pressed)
    public void OnDiscardClick()
    {
        // Disable UI 
        ToggleDraftUI(false);

        // Discard currently Viewed Card (Already gets discarded)
        // Draw new Card
        decision_Card = GameManager.Instance.DrawCard();

        // Add Next Card to Player Hand
        // PlayerManager.Instance.Hand.AddCardToHand(decision_Card);

        // Switch Turns
        // (Need to add check somewhere for IF need to switch to BID PHASE)
        GameManager.Instance.ChangePhase(Phase.AIDRAFT);
    }
}
