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
    
    public void Active(bool setAllowed)
    {
        allowed = setAllowed;
    }

    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {
        if (allowed)
        {
            mousePosition = Input.mousePosition - GetMousePos();
            OnCardMove.Invoke(GameManager.Instance.currentPhase);
        }
    }

    private void OnMouseDrag()
    {
        if (allowed)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        }
    }

    private void OnMouseUp() 
    { 
        if (allowed) 
        {
            OnCardFinMove.Invoke(GameManager.Instance.currentPhase);
        }
    }
}
