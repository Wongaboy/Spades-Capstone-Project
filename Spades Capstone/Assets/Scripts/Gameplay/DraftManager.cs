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
    #endregion

    void Start()
    {
        deck = GameManager.Instance.deck;
        lead = GameManager.Instance.lead;
    }

    // transition from Draft into the next phase
    private void endDraft()
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

}
