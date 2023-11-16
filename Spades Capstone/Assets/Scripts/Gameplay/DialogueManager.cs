using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    #region "Singleton"
    private static DialogueManager _instance;

    public static DialogueManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    #region "Class Variables"
    // !Work in Progress Variables!
    [SerializeField] TMP_Text dialogueSpeakerName;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] GameObject dialogueTextBox;
    
    // Current DialogueSO "Manager" is working with
    DialogueSO currentDialogue;
    int dialogueIndex;

    // Temp Dialogue to test
    [SerializeField] DialogueSO tempDialogue;
    #endregion

    private void Start()
    {
        // Disable Dialogue Box on game start
        dialogueTextBox.SetActive(false);    
    }

    // Testing Function to trigger StartDialogue()
    public void TestDialogue()
    {
        StartDialogue(tempDialogue);
    }

    // Called with given DialogueSO to activate DialogueBox, etc.
    public void StartDialogue(DialogueSO newDialogue)
    {
        dialogueTextBox.SetActive(true);
        currentDialogue = newDialogue;
        dialogueSpeakerName.text = currentDialogue.dialogueName;
        dialogueIndex = 0;
        OnNextButton();
    }

    // Called at end of Dialogue to deactivate DialogueBox and reset variables
    public void EndDialogue()
    {
        dialogueTextBox.SetActive(false);
        dialogueIndex = 0;
    }

    // Update Dialogue displays with given text string
    void UpdateDialogue(string text)
    {
        dialogueText.text = text;
    }

    // Called on DialogueButton Press to move dialogue forward OR Exit dialogue
    public void OnNextButton()
    {
        if (dialogueIndex < currentDialogue.dialogueTexts.Count)
        {
            UpdateDialogue(currentDialogue.dialogueTexts[dialogueIndex]);
            dialogueIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

}
