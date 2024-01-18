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
    
    // Dialogue Database? -- To Be Added
    private Dictionary<string, DialogueSO> cheatDialogueDatabase= new Dictionary<string, DialogueSO>();
    [SerializeField] List<DialogueSO> allDialogueSO;

    // Queue of DialogueSO's for DialogueManager to process on StartDialogue()
    Queue<DialogueSO> dialogueQueue = new Queue<DialogueSO>();

    // Temp Dialogue to test
    [SerializeField] DialogueSO tempDialogue;
    [SerializeField] DialogueSO tempDialogue2;

    // Current DialogueSO "Manager" is working with
    DialogueSO currentDialogue;
    int dialogueIndex; // Index of DialogueChunk

    #endregion

    private void Start()
    {
        // Disable Dialogue Box on game start
        dialogueTextBox.SetActive(false);

        foreach (DialogueSO dialogue in allDialogueSO)
        {
            cheatDialogueDatabase.Add(dialogue.dialogueTitle, dialogue);
        }
        // Enqueue Some Test Dialogues     
        AddToDialogueQueue(tempDialogue);
        AddToDialogueQueue(tempDialogue2);
        //AddToDialogueQueue(tempDialogue);
    }

    // Testing Function to trigger StartDialogue()
    public void TestDialogue()
    {
        if (dialogueQueue.Count <= 0) { Debug.Log("Nothing in Queue"); }
        else {
            StartDialogue();
        }
    }

    public void AddToDialogueQueue(DialogueSO dialogue)
    {
        dialogueQueue.Enqueue(dialogue);
    }

    public void AddCheatDialogueToQueue(string dialogue)
    {
        dialogueQueue.Enqueue(cheatDialogueDatabase[dialogue]);
    }

    // Starts Going through Queue of DialogueSO's
    public void StartDialogue()
    {
        dialogueTextBox.SetActive(true);
        currentDialogue = dialogueQueue.Dequeue();
        dialogueSpeakerName.text = currentDialogue.dialogueName;
        dialogueIndex = 0;

        OnNextButton(); // Updates Dialogue UI to the first Dialogue Chunk
    }

    // At End of DialogueSO => deactivate DialogueBox OR Continue to next DialogueSO in queue
    public void EndDialogue()
    {
        dialogueIndex = 0;

        if (dialogueQueue.Count <= 0)
        {
            dialogueTextBox.SetActive(false);
        }
        else
        {
            StartDialogue();
        }
    }

    // Update Dialogue Textbox to display given string(Text)
    void UpdateDialogue(string text)
    {
        dialogueText.text = text;
    }

    // Assigned to DialogueUI "Next Button", Moves to next DialogueChunk OR calls EndDialogue()
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
