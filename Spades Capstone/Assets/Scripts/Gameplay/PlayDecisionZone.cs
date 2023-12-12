using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDecisionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerManager.Instance.PlayCard(other.gameObject.GetComponent<Card>()); // IM SORRY IK ITS BAD FUCKKKKKK
    }
}
