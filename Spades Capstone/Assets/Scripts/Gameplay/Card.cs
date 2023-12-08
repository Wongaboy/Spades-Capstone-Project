using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Suit suit;
    public int val;
    [HideInInspector]
    public GameObject cardObj;
    [SerializeField]
    private Canvas cardFaceCanvas;
    [SerializeField]
    private Rigidbody cardBody;
    [SerializeField]
    private CardInteraction cardInteraction;

    void Awake(){
        cardObj = this.gameObject;
        cardFaceCanvas.worldCamera = Camera.main;
    }

    public void Freeze()
    {
        cardBody.useGravity = false;
    }

    public void Unfreeze()
    {
        cardBody.useGravity = true;
    }

    public void SetInteractable(bool interactable)
    {
        cardInteraction.enabled = interactable;
    }
}

public enum Suit { SPADE, HEART, CLUB, DIAMOND}
