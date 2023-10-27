using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftUI : MonoBehaviour
{
    [SerializeField] GameObject DraftUIPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        DraftUIPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called to Trigger Draft UI Prompt for Player to Choose Card
    public void ToggleDraftUI()
    {
        DraftUIPanel.SetActive(!DraftUIPanel.activeSelf);
    }

    // Function for when Players KEEPs Drawn Card (KEEP Button Pressed)
    public void OnKeepClick()
    {
        // Disable UI
        ToggleDraftUI();

        // Add Card to Player's Hand

        // Discard Next Card in Deck

        // Switch Turns

        Debug.Log("KEEP Clicked");
    }

    // Function for when Players DISCARDs Drawn Card (DISCARD Button Pressed)
    public void OnDiscardClick()
    {
        // Disable UI 
        ToggleDraftUI();

        // Discard currently Viewed Card

        // Add Next Card to Player Hand

        // Switch Turns

        Debug.Log("DISCARD Clicked");
    }
}
