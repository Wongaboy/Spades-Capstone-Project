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
    TMP_Text dialogueSpeakerName;
    TMP_Text dialogueText;

    GameObject dialogueTextBox;
    Queue<DialogueSO> currentDialogue = new Queue<DialogueSO>();
    int dialogueIndex;
    #endregion

    public void HumanEnter()
    {
        if (currentDialogue.Count > 0)
        {
            DialogueSO dialogue = currentDialogue.Peek();
            dialogueSpeakerName.text = dialogue.dialogueName;
            dialogueIndex = 0;
        }
    }

    public void HumanExit()
    {
        dialogueIndex = 0;
        currentDialogue.Dequeue();
        if (currentDialogue.Count > 0)
        {
            HumanEnter();
        }
    }

    void UpdateDialogue(string text)
    {
        dialogueText.text = text;
    }

    public void OnNextButton()
    {
        if (dialogueIndex < currentDialogue.Peek().dialogueTexts.Count)
        {
            UpdateDialogue(currentDialogue.Peek().dialogueTexts[dialogueIndex]);
            dialogueIndex++;
        }
        else
        {
            HumanExit();
        }
    }

    public void OnDayReset()
    {
        currentDialogue.Clear();
    }
}
