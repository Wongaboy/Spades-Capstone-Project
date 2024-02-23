using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class confirmBid : MonoBehaviour
{
    public void OnClick()
    {
        BidUI.Instance.ConfirmPlayerBid();
    }
}
