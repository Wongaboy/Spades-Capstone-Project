using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for updating the UI as draft phase moves along - might be unnecessary
public class DraftManager : MonoBehaviour
{
    private Deck deck;
    private Character lead;

    void Start()
    {
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
