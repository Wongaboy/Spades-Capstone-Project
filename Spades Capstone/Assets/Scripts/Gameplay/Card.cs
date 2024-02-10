using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Data")]
    public Suit suit;
    public int val;
    [SerializeField] 
    private float cardSpeed = 10f;
    [SerializeField] private DialogueSO dialogueOnPlay = null;

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

    override public string ToString()
    {
        return val.ToString() + " of " + suit.ToString() + "s";
    }

    // method to prevent card movement at inappropriate intervals
    public void SetInteractable(bool interactable)
    {
        cardInteraction.Active(interactable);
    }

    // call this to move a card anywhere in space - will happen in .05*cardSpeed number of frames
    public void MoveToLocation(Vector3 location, Quaternion rotation, bool UseGravityOnEnd=false)
    {
        StartCoroutine(TravelTo(location, rotation, UseGravityOnEnd));
    }

    // this is kept private so that ppl don't have to remember to use StartCoroutine() whenever they want to move a card
    private IEnumerator TravelTo(Vector3 location, Quaternion rotation, bool UseGravOnEnd)
    {
        cardBody.useGravity = false;
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
        cardBody.useGravity = UseGravOnEnd;
    }

    public bool HasDialogueAttached()
    {
        if (dialogueOnPlay != null) { return true; }
        else { return false; }
    }

    public DialogueSO GetDialogueSO()
    {
        return dialogueOnPlay;
    }


}

public enum Suit { SPADE, HEART, CLUB, DIAMOND}
