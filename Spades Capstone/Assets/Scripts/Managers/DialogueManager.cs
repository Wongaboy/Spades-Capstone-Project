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

    // [SerializeReference] TMP_Text tallyBoardOpponentName;

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
    private Queue<string> paragraphQueue = new Queue<string>();
    private string currentParagraph = "";
    private bool isDialogueSequenceDone = true;
    private string knownCharacterName = "???";

    private bool isInteractable = true;

    // Letter by Letter Vars & Things
    [SerializeField] private float typeSpeed = 10;
    private bool isTyping = false;
    private Coroutine typeDialogueRoutine;

    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;

    // Test Dialogue Variable
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
        if (Input.GetMouseButtonDown(0) && IsDialogueActive() && isInteractable)
        {
            DisplayNextChunk();
        }  
    }

    // Testing Function to trigger StartDialogue()
    public void TestDialogue()
    {
        EnqueueDialogueSO(testDialogue, false);
        StartCoroutine(ResolveDialogue());
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

    // DEPRECATED -- Resolves DialogueSO in queue, but no phase change
    public IEnumerator ResolveDialogue()
    {
        if (dialogueQueue.Count > 0)
        {
            StartDialogue();
            yield return new WaitUntil(() => (isDialogueSequenceDone == true));
        }
    }

    // Resolve Dialogues until the queue is empty, then change to given Phase
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
        AIManager.Instance.AIAnimator.Play("beginyap");
        isDialogueSequenceDone = false;
        dialogueTextBox.SetActive(true);
        currentDialogue = dialogueQueue.Dequeue();

        foreach(string p in currentDialogue.dialogueTexts)
        {
            paragraphQueue.Enqueue(p);
        }

        if (currentDialogue.dialogueName != "???")
        {
            knownCharacterName = currentDialogue.dialogueName;
            TallyBoard.Instance.updateAIName(knownCharacterName);
        }

        dialogueSpeakerName.text = knownCharacterName;

        DisplayNextChunk(); // Updates Dialogue UI to the first Dialogue Chunk
    }

    // At End of DialogueSO => deactivate DialogueBox OR Continue to next DialogueSO in queue
    public void EndDialogue()
    {
        AIManager.Instance.AIAnimator.Play("endyap");
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

    // Coroutine to type dialogue character by character
    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;

        dialogueText.text = "";

        string originalText = p;
        string displayedText = "";
        int alphaIndex = 0;

        foreach(char c in p.ToCharArray())
        {
            alphaIndex++;
            dialogueText.text = originalText;

            displayedText = dialogueText.text.Insert(alphaIndex, HTML_ALPHA);
            dialogueText.text = displayedText;

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }

        isTyping = false;
    }

    // Skips scrolling & immediately shows text chunk
    private void FinishParagraphEarly()
    {
        StopCoroutine(typeDialogueRoutine);

        dialogueText.text = currentParagraph;

        isTyping = false;
    }

    // Assigned to DialogueUI "Next Button", Moves to next DialogueChunk OR calls EndDialogue()
    public void DisplayNextChunk()
    {
        if (paragraphQueue.Count != 0)
        {
            if (!isTyping)
            {
                currentParagraph = paragraphQueue.Dequeue();
                typeDialogueRoutine = StartCoroutine(TypeDialogueText(currentParagraph));
            }
            else
            {
                FinishParagraphEarly();
            }
        }
        else
        {
            if (isTyping) { FinishParagraphEarly(); }
            else { EndDialogue(); }
        }
    }

    // Return bool -> active state of Dialogue UI Overlay
    public bool IsDialogueActive()
    {
        return dialogueTextBox.activeSelf;
    }

    // Return bool -> is the Dialogue queue empty
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

    // Set if the Dialogue UI recieves input
    public void SetDialogueInteractable(bool canInteract)
    {
        isInteractable = canInteract;
    }

    // DEPRECATED -- ???
    public void TurnOffPressSpace(bool state)
    {
        pressSpaceText.SetActive(state);
    }

}
