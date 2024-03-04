using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    #region "Class Variables"
    // Things to be Deactivated at Game Start
    [SerializeReference] GameObject gameManager;
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
        gameManager.SetActive(true);
        introCamera.SetActive(false);
        introTimeline.SetActive(false);
        foreach(GameObject prop in propsToActivate)
        {
            /*

             This is the area to Add the Chalk VFX for Tallyboard Changes

            */
            prop.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
