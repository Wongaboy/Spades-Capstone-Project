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

    [SerializeField] GameObject TurnUIPanel;
    [SerializeField] TMP_Text AISuit_Text;
    [SerializeField] TMP_Text AIValue_Text;
    [SerializeField] TMP_Text PlayerSuit_Text;
    [SerializeField] TMP_Text PlayerValue_Text;

    //private Card player_card;
    //private Card ai_card;


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

    public void TurnOnPhaseChanged(Phase phase)
    {
        if (phase == Phase.PLAYERTURN)
        {
            ToggleTurnUI(true);
        }
    }

    public void ToggleTurnUI(bool ui_state)
    {
        if (ui_state == false)
        {
            PlayerSuit_Text.text = "";
            PlayerValue_Text.text = "";
            AISuit_Text.text = "";
            AIValue_Text.text = "";
        }
        TurnUIPanel.SetActive(ui_state);
    }

    public void UpdatePlayerCardInfo(Card card)
    {
        PlayerSuit_Text.text = card.suit.ToString();
        PlayerValue_Text.text = card.val.ToString();
    }

    public void UpdateAICardInfo(Card card)
    {
        AISuit_Text.text = card.suit.ToString();
        AIValue_Text.text = card.val.ToString();
    }

    public void ClearCardInfo()
    {
        PlayerSuit_Text.text = "";
        PlayerValue_Text.text = "";
        AISuit_Text.text = "";
        AIValue_Text.text = "";
    }

    public void OnPlayClicked()
    {
        PlayerManager.Instance.HandlePlayerTurn();
    }

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
