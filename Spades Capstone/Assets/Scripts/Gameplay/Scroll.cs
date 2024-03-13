using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class Scroll : MonoBehaviour
{

    [SerializeField]
    private Canvas scrollFaceCanvas;
    [SerializeField]
    private Transform viewSpot;
    [SerializeField]
    private Transform tableSpot;
    

    private bool interactionAllowed = true;
    private float scrollSpeed = 20f;

    // Start is called before the first frame update
    void Awake()
    {
        scrollFaceCanvas.worldCamera = Camera.main;

    }

    private void OnMouseDown()
    {
        if (interactionAllowed && gameObject.transform.position == tableSpot.position)
        {
            StartCoroutine(TravelTo(viewSpot.position, viewSpot.rotation));
        }
        else if (interactionAllowed)
        {
            StartCoroutine(TravelTo(tableSpot.position, tableSpot.rotation));
        }
        interactionAllowed = false;
    }

    private IEnumerator TravelTo(Vector3 location, Quaternion rotation)
    {
        Vector3 locInc = (location - gameObject.transform.position) / scrollSpeed;
        Quaternion rotQ = Quaternion.Inverse(gameObject.transform.rotation) * rotation;
        Quaternion rotInc = Quaternion.Slerp(Quaternion.identity, rotQ, 1f / scrollSpeed);
        for (int i = 0; i < scrollSpeed; i++)
        {
            yield return new WaitForSeconds(.05f);
            gameObject.transform.position += locInc;
            gameObject.transform.rotation *= rotInc;

        }
        interactionAllowed = true;
    }


}
