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
    private Collider cardCollider;

    void Awake(){
        cardObj = this.gameObject;
        cardFaceCanvas.worldCamera = Camera.main;
    }

    public void Freeze()
    {
        cardBody.useGravity = false;
        cardCollider.enabled = false;
    }

    public void Unfreeze()
    {
        cardBody.useGravity = true;
        cardCollider.enabled = true;
    }
}

public enum Suit { SPADE, HEART, CLUB, DIAMOND}
