using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInteraction : MonoBehaviour
{
    private Vector3 mousePosition;
    private bool allowed = false;
    public static event Action<Phase> OnCardMove;
    public static event Action<Phase> OnCardFinMove;
    
    // toggle whether cards are draggable
    public void Active(bool setAllowed)
    {
        allowed = setAllowed;
    }

    // get the world position of the mouse
    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    // if cards are draggable, update the mouse position
    private void OnMouseDown()
    {
        if (allowed)
        {
            mousePosition = Input.mousePosition - GetMousePos();
            OnCardMove.Invoke(GameManager.Instance.currentPhase);
        }
    }

    // if cards are draggable, drag them
    private void OnMouseDrag()
    {
        if (allowed)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        }
    }

    // Tell the game that the card has finished being dragged
    private void OnMouseUp() 
    { 
        if (allowed) 
        {
            OnCardFinMove.Invoke(GameManager.Instance.currentPhase);
        }
    }
}
