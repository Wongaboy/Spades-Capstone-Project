using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnUI : MonoBehaviour
{
    #region "Singleton"
    private static TurnUI _instance;

    public static TurnUI Instance { get { return _instance; } }

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
    [SerializeField] GameObject TurnUIPanel;

    // Played Cards Info (Player and AI)
    [SerializeField] TMP_Text AISuit_Text;
    [SerializeField] TMP_Text AIValue_Text;
    [SerializeField] TMP_Text PlayerSuit_Text;
    [SerializeField] TMP_Text PlayerValue_Text;

    // Start is called before the first frame update
    void Start()
    {
        TurnUIPanel.SetActive(false);

        GameManager.OnPhaseChanged += TurnOnPhaseChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // On Phase Change to Phase.PlayerTurn
    public void TurnOnPhaseChanged(Phase phase)
    {
        if (phase == Phase.PLAYERTURN)
        {
            ToggleTurnUI(true);
        }
    }

    // Called to Trigger Turn UI Prompt for Player to Play a Card
    public void ToggleTurnUI(bool ui_state)
    {
        // When UI is toggled off Reset the info
        if (ui_state == false)
        {
            ClearCardInfo();
        }
        TurnUIPanel.SetActive(ui_state);
    }

    // Update Player's Played Card
    public void UpdatePlayerCardInfo(Card card)
    {
        PlayerSuit_Text.text = card.suit.ToString();
        PlayerValue_Text.text = card.val.ToString();
    }

    // Update AI's Played Card
    public void UpdateAICardInfo(Card card)
    {
        AISuit_Text.text = card.suit.ToString();
        AIValue_Text.text = card.val.ToString();
    }

    // Clears Card UI info
    public void ClearCardInfo()
    {
        PlayerSuit_Text.text = "";
        PlayerValue_Text.text = "";
        AISuit_Text.text = "";
        AIValue_Text.text = "";
    }

    // Handles Player trying to play card
    public void OnPlayClicked()
    {
        PlayerManager.Instance.HandlePlayerTurn();
    }

    // Decides which Phase to switch to
    public void Decide_ChangePhase()
    {
        if (PlayerManager.Instance.isLead == true)
        {
            GameManager.Instance.ChangePhase(Phase.AITURN);
        }
        else
        {
            GameManager.Instance.ChangePhase(Phase.ENDOFTRICK);
        }
    }
}
