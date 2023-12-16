using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Data")]
    public Suit suit;
    public int val;
    [SerializeField] float cardSpeed = 10f;

    [Header("Components")]
    [SerializeField]
    private Canvas cardFaceCanvas;
    [SerializeField]
    private Rigidbody cardBody;
    [SerializeField]
    private CardInteraction cardInteraction;

    void Awake(){
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
        //gameObject.transform.position = location;
        //gameObject.transform.rotation = rotation;
        StartCoroutine(TravelTo(location, rotation));
    }

    private IEnumerator TravelTo(Vector3 location, Quaternion rotation)
    {
        cardBody.detectCollisions = false;
        Vector3 locInc = (location - gameObject.transform.position)/cardSpeed;
        Quaternion rotQ = Quaternion.Inverse(gameObject.transform.rotation) * rotation;
        Quaternion rotInc = Quaternion.Slerp(Quaternion.identity, rotQ, 1f / cardSpeed);
        for (int i = 0; i< cardSpeed; i++)
        {
            yield return new WaitForSeconds(.05f);
            gameObject.transform.position += locInc;
            gameObject.transform.rotation *= rotInc;
            
        }
        
        cardBody.detectCollisions = true;
    }
}

public enum Suit { SPADE, HEART, CLUB, DIAMOND}
