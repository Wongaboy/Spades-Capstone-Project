using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInteraction : MonoBehaviour
{
    private Vector3 mousePosition;
    private bool allowed;
    
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
        }
    }

    private void OnMouseDrag()
    {
        if (allowed)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        }
    }
}
