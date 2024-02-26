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
    private bool dialogueTriggered = false;

    [Header("Components")]
    [SerializeField]
    private Canvas cardFaceCanvas;
    [SerializeField]
    private Rigidbody cardBody;
    [SerializeField]
    private CardInteraction cardInteraction;
  
    private Transform cardBorderVFX;
    private MeshRenderer cardBorderVFXRenderer;

    [SerializeField] private Material[] greenMaterials;
    [SerializeField] private Material[] redMaterials;

    void Awake(){
        cardFaceCanvas.worldCamera = Camera.main;

        cardBorderVFX = this.gameObject.transform.GetChild(1);
        cardBorderVFXRenderer = cardBorderVFX.gameObject.GetComponent<MeshRenderer>();
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
        if (dialogueOnPlay != null && dialogueTriggered == false) { return true; }
        else { return false; }
    }

    public DialogueSO GetDialogueSO()
    {
        dialogueTriggered = true;
        return dialogueOnPlay;
    }

    public void ToggleOnVFXBorder(bool isActive, bool isValidMove)
    {
        if (isValidMove) { 
            cardBorderVFXRenderer.materials = greenMaterials; 
        }
        else { 
            cardBorderVFXRenderer.materials = redMaterials; 
        }

        cardBorderVFX.gameObject.SetActive(isActive);
    }
}

public enum Suit { SPADE, HEART, CLUB, DIAMOND}
