using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDecisionZone : MonoBehaviour
{
    private void Start()
    {
        CardInteraction.OnCardMove += OnCardInteract;
        CardInteraction.OnCardFinMove += OnEndInteraction;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager.Instance.PlayCard(other.gameObject.GetComponent<Card>()); // IM SORRY IK ITS BAD FUCKKKKKK
    }

    private void OnCardInteract(Phase currPhase)
    {
        if (currPhase == Phase.PLAYERTURN) { gameObject.SetActive(false); }
    }

    private void OnEndInteraction(Phase currPhase)
    {
        if(currPhase == Phase.PLAYERTURN) { gameObject.SetActive(true); }
    }

    private void OnDestroy()
    {
        CardInteraction.OnCardMove -= OnCardInteract;
        CardInteraction.OnCardFinMove -= OnEndInteraction;
    }
}
