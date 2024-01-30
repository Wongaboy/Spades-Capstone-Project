using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Scroll : MonoBehaviour
{

    [SerializeField]
    private Canvas scrollFaceCanvas;
    // private Transform viewPosition;

    // private bool interactionAllowed;

    // Start is called before the first frame update
    void Awake()
    {
        scrollFaceCanvas.worldCamera = Camera.main;
    }


}
