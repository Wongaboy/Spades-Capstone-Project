using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftManager : MonoBehaviour
{
    void Awake()
    {
        GameManager.OnPhaseChanged += startDraft;
    }

    private void OnDestroy()
    {
        GameManager.OnPhaseChanged -= startDraft;
    }

    public void startDraft(Phase phase)
    {
        if(phase == Phase.DRAFT)
        {

        }
    }
}
