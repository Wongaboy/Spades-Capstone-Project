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

    override public string ToString()
    {
        return val.ToString() + " of " + suit.ToString() + "s";
    }

    public void SetInteractable(bool interactable)
    {
        cardInteraction.Active(interactable);
    }

    // Will eventually use this to move cards into hands and on to tables and such - for the sake of JUICE
    public void MoveToLocation(Vector3 location, Quaternion rotation)
    {
        cardObj.transform.position = location;
        cardObj.transform.rotation = rotation;
    }
}

public enum Suit { SPADE, HEART, CLUB, DIAMOND}
