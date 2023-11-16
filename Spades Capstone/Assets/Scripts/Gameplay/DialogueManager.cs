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
    
    DialogueSO currentDialogue;
    int dialogueIndex;

    [SerializeField] DialogueSO tempDialogue;
    #endregion
    private void Start()
    {
        dialogueTextBox.SetActive(false);    
    }

    // Testing Function
    public void TestDialogue()
    {
        StartDialogue(tempDialogue);
    }
    public void StartDialogue(DialogueSO newDialogue)
    {
        dialogueTextBox.SetActive(true);
        currentDialogue = newDialogue;
        dialogueSpeakerName.text = currentDialogue.dialogueName;
        dialogueIndex = 0;
        OnNextButton();
    }

    public void EndDialogue()
    {
        dialogueTextBox.SetActive(false);
        dialogueIndex = 0;
    }

    void UpdateDialogue(string text)
    {
        dialogueText.text = text;
    }

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
