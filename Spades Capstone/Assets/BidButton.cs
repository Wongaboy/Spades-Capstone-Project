using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BidButton : MonoBehaviour
{
    [SerializeField] private int bidVal;
    [SerializeField] private Image button_img;
    public static event Action<int> setBid;

    public void Awake()
    {
        setBid += imgToggle;
    }

    public void OnDestroy()
    {
        setBid += imgToggle;
    }

    public void OnClick()
    {
        Debug.Log("Clicked");
        setBid.Invoke(bidVal);
    }

    public void imgToggle(int bid)
    {
        //if (bid != bidVal) { button_img. }
        //else { img.enabled = false; }
    }
}
