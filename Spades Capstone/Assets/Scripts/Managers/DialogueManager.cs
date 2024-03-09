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
    [SerializeField] TMP_Text dialogueSpeakerName;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] GameObject dialogueTextBox;

    [SerializeReference] TMP_Text tallyBoardOpponentName;

    [SerializeReference] GameObject pressSpaceText;

    // Dialogue Databases
    [SerializeField] DialogueSO[] phaseDialogues;

    private Dictionary<CheatName, DialogueSO> cheatDialogueDatabase = new Dictionary<CheatName, DialogueSO>();
    [SerializeField] List<CheatSO> allCheatSO;

    // Card with DialogueSO on Play Database
    private Dictionary<(int, Suit), DialogueSO> cardDialogueDatabase = new Dictionary<(int, Suit), DialogueSO>();

    // Queue of DialogueSO's for DialogueManager to process on StartDialogue()
    Queue<DialogueSO> dialogueQueue = new Queue<DialogueSO>();

    // Current DialogueSO "Manager" is working with
    private DialogueSO currentDialogue;
    private int dialogueIndex; // Index of DialogueChunk
    private bool isDialogueSequenceDone = true;
    private string knownCharacterName = "???";

    [SerializeField] DialogueSO testDialogue;
    #endregion

    private void Start()
    {
        // Disable Dialogue Box on game start
        dialogueTextBox.SetActive(false);

        foreach (CheatSO cheat in allCheatSO)
        {
            cheatDialogueDatabase.Add(cheat.cheatEnumName, cheat.cheatDialogue);
        }
    }

    private void Update()
    {
        // if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && IsDialogueActive())
        if (Input.GetMouseButtonDown(0) && IsDialogueActive())
        {
            OnNextButton();
        }  
    }

    // Testing Function to trigger StartDialogue()
    public void TestDialogue()
    {
        //AddCheatPhaseDialogue(1, false);
        //AddCheatPhaseDialogue(2, false);

        //if (dialogueQueue.Count <= 0) { Debug.Log("nothing in queue"); }
        //else
        //{
        //    AddCheatDialogue(CheatName.ChangeBid, false);
        //    AddCheatDialogue(CheatName.IgnorePenalty, false);
        //    AddCheatDialogue(CheatName.RandomizePlayerCardValue, false);
        //    AddCheatDialogue(CheatName.AddValueFromDiscard, false);
        //}

        EnqueueDialogueSO(testDialogue, false);
        StartCoroutine(ResolveDialogue());
        // StartDialogue();
    }

    // Given Cheatname will add Dialogue to queue & can be told to trigger dialogue immedietly
    public void AddCheatDialogue(CheatName cheatname, bool triggerNow)
    {
        dialogueQueue.Enqueue(cheatDialogueDatabase[cheatname]);
        if (triggerNow == true) { StartDialogue(); }
    }

    // Enqueue any DialogueSO into queue
    public void EnqueueDialogueSO(DialogueSO dialogue, bool triggerNow)
    {
        dialogueQueue.Enqueue(dialogue);
        if (triggerNow == true) { StartDialogue(); }
    }

    // Enqueue CheatPhase Transition Dialogue
    public void AddCheatPhaseDialogue(int phaseNumber, bool triggerNow)
    {
        dialogueQueue.Enqueue(phaseDialogues[phaseNumber - 1]);
        if (triggerNow == true) { StartDialogue(); }
    }

    public IEnumerator ResolveDialogue()
    {
        if (dialogueQueue.Count > 0)
        {
            StartDialogue();
            yield return new WaitUntil(() => (isDialogueSequenceDone == true));
        }
    }

    public IEnumerator ResolveDialogue(Phase phase)
    {
        if (dialogueQueue.Count > 0)
        {
            StartDialogue();
            yield return new WaitUntil(() => (isDialogueSequenceDone == true));
        }

        GameManager.Instance.ChangePhase(phase);
    }

    // Starts Going through Queue of DialogueSO's
    public void StartDialogue()
    {
        isDialogueSequenceDone = false;
        dialogueTextBox.SetActive(true);
        currentDialogue = dialogueQueue.Dequeue();
        if (currentDialogue.dialogueName != "???")
        {
            knownCharacterName = currentDialogue.dialogueName;
            tallyBoardOpponentName.text = knownCharacterName;
        }
        dialogueSpeakerName.text = knownCharacterName;
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
            isDialogueSequenceDone = true;
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

    public bool IsDialogueActive()
    {
        return dialogueTextBox.activeSelf;
    }

    public bool HasDialogueEnqueued()
    {
        if (dialogueQueue.Count > 0) { return true; }
        else { return false; }
    }

    public bool DoesCardHaveDialogue(Card card)
    {
        (int, Suit) cardValues = (card.val, card.suit);

        List<(int, Suit)> allCardsWithDialogue = new List<(int, Suit)>(cardDialogueDatabase.Keys);

        return allCardsWithDialogue.Contains(cardValues);
    }

    public void TurnOffPressSpace(bool state)
    {
        pressSpaceText.SetActive(state);
    }

}
