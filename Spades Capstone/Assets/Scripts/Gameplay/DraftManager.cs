using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftManager : MonoBehaviour
{
    #region "Singleton"
    private static DraftManager _instance;

    public static DraftManager Instance { get { return _instance; } }

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

    #region "Class Reference/ Variables"
    private Deck deck;
    private Character lead;
    private Character drafter;
    #endregion

    void Start()
    {
        deck = GameManager.Instance.deck;
        lead = GameManager.Instance.lead;
    }

    public void startDraft()
    {
        drafter = lead;
        bool drafting = true;
        while(drafting){
            // yield HandleTurn(drafter)
        }
        
    }

    public void endDraft()
    {
        if(lead == Character.DEATH)
        {
            GameManager.Instance.ChangePhase(Phase.AITURN);
        }
        else
        {
            GameManager.Instance.ChangePhase(Phase.PLAYERTURN);
        }
    }

    private void HandleTurn(Character drafter){


    }
}
