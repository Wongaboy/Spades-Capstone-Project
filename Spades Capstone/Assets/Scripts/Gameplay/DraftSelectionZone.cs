using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftSelectionZone : MonoBehaviour
{
    [SerializeField] private bool decisionToKeep;

    private void Start()
    {
        CardInteraction.OnCardMove += OnCardInteract;
        CardInteraction.OnCardFinMove += OnEndInteraction;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager.Instance.DraftDecision(decisionToKeep);
    }

    private void OnCardInteract(Phase currPhase)
    {
        if (currPhase == Phase.PLAYERDRAFT) { gameObject.SetActive(false); }
    }

    private void OnEndInteraction(Phase currPhase)
    {
        if (currPhase == Phase.PLAYERDRAFT) { gameObject.SetActive(true); }
    }

    private void OnDestroy()
    {
        CardInteraction.OnCardMove -= OnCardInteract;
        CardInteraction.OnCardFinMove -= OnEndInteraction;
    }
}
