using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftSelectionZone : MonoBehaviour
{
    [SerializeField] private bool decisionToKeep;

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager.Instance.DraftDecision(decisionToKeep);
    }
}
