using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    #region "Class Variables"
    // Things to be Deactivated at Game Start
    [SerializeReference] GameObject mainCamera;

    [SerializeReference] GameObject introCamera;
    [SerializeReference] GameObject introTimeline;

    [SerializeReference] List<GameObject> propsToActivate;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // Deactive Intro Camera & Timeline; Activate Main Camera
        mainCamera.SetActive(true);
        introCamera.SetActive(false);
        introTimeline.SetActive(false);
        foreach(GameObject prop in propsToActivate)
        {
            /*

             This is the area to Add the Chalk VFX for Tallyboard Changes

            */
            if(prop.tag != "deck")
            {
                StartCoroutine(ShowProp(prop));
            }
            else
            {
                prop.SetActive(true);
            }
            
        }
        // Call to Start Gameloop
        GameManager.Instance.StartGameAfterIntro();
    }

    IEnumerator ShowProp(GameObject prop)
    {
        Vector3 finalPos = prop.transform.position;
        float decrement = 2.5f;
        prop.transform.position = new Vector3(finalPos.x, finalPos.y + decrement, finalPos.z);
        prop.SetActive(true);
        
        while(prop.transform.position != finalPos)
        {
            decrement -= .05f;
            yield return new WaitForSeconds(.05f);
            prop.transform.position = new Vector3(finalPos.x, finalPos.y + decrement, finalPos.z);
        }
        
    }
}
