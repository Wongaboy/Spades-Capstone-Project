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

    void Start()
    {
        
    }

    private void OnDestroy()
    {

    }

    public void startDraft()
    {

    }

    public void endDraft()
    {
        if(GameManager.Instance.lead == Character.DEATH)
        {
            GameManager.Instance.ChangePhase(Phase.AITURN);
        }
        else
        {
            GameManager.Instance.ChangePhase(Phase.PLAYERTURN);
        }
    }
}
