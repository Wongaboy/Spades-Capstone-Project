using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BidButton : MonoBehaviour
{
    [SerializeField] private int bidVal;
    public static event Action<int> setBid;

    public void OnClick()
    {
        SoundFXManager.Instance.PlayChalkSFX(gameObject.transform, .2f);
        Debug.Log("Clicked");
        setBid.Invoke(bidVal);
    }

}
